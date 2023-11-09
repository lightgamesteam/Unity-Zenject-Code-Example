using System.Collections.Generic;
using UnityEngine;

namespace Module.Core.Data {
    public class UniversalDataObject {
        public string CultureCode;
        public string ActivityName;
        public List<UniversalModelData> Models;
    }

    public class UniversalModelData : ModelData {
        public int ModelId;
        public UniversalSpotData[] SpotItems;
    }

    public class UniversalSpotData {
        public string ModelName;
        public Vector3 Position;
        public Quaternion Rotation;
        public int[] ModelIds;
    }
}
