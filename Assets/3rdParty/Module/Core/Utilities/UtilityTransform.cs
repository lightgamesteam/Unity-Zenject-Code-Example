using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Module.Core {
    public static partial class Utilities {
        public static class Transform {
            public static Dictionary<UnityEngine.Transform, string[]> GetModifiedPartLabelsFromGameObject(IEnumerable<GameObject> source) {
                var result = new Dictionary<UnityEngine.Transform, string[]>();
                foreach (var gameObject in source) {
                    var transform = gameObject.GetComponent<UnityEngine.Transform>();
                    for (var i = 0; i < transform.childCount; i++) {
                        var partTransform = transform.GetChild(i);
                        var partName = partTransform.name;
                        var words = GetModifiedLabels(partName);
                        result.Add(partTransform, words);
                        GetSubPartLabelsFromTransform(partTransform, ref result);
                    }
                }
                return result;
            }

            public static string[] GetModifiedLabels(string value) {
                const string pattern = @"([A-Z0-9]+[a-z0-9]+)|([A-Z0-9]+)|([a-z0-9]+)";
                var words = Regex.Matches(value, pattern).OfType<Match>().Select(m => m.Value);
                return words.Select(word => word.ToLower()).ToArray();
            }
            
            private static void GetSubPartLabelsFromTransform(UnityEngine.Transform layer, ref Dictionary<UnityEngine.Transform, string[]> result) {
                for (var i = 0; i < layer.childCount; i++) {
                    var partTransform = layer.GetChild(i);
                    var partName = partTransform.name;
                    var words = GetModifiedLabels(partName);
                    result.Add(partTransform, words);
                }
            }
        }
    }
}