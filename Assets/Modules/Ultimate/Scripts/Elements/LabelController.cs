using Module.Core.Model;
using TDL.Modules.Ultimate.Core.ActivityData;
using UnityEngine;

namespace TDL.Modules.Ultimate.Core.Elements {
    public class LabelController : ModelPartControllerBase {
        public readonly DataBase Data;

        public LabelController(Transform transform, DataBase dataBase) : base(new ElementView(transform)) {
            Data = dataBase;
            if (dataBase is ActivityData.LabelData labelDataBase) {
                View.GameObject.GetComponent<ItemElementLabelController>().Initialize(labelDataBase);
            }
        }
    }
}