using System.Collections.Generic;

namespace TDL.Server
{
    public class Classification
    {
        public class ClassificationResponse : ResponseBase
        {
            public ClassificationItem Classification { get; set; }
        }

        public class ClassificationItem
        {
            public int ClassificationId { get; set; }
            public string ClassificationName { get; set; }
            public List<PropertyItem> Properties { get; set; } = new List<PropertyItem>();
            public List<ActivityDetail> ClassificationDetails { get; set; } = new List<ActivityDetail>();
            public LocalName[] ClassificationLocal { get; set; }
        }

        public class PropertyItem
        {
            public int PropertyId { get; set; }
            public string PropertyName { get; set; }
            public List<ModelItem> ModelDetails { get; set; } = new List<ModelItem>();
            public LocalName[] PropertyLocal { get; set; }
        }

        public class ModelItem
        {
            public int AssetId { get; set; }
            public string AssetName { get; set; }
        }
    }
}