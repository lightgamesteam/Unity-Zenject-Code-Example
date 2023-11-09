using System;
using System.Collections.Generic;
using System.Linq;
using Module.Engine.ScrollbarLanguages.Item;
using UnityEngine;

namespace Module.Engine.ScrollbarLanguages {
    public static partial class Connect {
        public class Dictionary {
            public static void DataByGuid(
                IEnumerable<ItemScrollbarLanguagesData> itemDataArray,
                out Dictionary<int, ItemScrollbarLanguagesData> dictionary)
            {
                dictionary = itemDataArray.ToDictionary(data => data.Id, data => data);
            }

            public static void ControllerByData(
                IEnumerable<ItemScrollbarLanguagesData> itemDataArray,
                GameObject prefab, Transform content,
                out Dictionary<ItemScrollbarLanguagesData, Item.ItemScrollbarLanguagesView> dictionary)
            {
                dictionary = new Dictionary<ItemScrollbarLanguagesData, Item.ItemScrollbarLanguagesView>();
                foreach (var data in itemDataArray) {
                    var view = Instantiate.Prefab<Item.ItemScrollbarLanguagesView>(prefab, content);
                    dictionary.Add(data, view);
                }
            }
        }
    }
}