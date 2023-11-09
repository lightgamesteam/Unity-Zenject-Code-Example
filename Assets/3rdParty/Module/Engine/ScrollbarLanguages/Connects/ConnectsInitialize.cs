using System;
using System.Collections.Generic;
using Module.Engine.ScrollbarLanguages.Item;

namespace Module.Engine.ScrollbarLanguages {
    public static partial class Connect {
        public class Initialize {
            public static void Component(Dictionary<ItemScrollbarLanguagesData, Item.ItemScrollbarLanguagesView> dictionary, Action<int> selectPartAction) {
                foreach (var pair in dictionary) {
                    pair.Value.Initialize(pair.Key.DisplayLabel, () => { selectPartAction.Invoke(pair.Key.Id); });
                }
            }
        }
    }
}