using TDL.Modules.Ultimate.Core.ActivityData;

namespace TDL.Modules.Ultimate.Core.Elements {
    public class LabelGroupController {
        public readonly GroupData Data;
        public readonly LabelController LayerModelController;
        public readonly LabelController[] LabelModelControllers;

        public LabelGroupController(GroupData data, LabelController layerModelController, LabelController[] labelModelControllers) {
            Data = data;
            LayerModelController = layerModelController;
            LabelModelControllers = labelModelControllers;
        }
    }
}
