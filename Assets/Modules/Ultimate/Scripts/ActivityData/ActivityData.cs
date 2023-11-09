using TDL.Server;
using UnityEngine;

namespace TDL.Modules.Ultimate.Core.ActivityData {
    public class GroupData {
        public LayerData LayerData;
        public LabelData[] LabelDataArray;
    }
    
    public class LayerData : DataBase {}
    
    public class LabelData : DataBase {
        public Color HighlightColor;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public AssociatedAsset[] LabelHotSpot;
        public int PartOrder;
    }

    public class DataBase {
        public int Id;
        public string GoName;
        public LocalName[] LocalNames;
    }
}