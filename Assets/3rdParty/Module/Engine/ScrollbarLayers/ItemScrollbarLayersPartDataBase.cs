using System;

namespace Module.Engine.ScrollbarLayers.Item {
    public abstract class ItemScrollbarLayersPartDataBase {
        public readonly Guid Guid;
        public readonly string DisplayLabel;
        public StateType StateType { get; private set; }
        public LightStateType LightStateType { get; private set; }

        protected ItemScrollbarLayersPartDataBase(Guid guid, string displayLabel, StateType stateType) {
            Guid = guid;
            DisplayLabel = displayLabel;
            SetState(stateType);
            SetLightState(LightStateType.Disable);
        }

        public void SetState(StateType stateType) {
            StateType = stateType;
        }

        public void SetLightState(LightStateType lightStateType) {
            LightStateType = lightStateType;
        }
    }
}
