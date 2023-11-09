using System;

namespace Module.Engine.ScrollbarLayers.Item {
    public class ItemScrollbarLayersPartData : ItemScrollbarLayersPartDataBase {
        public EyeStateType EyeStateType { get; private set; }

        public ItemScrollbarLayersPartData(Guid guid, string displayLabel, StateType stateType) : base(guid, displayLabel, stateType) {
            SetEyeState(EyeStateType.None);
        }

        public void SetEyeState(EyeStateType stateType) {
            EyeStateType = stateType;
        }
    }
}
