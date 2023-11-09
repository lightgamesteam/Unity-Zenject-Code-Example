using System;
using System.Collections.Generic;

namespace Module.Core.Data {
    [Serializable]
    public class ModelData {
        public string FileHash;
        public string ModelName;
        public List<ModelPartData> ModelParts;
    }
}
