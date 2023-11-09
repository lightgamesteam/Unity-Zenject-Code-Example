using System;

namespace Module.Engine.ScrollbarLabels.Item {
    public class ItemScrollbarLabelsPartData : ItemScrollbarLabelsPartDataBase {
        public EyeStateType EyeStateType { get; private set; }

        public ItemScrollbarLabelsPartData(Guid guid, string displayLabel, StateType stateType) : base(guid, displayLabel, stateType) {
            SetEyeState(EyeStateType.None);
        }

        public void SetEyeState(EyeStateType stateType) {
            EyeStateType = stateType;
        }
    }
}
