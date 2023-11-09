using System;

namespace Module.Engine.ScrollbarLanguages.Item {
    public class ItemScrollbarLanguagesData {
        public readonly int Id;
        public readonly string DisplayLabel;
        public ItemStateType ItemStateType { get; private set; }
        public SelectStateType SelectStateType { get; private set; }

        public ItemScrollbarLanguagesData(int id, string displayLabel, ItemStateType itemStateType, SelectStateType selectStateType) {
            Id = id;
            DisplayLabel = displayLabel;
            SetState(itemStateType);
            SetLightState(selectStateType);
        }

        public void SetState(ItemStateType itemStateType) {
            ItemStateType = itemStateType;
        }

        public void SetLightState(SelectStateType selectStateType) {
            SelectStateType = selectStateType;
        }
    }
}
