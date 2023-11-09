using System.Collections;
using System.Text;
using Module.DeveloperDebugging.Core;
using UnityEngine;

namespace Module.DeveloperDebugging.Components.PerformanceData.FPSCounter {
    public class ComponentFPSCounterController : ComponentControllerBase<ComponentFPSCounterView> {
        #region Variables

        protected int CurrentFps;
        protected FpsLevel CurrentFpsLevel;
        protected StringBuilder StringBuilder = new StringBuilder();

        protected string TagColorNormal => _tagColorNormalCached ?? (_tagColorNormalCached = string.Format(FPS_TEXT_START, Color32ToHex(ColorNormal)));
        protected string TagColorWarning => _tagColorWarningCached ?? (_tagColorWarningCached = string.Format(FPS_TEXT_START, Color32ToHex(ColorWarning)));
        protected string TagColorCritical => _tagColorCriticalCached ?? (_tagColorCriticalCached = string.Format(FPS_TEXT_START, Color32ToHex(ColorCritical)));

        private string _tagColorNormalCached;
        private string _tagColorWarningCached;
        private string _tagColorCriticalCached;

        protected readonly Color ColorNormal = new Color32(114, 153, 255, 255);
        protected readonly Color ColorWarning = new Color32(236, 224, 88, 255);
        protected readonly Color ColorCritical = new Color32(249, 91, 91, 255);
        protected const int WARNING_LEVEL_VALUE = 50;
        protected const int CRITICAL_LEVEL_VALUE = 20;
        protected const string COLOR_TEXT_START = "<color=#{0}>";
        protected const string COLOR_TEXT_END = "</color>";
        protected const string FPS_TEXT_START = COLOR_TEXT_START + "FPS: ";

        #endregion

        #region Public methods

        public IEnumerator UpdateCounter() {
            while (true) {
                var previousUpdateTime = Time.unscaledTime;
                var previousUpdateFrames = Time.frameCount;
                yield return new WaitForSeconds(0.5f);

                var timeElapsed = Time.unscaledTime - previousUpdateTime;
                var framesChanged = Time.frameCount - previousUpdateFrames;
                var currentValue = framesChanged / timeElapsed;
                UpdateValue((int)currentValue);
            }
        }

        #endregion

        #region Private methods

        private void UpdateValue(int currentFps) {
            if (CurrentFps == currentFps) { return; }

            CurrentFps = currentFps;
            UpdateFpsLevel(currentFps, ref CurrentFpsLevel);
            StringBuilder.Length = 0;
            StringBuilder.Append(GetTagColor()).Append(currentFps).Append(COLOR_TEXT_END);
            View.DisplayText.text = StringBuilder.ToString();
        }

        private string GetTagColor() {
            return CurrentFpsLevel == FpsLevel.Warning ? TagColorWarning 
                : CurrentFpsLevel == FpsLevel.Critical ? TagColorCritical : TagColorNormal;
        }

        private static void UpdateFpsLevel(int currentFps, ref FpsLevel currentFpsLevel) {
            if (currentFps <= CRITICAL_LEVEL_VALUE) {
                if (currentFps != 0 && currentFpsLevel != FpsLevel.Critical) {
                    currentFpsLevel = FpsLevel.Critical;
                }
            } else if (currentFps < WARNING_LEVEL_VALUE) {
                if (currentFps != 0 && currentFpsLevel != FpsLevel.Warning) {
                    currentFpsLevel = FpsLevel.Warning;
                }
            } else {
                if (currentFps != 0 && currentFpsLevel != FpsLevel.Normal) {
                    currentFpsLevel = FpsLevel.Normal;
                }
            }
        }

        private static string Color32ToHex(Color32 color) {
            return color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2") + color.a.ToString("x2");
        }

        #endregion
    }
}