using System;

namespace Module.Engine.ScrollbarLabels.Item {
    public class ItemScrollbarLabelsGroupData : ItemScrollbarLabelsStateDataBase {
        public readonly bool IsSelectable;

        public ItemScrollbarLabelsGroupData(Guid guid, string displayLabel, StateType stateType, bool isSelectable) : base(guid, displayLabel, stateType) {
            IsSelectable = isSelectable;
        }
    }
}
