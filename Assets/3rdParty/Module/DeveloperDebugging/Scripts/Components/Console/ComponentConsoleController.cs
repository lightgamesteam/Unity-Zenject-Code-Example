using System.Text;
using Module.DeveloperDebugging.Core;

namespace Module.DeveloperDebugging.Components.Console {
    public class ComponentConsoleController : ComponentControllerBase<ComponentConsoleView> {
        #region Variables

        protected StringBuilder StringBuilder = new StringBuilder();

        #endregion

        #region Public methods

        public void Clear() {
            StringBuilder.Clear();
            RefreshConsoleView();
        }

        public void Log(string value) {
            StringBuilder.AppendLine(value);
            RefreshConsoleView();
        }

        #endregion

        #region Protected methods

        protected override void Initialize() {
            base.Initialize();
            View.ClearButton.onClick.AddListener(Clear);
        }

        protected override void Release() {
            View.ClearButton.onClick.RemoveListener(Clear);
            base.Release();
        }

        #endregion

        #region Private methods

        private void RefreshConsoleView() {
            View.ConsoleInputField.text = StringBuilder.ToString();
        }

        #endregion
    }
}