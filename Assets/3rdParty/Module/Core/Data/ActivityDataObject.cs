using System.Collections.Generic;

namespace Module.Core.Data {
    public class ActivityDataObject {
        public string CultureCode;
        public string ActivityName;
        public bool IsModelSpecific;
        public int NumberOfAttempts;
        public List<ModelData> Models;
    }
}
