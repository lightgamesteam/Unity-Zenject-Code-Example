using TMPro;
using UnityEngine.UI;

namespace Module.Core.UIContent {
    public class ContentText {
        protected readonly Text[] TextArray;
        protected readonly TextMeshProUGUI[] TextMeshProArray;
        protected readonly SelectableButtonTextEvent[] SelectableTextArray;

        public ContentText(params Text[] componentTexts) {
            CreateTextArray(ref TextArray, componentTexts);
            CreateTextArray(ref TextMeshProArray);
            CreateTextArray(ref SelectableTextArray);
        }

        public ContentText(params TextMeshProUGUI[] componentTexts) {
            CreateTextArray(ref TextArray);
            CreateTextArray(ref TextMeshProArray, componentTexts);
            CreateTextArray(ref SelectableTextArray);
        }

        public ContentText(params SelectableButtonTextEvent[] componentTexts) {
            CreateTextArray(ref TextArray);
            CreateTextArray(ref TextMeshProArray);
            CreateTextArray(ref SelectableTextArray, componentTexts);
        }

        public void SetText(string value) {
            foreach (var text in TextArray) {
                if (text != null) {
                    text.text = value;
                }
            }
            foreach (var text in TextMeshProArray) {
                if (text != null) {
                    text.text = value;
                }
            }
            foreach (var text in SelectableTextArray) {
                if (text != null) {
                    text.DisplayText = value;
                }
            }
        }

        public string GetText() {
            foreach (var text in TextArray) {
                if (text != null) {
                    return text.text;
                }
            }
            foreach (var text in TextMeshProArray) {
                if (text != null) {
                    return text.text;
                }
            }
            foreach (var text in SelectableTextArray) {
                if (text != null) {
                    return text.DisplayText;
                }
            }
            return string.Empty;
        }

        private static void CreateTextArray<T>(ref T[] array) {
            array = array ?? new T[0];
        }

        private static void CreateTextArray<T>(ref T[] array, params T[] value) {
            array = value ?? new T[0];
        }
    }
}