using System;
using System.Collections.Generic;
using Module.Engine.ScrollbarLabels.Item;

namespace Module.Engine.ScrollbarLabels {
    public static partial class Connect {
        public class Initialize {
            public static void Component(Dictionary<ItemScrollbarLabelsGroupData, ItemScrollbarLabelsGroupView> dictionary) {
                foreach (var pair in dictionary) {
                    pair.Value.Initialize(pair.Key.DisplayLabel);
                }
            }

            public static void Component(Dictionary<ItemScrollbarLabelsPartData, ItemScrollbarLabelsPartView> dictionary, Action<Guid> selectPartAction) {
                foreach (var pair in dictionary) {
                    pair.Value.Initialize(pair.Key.DisplayLabel, () => { selectPartAction.Invoke(pair.Key.Guid); });
                }
            }
        }
    }
}