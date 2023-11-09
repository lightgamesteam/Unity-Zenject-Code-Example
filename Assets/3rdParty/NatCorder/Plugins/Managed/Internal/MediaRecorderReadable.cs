/* 
*   NatCorder
*   Copyright (c) 2019 Yusuf Olokoba.
*/

namespace NatCorder.Internal {

    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;
    using System;
    using System.Runtime.InteropServices;
    using Readback;

    public abstract class ReadableTexture : IDisposable {

        protected ReadableTexture (RenderTexture reTex) => this.renderTexture = reTex;

        public abstract void Dispose ();

        public abstract void Readback (Action<IntPtr> handler);

        public static implicit operator RenderTexture (ReadableTexture readable) => readable.renderTexture;

        public static ReadableTexture ToReadable (RenderTexture reTex) {

#if UNITY_WEBGL
            return new SyncReadableTexture(reTex);
#else
switch (SystemInfo.graphicsDeviceType) {
                case GraphicsDeviceType.OpenGLES2:
                case GraphicsDeviceType.OpenGLES3:
                    return new GLESReadableTexture(reTex, false); // Use multithreading? Insane performance gains, but might cause JNI error on Unity < 2019.2
                case var _ when SystemInfo.supportsAsyncGPUReadback:
                    return new AsyncReadableTexture(reTex);
                default:
                    return new SyncReadableTexture(reTex);
            }
#endif
        }

        protected readonly RenderTexture renderTexture;
    }

    namespace Readback {

        public sealed class SyncReadableTexture : ReadableTexture {

            public SyncReadableTexture (RenderTexture reTex) : base(reTex) {
                this.readbackBuffer = new Texture2D(reTex.width, reTex.height, TextureFormat.RGBA32, false, false);
                this.pixelBuffer = new byte[reTex.width * reTex.height * 4];
            }

            public override void Dispose () => Texture2D.Destroy(readbackBuffer);

            public override void Readback (Action<IntPtr> handler) {
                // Readback
                RenderTexture.active = renderTexture;
                readbackBuffer.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
                readbackBuffer.GetRawTextureData<byte>().CopyTo(pixelBuffer);
                // Invoke handler
                var handle = GCHandle.Alloc(pixelBuffer, GCHandleType.Pinned);
                handler(handle.AddrOfPinnedObject());
                handle.Free();
            }

            private readonly Texture2D readbackBuffer;
            private readonly byte[] pixelBuffer;
        }

        public sealed class AsyncReadableTexture : ReadableTexture {

            public AsyncReadableTexture (RenderTexture reTex) : base(reTex) => this.pixelBuffer = new byte[reTex.width * reTex.height * 4];

            public override void Dispose () => pixelBuffer = null;

            public override void Readback (Action<IntPtr> handler) {
                AsyncGPUReadback.Request(renderTexture, 0, request => {
                    if (pixelBuffer == null)
                        return;
                    request.GetData<byte>().CopyTo(pixelBuffer);
                    var handle = GCHandle.Alloc(pixelBuffer, GCHandleType.Pinned);
                    handler(handle.AddrOfPinnedObject());
                    handle.Free();
                });
            }

            private byte[] pixelBuffer;
        }

        public sealed class GLESReadableTexture : ReadableTexture {

            public GLESReadableTexture (RenderTexture reTex, bool multiThread = false) : base(reTex) {
                // Get texture ptr
                var _= reTex.colorBuffer;
                // Setup native resources
                this.Unmanaged = new AndroidJavaClass(@"com.natsuite.natrender.Unmanaged");
                var callback = new Callback((context, nativeBuffer) => {
                    var handle = (GCHandle)(IntPtr)context;
                    var handler = handle.Target as Action<IntPtr>;
                    handle.Free();
                    if (Unmanaged != null)
                        handler((IntPtr)Unmanaged.CallStatic<long>(@"baseAddress", nativeBuffer));
                });
                this.texture = new AndroidJavaObject(
                    @"com.natsuite.natcorder.readback.GLESReadableTexture",
                    reTex.GetNativeTexturePtr().ToInt32(),
                    reTex.width,
                    reTex.height,
                    callback,
                    multiThread
                );
                // Attach render thread to JNI
                using (var dispatcher = new RenderDispatcher())
                    dispatcher.Dispatch(() => AndroidJNI.AttachCurrentThread());
            }

            public override void Dispose () {
                Unmanaged.Dispose();
                Unmanaged = null;
                texture.Call(@"release");
                texture.Dispose();
                texture = null;
            }

            public override void Readback (Action<IntPtr> handler) {
                GL.Flush();
                using (var dispatcher = new RenderDispatcher())
                    dispatcher.Dispatch(() => texture?.Call(@"readback", (long)(IntPtr)GCHandle.Alloc(handler, GCHandleType.Normal)));
            }

            private AndroidJavaClass Unmanaged;
            private AndroidJavaObject texture;

            private class Callback : AndroidJavaProxy {

                private readonly Action<long, AndroidJavaObject> handler;
                public Callback (Action<long, AndroidJavaObject> handler) : base(@"com.natsuite.natcorder.readback.ReadableTexture$Callback") => this.handler = handler;
                
                [Preserve]
                private void onReadback (long context, AndroidJavaObject nativeBuffer) {
                    handler(context, nativeBuffer);
                    nativeBuffer.Dispose();
                }

                [Preserve]
                public override AndroidJavaObject Invoke (string methodName, AndroidJavaObject[] javaArgs) { // Subvert Reflection, save time
                    var context = javaArgs[0].Call<long>(@"longValue");
                    var nativeBuffer = javaArgs[1];
                    onReadback(context, nativeBuffer);
                    return null;
                }
            }
        }
    }
}