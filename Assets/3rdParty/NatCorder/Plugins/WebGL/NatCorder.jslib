const NatCorderWebGL = {
    $sharedInstance: {
        startTime: 0,
        recordingCallback: null,
        recordingContext: null,
        framebuffer: null,
        framebufferContext: null,
        pixelBuffer: null,
        audioContext: null,
        audioStream: null,
        recorder: null,
        MIME_TYPE: "video/webm",
        IsWebKit: false
    },
    NCCreateMP4Recorder: function (e, n, a, r, t, o, s) {
        sharedInstance.framebuffer = document.createElement("canvas"), sharedInstance.framebuffer.width = e, sharedInstance.framebuffer.height = n, sharedInstance.framebufferContext = sharedInstance.framebuffer.getContext("2d"), sharedInstance.pixelBuffer = sharedInstance.framebufferContext.getImageData(0, 0, e, n);
        const c = [sharedInstance.framebuffer.captureStream(a).getVideoTracks()[0]];

        var isWebkit = /Safari/.test(navigator.userAgent) && /Apple Computer/.test(navigator.vendor);

        if (isWebkit) {
            sharedInstance.MIME_TYPE = "video/mp4;codecs=avc1";
            sharedInstance.IsWebKit = true;
        }

        var stream = document.audioStream;

        document.position = 0;
        document.audioContext = new AudioContext;
        document.tempSize = 1024;
        document.tempArray = new Float32Array(document.tempSize);
        document.analyser = document.audioContext.createAnalyser();
        document.analyser.minDecibels = -90;
        document.analyser.maxDecibels = -10;
        document.analyser.smoothingTimeConstant = .85;
        document.mediaRecorder = new MediaRecorder(stream);
        document.source = document.audioContext.createMediaStreamSource(stream);
        document.source.connect(document.analyser);
        document.readDataOnInterval = (function () {
            if (document.dataArray == undefined) {
                setTimeout(document.readDataOnInterval, 250);
                return
            }
            document.tempInterval = Math.floor(document.tempSize / document.dataArray.length * 250);
            setTimeout(document.readDataOnInterval, document.tempInterval);
            if (document.dataArray == undefined) {
                return
            }
            document.analyser.getFloatTimeDomainData(document.tempArray);
            document.volume = 0;
            var j = (document.position + document.dataArray.length - document.tempSize) % document.dataArray.length;
            for (var i = 0; i < document.tempSize; ++i) {
                document.volume = Math.max(document.volume, Math.abs(document.tempArray[i]));
                document.dataArray[j] = document.tempArray[i];
                j = (j + 1) % document.dataArray.length
            }
            document.position = (document.position + document.tempSize) % document.dataArray.length
        });
        document.readDataOnInterval()

        var outputTracks = [];
        outputTracks = outputTracks.concat(c);
        outputTracks = outputTracks.concat(document.mediaRecorder.stream.getAudioTracks());
        const d = {
            mimeType: sharedInstance.MIME_TYPE,
            videoBitsPerSecond: r
        };
        return sharedInstance.recorder = new MediaRecorder(new MediaStream(outputTracks), d), 1
    },
    NCCreateGIFRecorder: function (e, n, a) {
        return 0
    },
    NCCreateHEVCRecorder: function (e, n, a, r, t, o, s) {
        return 0
    },
    NCStartRecording: function (e, n, a, r) {
        startTime = Date.now();
        sharedInstance.recordingCallback = a, sharedInstance.recordingContext = r, sharedInstance.recorder.start(), document.mediaRecorder.start(), console.log("NatCorder: Starting recording")
    },
    NCStopRecording: function (e) {
        FS.syncfs(false, (function (err) { }));
        sharedInstance.recorder.stop();
        document.mediaRecorder.stop();
        document.audioStream.getTracks().forEach(function (track) {
            track.stop();
        });
        sharedInstance.framebuffer = null;
        sharedInstance.framebufferContext = null;
        sharedInstance.pixelBuffer = null;
        sharedInstance.audioContext = null;
        console.log("NatCorder: Stopping recording"), sharedInstance.recorder.ondataavailable = (function (e) {
            console.log(e);
            const n = new Blob([e.data], {
                type: sharedInstance.MIME_TYPE
            });
            var duration = Date.now() - startTime;

            if (sharedInstance.IsWebKit) {
                a = URL.createObjectURL(n);
                console.log("NatCorder: Completed recording video", n, "to URL:", a);
                const r = lengthBytesUTF8(a) + 1,
                    t = _malloc(r);
                stringToUTF8(a, t, r), Runtime.dynCall("vii", sharedInstance.recordingCallback, [sharedInstance.recordingContext, t]), _free(t)
            }

            else {
                ysFixWebmDuration(n, duration, (function (fixedBlob) {
                    a = URL.createObjectURL(fixedBlob);
                    console.log(e.size);
                    console.log(sharedInstance.MIME_TYPE);
                    console.log(document.mediaRecorder);
                    console.log(e.data);
                    console.log(fixedBlob.type + " " + fixedBlob.size);
                    console.log("NatCorder: Completed recording video", fixedBlob, "to URL:", a);
                    const r = lengthBytesUTF8(a) + 1,
                        t = _malloc(r);
                    stringToUTF8(a, t, r), Runtime.dynCall("vii", sharedInstance.recordingCallback, [sharedInstance.recordingContext, t]), _free(t)
                }))
            }
        })
    },
    NCEncodeFrame: function (e, n, a, r) {
        for (var t = sharedInstance.pixelBuffer.width, o = sharedInstance.pixelBuffer.height, s = 4 * t, c = 0; c < o; c++) sharedInstance.pixelBuffer.data.set(new Uint8Array(HEAPU8.buffer, n + (o - c - 1) * s, s), c * s);
        sharedInstance.framebufferContext.putImageData(sharedInstance.pixelBuffer, 0, 0)
    },
    NCEncodeSamples: function (e, n, a, r) {
        const t = document.audioContext.createBuffer(sharedInstance.channelCount, a / sharedInstance.channelCount, sharedInstance.sampleRate);
        n = new Float32Array(HEAPU8.buffer, n, a);
        for (var o = 0; o < t.numberOfChannels; o++) {
            const e = t.getChannelData(o);
            for (var s = 0; s < t.length; s++) e[s] = n[s * t.numberOfChannels + o]
        }
        var c = document.audioContext.createBufferSource();
        c.buffer = t, c.connect(document.mediaRecorder.stream), c.start()
    },
    NCCurrentTimestamp: function () {
        return Math.round(1e6 * performance.now())
    }
};
autoAddDeps(NatCorderWebGL, "$sharedInstance"), mergeInto(LibraryManager.library, NatCorderWebGL);