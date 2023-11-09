using System;

namespace Module.Engine.ScrollbarLabels.Item {
    public abstract class ItemScrollbarLabelsStateDataBase {
        public readonly Guid Guid;
        public readonly string DisplayLabel;
        public StateType StateType { get; private set; }
        public LightStateType LightStateType { get; private set; }

        protected ItemScrollbarLabelsStateDataBase(Guid guid, string displayLabel, StateType stateType) {
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
