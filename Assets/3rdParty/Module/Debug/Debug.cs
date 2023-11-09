using System;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Module {
    public static class Debug {
        private static readonly LogForm _logForm = new LogForm();

        #region LogError

        public static void LogError(object msg) { CreateAndSendForm(LogType.Error, msg); }

        public static void LogError(this object obj, object msg) { LogError(obj.GetType(), msg); }
        public static void LogError(this object obj, object msg, Color32 classColor) { LogError(obj.GetType(), msg, classColor); }
        public static void LogError(this object obj, object msg, Color32 classColor, Color32 msgColor) { LogError(obj.GetType(), msg, classColor, msgColor); }
        public static void LogError(this object obj, object title, object msg) { LogError(obj.GetType(), title, msg); }
        public static void LogError(this object obj, object title, object msg, Color32 classColor) { LogError(obj.GetType(), title, msg, classColor); }
        public static void LogError(this object obj, object title, object msg, Color32 classColor, Color32 titleColor) { LogError(obj.GetType(), title, msg, classColor, titleColor); }
        public static void LogError(this object obj, object title, object msg, Color32 classColor, Color32 titleColor, Color32 msgColor) { LogError(obj.GetType(), title, msg, classColor, titleColor, msgColor); }

        public static void LogError(Type classType, object msg) { CreateAndSendForm(LogType.Error, classType, msg); }
        public static void LogError(Type classType, object msg, Color32 classColor) { CreateAndSendForm(LogType.Error, classType, msg, classColor); }
        public static void LogError(Type classType, object msg, Color32 classColor, Color32 msgColor) { CreateAndSendForm(LogType.Error, classType, msg, classColor, msgColor); }
        public static void LogError(Type classType, object title, object msg) { CreateAndSendForm(LogType.Error, classType, title, msg); }
        public static void LogError(Type classType, object title, object msg, Color32 classColor) { CreateAndSendForm(LogType.Error, classType, title, msg, classColor); }
        public static void LogError(Type classType, object title, object msg, Color32 classColor, Color32 titleColor) { CreateAndSendForm(LogType.Error, classType, title, msg, classColor, titleColor); }
        public static void LogError(Type classType, object title, object msg, Color32 classColor, Color32 titleColor, Color32 msgColor) { CreateAndSendForm(LogType.Error, classType, title, msg, classColor, titleColor, msgColor); }

        public static void LogErrorRed(this object obj, object msg) { obj.LogError(msg, Color.red); }
        public static void LogErrorRed(this object obj, object title, object msg) { obj.LogError(title, msg, Color.red); }

        public static void LogErrorGreen(this object obj, object msg) { obj.LogError(msg, Color.green); }
        public static void LogErrorGreen(this object obj, object title, object msg) { obj.LogError(title, msg, Color.green); }

        public static void LogErrorBlue(this object obj, object msg) { obj.LogError(msg, Color.blue); }
        public static void LogErrorBlue(this object obj, object title, object msg) { obj.LogError(title, msg, Color.blue); }

        public static void LogErrorYellow(this object obj, object msg) { obj.LogError(msg, Color.yellow); }
        public static void LogErrorYellow(this object obj, object title, object msg) { obj.LogError(title, msg, Color.yellow); }

        #endregion

        #region Log

        public static void Log(object msg) { CreateAndSendForm(LogType.Log, msg); }

        public static void Log(this object obj, object msg) { Log(obj.GetType(), msg); }
        public static void Log(this object obj, object msg, Color32 classColor) { Log(obj.GetType(), msg, classColor); }
        public static void Log(this object obj, object msg, Color32 classColor, Color32 msgColor) { Log(obj.GetType(), msg, classColor, msgColor); }
        public static void Log(this object obj, object title, object msg) { Log(obj.GetType(), title, msg); }
        public static void Log(this object obj, object title, object msg, Color32 classColor) { Log(obj.GetType(), title, msg, classColor); }
        public static void Log(this object obj, object title, object msg, Color32 classColor, Color32 titleColor) { Log(obj.GetType(), title, msg, classColor, titleColor); }
        public static void Log(this object obj, object title, object msg, Color32 classColor, Color32 titleColor, Color32 msgColor) { Log(obj.GetType(), title, msg, classColor, titleColor, msgColor); }

        public static void Log(Type classType, object msg) { CreateAndSendForm(LogType.Log, classType, msg); }
        public static void Log(Type classType, object msg, Color32 classColor) { CreateAndSendForm(LogType.Log, classType, msg, classColor); }
        public static void Log(Type classType, object msg, Color32 classColor, Color32 msgColor) { CreateAndSendForm(LogType.Log, classType, msg, classColor, msgColor); }
        public static void Log(Type classType, object title, object msg) { CreateAndSendForm(LogType.Log, classType, title, msg); }
        public static void Log(Type classType, object title, object msg, Color32 classColor) { CreateAndSendForm(LogType.Log, classType, title, msg, classColor); }
        public static void Log(Type classType, object title, object msg, Color32 classColor, Color32 titleColor) { CreateAndSendForm(LogType.Log, classType, title, msg, classColor, titleColor); }
        public static void Log(Type classType, object title, object msg, Color32 classColor, Color32 titleColor, Color32 msgColor) { CreateAndSendForm(LogType.Log, classType, title, msg, classColor, titleColor, msgColor); }

        public static void LogRed(this object obj, object msg) { obj.Log(msg, Color.red); }
        public static void LogRed(this object obj, object title, object msg) { obj.Log(title, msg, Color.red); }

        public static void LogGreen(this object obj, object msg) { obj.Log(msg, Color.green); }
        public static void LogGreen(this object obj, object title, object msg) { obj.Log(title, msg, Color.green); }

        public static void LogBlue(this object obj, object msg) { obj.Log(msg, Color.blue); }
        public static void LogBlue(this object obj, object title, object msg) { obj.Log(title, msg, Color.blue); }

        public static void LogYellow(this object obj, object msg) { obj.Log(msg, Color.yellow); }
        public static void LogYellow(this object obj, object title, object msg) { obj.Log(title, msg, Color.yellow); }

        #endregion

        #region LogWarning

        public static void LogWarning(object msg) { CreateAndSendForm(LogType.Warning, msg); }

        public static void LogWarning(this object obj, object msg) { LogWarning(obj.GetType(), msg); }
        public static void LogWarning(this object obj, object msg, Color32 classColor) { LogWarning(obj.GetType(), msg, classColor); }
        public static void LogWarning(this object obj, object msg, Color32 classColor, Color32 msgColor) { LogWarning(obj.GetType(), msg, classColor, msgColor); }
        public static void LogWarning(this object obj, object title, object msg) { LogWarning(obj.GetType(), title, msg); }
        public static void LogWarning(this object obj, object title, object msg, Color32 classColor) { LogWarning(obj.GetType(), title, msg, classColor); }
        public static void LogWarning(this object obj, object title, object msg, Color32 classColor, Color32 titleColor) { LogWarning(obj.GetType(), title, msg, classColor, titleColor); }
        public static void LogWarning(this object obj, object title, object msg, Color32 classColor, Color32 titleColor, Color32 msgColor) { LogWarning(obj.GetType(), title, msg, classColor, titleColor, msgColor); }

        public static void LogWarning(Type classType, object msg) { CreateAndSendForm(LogType.Warning, classType, msg); }
        public static void LogWarning(Type classType, object msg, Color32 classColor) { CreateAndSendForm(LogType.Warning, classType, msg, classColor); }
        public static void LogWarning(Type classType, object msg, Color32 classColor, Color32 msgColor) { CreateAndSendForm(LogType.Warning, classType, msg, classColor, msgColor); }
        public static void LogWarning(Type classType, object title, object msg) { CreateAndSendForm(LogType.Warning, classType, title, msg); }
        public static void LogWarning(Type classType, object title, object msg, Color32 classColor) { CreateAndSendForm(LogType.Warning, classType, title, msg, classColor); }
        public static void LogWarning(Type classType, object title, object msg, Color32 classColor, Color32 titleColor) { CreateAndSendForm(LogType.Warning, classType, title, msg, classColor, titleColor); }
        public static void LogWarning(Type classType, object title, object msg, Color32 classColor, Color32 titleColor, Color32 msgColor) { CreateAndSendForm(LogType.Warning, classType, title, msg, classColor, titleColor, msgColor); }

        public static void LogWarningRed(this object obj, object msg) { obj.LogWarning(msg, Color.red); }
        public static void LogWarningRed(this object obj, object title, object msg) { obj.LogWarning(title, msg, Color.red); }

        public static void LogWarningGreen(this object obj, object msg) { obj.LogWarning(msg, Color.green); }
        public static void LogWarningGreen(this object obj, object title, object msg) { obj.LogWarning(title, msg, Color.green); }

        public static void LogWarningBlue(this object obj, object msg) { obj.LogWarning(msg, Color.blue); }
        public static void LogWarningBlue(this object obj, object title, object msg) { obj.LogWarning(title, msg, Color.blue); }

        public static void LogWarningYellow(this object obj, object msg) { obj.LogWarning(msg, Color.yellow); }
        public static void LogWarningYellow(this object obj, object title, object msg) { obj.LogWarning(title, msg, Color.yellow); }

        #endregion

        #region Forms

        private static void CreateAndSendForm(LogType logType, object msg) {
            _logForm.ClassForm.RemoveForm();
            _logForm.TitleForm.RemoveForm();
            _logForm.MsgForm.CreateForm(msg);
            SendForm(logType);
        }

        private static void CreateAndSendForm(LogType logType, Type classType, object msg) {
            _logForm.ClassForm.CreateForm(classType);
            _logForm.TitleForm.RemoveForm();
            _logForm.MsgForm.CreateForm(msg);
            SendForm(logType);
        }

        private static void CreateAndSendForm(LogType logType, Type classType, object msg, Color32 classColor) {
            _logForm.ClassForm.CreateForm(classType, classColor);
            _logForm.TitleForm.RemoveForm();
            _logForm.MsgForm.CreateForm(msg);
            SendForm(logType);
        }

        private static void CreateAndSendForm(LogType logType, Type classType, object msg, Color32 classColor, Color32 msgColor) {
            _logForm.ClassForm.CreateForm(classType, classColor);
            _logForm.TitleForm.RemoveForm();
            _logForm.MsgForm.CreateForm(msg, msgColor);
            SendForm(logType);
        }

        private static void CreateAndSendForm(LogType logType, Type classType, object title, object msg) {
            _logForm.ClassForm.CreateForm(classType);
            _logForm.TitleForm.CreateForm(title);
            _logForm.MsgForm.CreateForm(msg);
            SendForm(logType);
        }

        private static void CreateAndSendForm(LogType logType, Type classType, object title, object msg, Color32 classColor) {
            _logForm.ClassForm.CreateForm(classType, classColor);
            _logForm.TitleForm.CreateForm(title);
            _logForm.MsgForm.CreateForm(msg);
            SendForm(logType);
        }

        private static void CreateAndSendForm(LogType logType, Type classType, object title, object msg, Color32 classColor, Color32 titleColor) {
            _logForm.ClassForm.CreateForm(classType, classColor);
            _logForm.TitleForm.CreateForm(title, titleColor);
            _logForm.MsgForm.CreateForm(msg);
            SendForm(logType);
        }

        private static void CreateAndSendForm(LogType logType, Type classType, object title, object msg, Color32 classColor, Color32 titleColor, Color32 msgColor) {
            _logForm.ClassForm.CreateForm(classType, classColor);
            _logForm.TitleForm.CreateForm(title, titleColor);
            _logForm.MsgForm.CreateForm(msg, msgColor);
            SendForm(logType);
        }

        private static void SendForm(LogType logType) {
            if (UnityDebug.isDebugBuild) {
                switch (logType) {
                    case LogType.Error:
                        UnityDebug.LogError(_logForm);
                        return;
                    case LogType.Log:
                        UnityDebug.Log(_logForm);
                        return;
                    case LogType.Warning:
                        UnityDebug.LogWarning(_logForm);
                        return;
                }
            }
        }

        #endregion

        private class LogForm {
            public readonly ObjectForm ClassForm = new ObjectForm();
            public readonly ObjectForm TitleForm = new ObjectForm();
            public readonly ObjectForm MsgForm = new ObjectForm();

            public override string ToString() {
                return GetClassForm() + GetTitleForm() + GetMsgForm();
            }

            private string GetClassForm() {
                if (!ClassForm.IsShow) { return string.Empty; }

                return ClassForm.IsColor 
                    ? string.Format("<color=#{1}>[{0}]</color>: ", ClassForm.Msg, ClassForm.Color) 
                    : string.Format("[{0}]: ", ClassForm.Msg);
            }

            private string GetTitleForm() {
                if (!TitleForm.IsShow) { return string.Empty; }

                return TitleForm.IsColor 
                    ? string.Format("<color=#{1}>{0}</color> -> ", TitleForm.Msg, TitleForm.Color) 
                    : string.Format("{0} -> ", TitleForm.Msg);
            }

            private string GetMsgForm() {
                if (!MsgForm.IsShow) { return string.Empty; }

                return MsgForm.IsColor 
                    ? string.Format("<color=#{1}>{0}</color>", MsgForm.Msg, MsgForm.Color) 
                    : string.Format("{0}", MsgForm.Msg);
            }
        }

        private class ObjectForm {
            public bool IsShow {get; private set; }
            public object Msg  { get; private set; }
            public string Color  { get; private set; }
            public bool IsColor { get; private set; }

            public ObjectForm() {
                RemoveForm();
            }

            public void RemoveForm() {
                IsShow = false;
            }

            public void CreateForm(object msg) {
                Msg = msg;
                IsColor = false;
                IsShow = true;
            }

            public void CreateForm(object msg, Color32 color) {
                Msg = msg;
                Color = ColorToHex(color);
                IsColor = true;
                IsShow = true;
            }

            private string ColorToHex(Color32 color) {
                return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            }
        }
    }
}