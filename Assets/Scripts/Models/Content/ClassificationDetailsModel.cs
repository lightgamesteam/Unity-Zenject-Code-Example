using System;
using System.Collections.Generic;
using TDL.Server;

namespace TDL.Models
{
    public class ClassificationDetailsModel
    {
        public Action OnClassificationDetailsModelUpdated;

        public int ClassificationId { get; private set; }
        public string ClassificationName { get; private set; }
        public List<Classification.PropertyItem> Properties { get; private set; }
        public LocalName[] ClassificationLocal { get; set; }

        public void Update(ClassificationDetailsModelStruct modelStruct)
        {
            ClassificationId = modelStruct.ClassificationId;
            ClassificationName = modelStruct.ClassificationName;
            Properties = modelStruct.Properties;
            ClassificationLocal = modelStruct.ClassificationLocal;

            NotifyUpdated();
        }

        private void NotifyUpdated()
        {
            OnClassificationDetailsModelUpdated?.Invoke();
        }

        public class ClassificationDetailsModelStruct
        {
            public int ClassificationId;
            public string ClassificationName;
            public List<Classification.PropertyItem> Properties;
            public LocalName[] ClassificationLocal { get; set; }
        }
    }
}