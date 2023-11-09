using System;

namespace Module.Engine.ScrollbarLabels.Item {
    public class ItemScrollbarLabelsContentData {
        public readonly Guid Guid;
        public readonly ItemScrollbarLabelsGroupData GroupData;
        public readonly ItemScrollbarLabelsPartData[] SubPartDataArray;

        public ItemScrollbarLabelsContentData(Guid guid, ItemScrollbarLabelsGroupData groupData, ItemScrollbarLabelsPartData[] subPartDataArray) {
            Guid = guid;
            GroupData = groupData;
            SubPartDataArray = subPartDataArray;
        }
    }
}
