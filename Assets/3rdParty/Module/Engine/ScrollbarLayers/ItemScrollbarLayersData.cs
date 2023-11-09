using System;

namespace Module.Engine.ScrollbarLayers.Item {
    public class ItemScrollbarLayersData {
        public readonly Guid Guid;
        public readonly ItemScrollbarLayersPartData PartData;
        public readonly ItemScrollbarLayersPartData[] SubPartDataArray;

        public ItemScrollbarLayersData(Guid guid, ItemScrollbarLayersPartData partData, ItemScrollbarLayersPartData[] subPartDataArray) {
            Guid = guid;
            PartData = partData;
            SubPartDataArray = subPartDataArray;
        }
    }
}
