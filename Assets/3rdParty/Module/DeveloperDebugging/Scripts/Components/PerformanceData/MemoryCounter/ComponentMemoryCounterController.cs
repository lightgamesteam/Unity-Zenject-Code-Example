using System.Collections;
using System.Text;
using Module.DeveloperDebugging.Core;
using UnityEngine;
using UnityEngine.Profiling;

namespace Module.DeveloperDebugging.Components.PerformanceData.MemoryCounter {
    public class ComponentMemoryCounterController : ComponentControllerBase<ComponentMemoryCounterView> {
        #region Variables

        protected long LastTotalValue;
        protected long LastAllocatedValue;
        protected long LastMonoValue;
        protected StringBuilder StringBuilder = new StringBuilder();

        protected const long MEMORY_DIVIDER = 1048576; // 1024^2
        protected const string LINE_START_TOTAL = "MEM TOTAL: ";
        protected const string LINE_START_ALLOCATED = "  MEM ALLOC: ";
        protected const string LINE_START_MONO = "  MEM MONO: ";
        protected const string LINE_END = " MB";

        #endregion

        #region Public methods

        public IEnumerator UpdateCounter() {
            while (true) {
                UpdateValue();
                yield return new WaitForSeconds(0.5f);
            }
        }

        #endregion

        #region Private methods

        private void UpdateValue() {
            UpdateValue(Profiler.GetTotalReservedMemoryLong(), ref LastTotalValue, View.IsPrecise);
            UpdateValue(Profiler.GetTotalAllocatedMemoryLong(), ref LastAllocatedValue, View.IsPrecise);
            UpdateValue(System.GC.GetTotalMemory(false), ref LastMonoValue, View.IsPrecise);

            StringBuilder.Length = 0;
            DisplayValue(ref StringBuilder, LINE_START_TOTAL, LastTotalValue, View.IsPrecise);
            DisplayValue(ref StringBuilder, LINE_START_ALLOCATED, LastAllocatedValue, View.IsPrecise);
            DisplayValue(ref StringBuilder, LINE_START_MONO, LastMonoValue, View.IsPrecise);
            View.DisplayText.text = StringBuilder.ToString();
		}

        private static void UpdateValue(long value, ref long lastValue, bool isPrecise) {
            long divisionResult = 0;
            bool newValue;
            if (isPrecise) {
                newValue = lastValue != value;
            } else {
                divisionResult = value / MEMORY_DIVIDER;
                newValue = lastValue != divisionResult;
            }
            if (newValue) {
                lastValue = isPrecise ? value : divisionResult;
            }
        }

        private static void DisplayValue(ref StringBuilder stringBuilder, string name, long value, bool isPrecise) {
            stringBuilder.Append(name);
            if (isPrecise) {
                stringBuilder.Append((value / (float)MEMORY_DIVIDER).ToString("F"));
            } else {
                stringBuilder.Append(value);
            }
            stringBuilder.Append(LINE_END);
        }

        #endregion
    }
}