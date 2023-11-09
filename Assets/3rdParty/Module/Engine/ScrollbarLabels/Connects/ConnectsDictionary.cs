using System;
using System.Collections.Generic;
using System.Linq;
using Module.Engine.ScrollbarLabels.Item;
using UnityEngine;

namespace Module.Engine.ScrollbarLabels {
    public static partial class Connect {
        public class Dictionary {
            public static void DataByGuid(
                IEnumerable<ItemScrollbarLabelsContentData> itemDataArray,
                out Dictionary<Guid, ItemScrollbarLabelsGroupData> dictionary)
            {
                dictionary = itemDataArray.ToDictionary(data => data.GroupData.Guid, data => data.GroupData);
            }

            public static void DataByGuid(
                IEnumerable<ItemScrollbarLabelsContentData> itemDataArray,
                out Dictionary<Guid, ItemScrollbarLabelsPartData> dictionary)
            {
                dictionary = itemDataArray.SelectMany(data => data.SubPartDataArray).ToDictionary(subData => subData.Guid);
            }

            public static void ControllerByData(
                IEnumerable<ItemScrollbarLabelsContentData> itemDataArray,
                GameObject componentPrefab, GameObject groupPrefab, GameObject partPrefab, Transform content,
                out Dictionary<ItemScrollbarLabelsContentData, ItemScrollbarLabelsSelectComponentView> contentDictionary,
                out Dictionary<ItemScrollbarLabelsGroupData, ItemScrollbarLabelsGroupView> groupDictionary,
                out Dictionary<ItemScrollbarLabelsPartData, ItemScrollbarLabelsPartView> partDictionary)
            {
                contentDictionary = new Dictionary<ItemScrollbarLabelsContentData, ItemScrollbarLabelsSelectComponentView>();
                groupDictionary = new Dictionary<ItemScrollbarLabelsGroupData, ItemScrollbarLabelsGroupView>();
                partDictionary = new Dictionary<ItemScrollbarLabelsPartData, ItemScrollbarLabelsPartView>();
                foreach (var data in itemDataArray) {
                    var component = Instantiate.Prefab<ItemScrollbarLabelsSelectComponentView>(componentPrefab, content);
                    component.Initialize(groupPrefab, partPrefab, data.SubPartDataArray.Length);
                    contentDictionary.Add(data, component);
                    groupDictionary.Add(data.GroupData, component.GroupView);
                    for (var index = 0; index < data.SubPartDataArray.Length; index++) {
                        partDictionary.Add(data.SubPartDataArray[index], component.PartViewArray[index]);
                    }
                }
            }
        }
    }
}