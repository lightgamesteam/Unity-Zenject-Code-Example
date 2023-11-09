using UnityEngine;

namespace Module.IK
{

    public class DistanceController
    {

        public static void CreateDistanceText(Transform parent)
        {
            var prefab = (TextMesh)Resources.Load("DistanceMirrorObject", typeof(TextMesh));
            DataModel.DistanceMirrorObject = Object.Instantiate(prefab);
            DataModel.DistanceSourceObject = Object.Instantiate(prefab);
            DataModel.DistanceMirrorObject.transform.parent = parent;
            DataModel.DistanceSourceObject.transform.parent = parent;
        }


        public static void UpdateDistance(Transform parent)
        {
            var rayCenter = DataModel.RaysCenter[parent];
            var minVertices = DataModel.MinVertices[parent];
            var maxVertices = DataModel.MaxVertices[parent];
            var x = Mathf.Lerp(minVertices.x, maxVertices.x, 0.5f);
            var y = Mathf.Lerp(minVertices.y, maxVertices.y, 0.5f);
            var z = Mathf.Lerp(minVertices.z, maxVertices.z, 0.5f);
            var center = new Vector3(-x, -y, z);

            DataModel.DistanceMirrorObject.text = Vector3.Distance(center, Vector3.zero).ToString("F2") + "mm";
            DataModel.DistanceSourceObject.text = Vector3.Distance(parent.position, Vector3.zero).ToString("F2") + "mm";

            DataModel.DistanceMirrorObject.transform.position = center - (center / 2f);
            DataModel.DistanceSourceObject.transform.position = parent.localPosition - (parent.localPosition / 2f);
        }


        public static void VisibilityDistances(bool value)
        {
            DataModel.DistanceMirrorObject.gameObject.SetActive(value);
            DataModel.DistanceSourceObject.gameObject.SetActive(value);
        }

    }

}