using TDL.Modules.Ultimate.Core.ActivityData;

namespace TDL.Modules.Ultimate.Core.Elements {
    public class LayerGroupController {
        public readonly GroupData Data;
        public readonly ModelController LayerModelController;
        public readonly ModelController[] LabelModelControllers;

        public LayerGroupController(GroupData data, ModelController layerModelController, ModelController[] labelModelControllers) {
            Data = data;
            LayerModelController = layerModelController;
            LabelModelControllers = labelModelControllers;
        }
    }
}
