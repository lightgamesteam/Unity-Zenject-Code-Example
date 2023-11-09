using System;
using UnityEngine;

namespace Module.ManagerMaterials {
    [Serializable]
    public struct MaterialVariants {
        public Material Opaque;
        public Material Cutout;
        public Material Fade;
        public Material Transparent;
    }
}
