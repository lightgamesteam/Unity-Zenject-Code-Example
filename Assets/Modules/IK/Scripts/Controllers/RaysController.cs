using UnityEngine;

namespace Module.IK
{

    public class RaysController
    {

        public static LineRenderer CreateRays(Transform parent)
        {
            var ray = Object.Instantiate(DataModel.LineRays.transform);
            ray.parent = DataModel.RaysObjectsTran;
            ray.name = parent.name;
            ray.position = parent.position;
            ray.localScale = Vector3.one;
            var lineRenderer = ray.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 3;
            return lineRenderer;
        }


        public static void UpdateRays(Transform parent)
        {
            var rayCenter = DataModel.RaysCenter[parent];
            var rayUp = DataModel.RaysUp[parent];
            var rayDown = DataModel.RaysDown[parent];
            var minVertices = DataModel.MinVertices[parent];
            var maxVertices = DataModel.MaxVertices[parent];
            var x = Mathf.Lerp(minVertices.x, maxVertices.x, 0.5f);
            var y = Mathf.Lerp(minVertices.y, maxVertices.y, 0.5f);
            var z = Mathf.Lerp(minVertices.z, maxVertices.z, 0.5f);
            var center = new Vector3(-x, -y, z);

            rayCenter.SetPosition(0, parent.localPosition);
            rayCenter.SetPosition(1, Vector3.zero);
            rayCenter.SetPosition(2, center);

            rayUp.SetPosition(0, parent.localPosition);
            rayUp.SetPosition(1, new Vector3(0, 4));
            rayUp.SetPosition(2, center);

            rayDown.SetPosition(0, parent.localPosition);
            rayDown.SetPosition(1, new Vector3(0, -4));
            rayDown.SetPosition(2, center);
        }


        public static void VisibilityRays(bool value)
        {
            DataModel.VisibilityRays = value;
            DataModel.RaysObjectsTran.gameObject.SetActive(value);
        }


        public static void RemoveAllLine()
        {
            var rays = DataModel.RaysObjectsTran.GetComponentsInChildren<LineRenderer>();
            foreach (var temp in rays)
            {
                Object.Destroy(temp.gameObject);
            }
        }
    }

}