using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Module.IK
{

    public class SceneController
    {

        public static void ScleanChildObjects(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Object.Destroy(child.gameObject);
            }
        }


        private static void CleanDictionarys()
        {
            DataModel.MirrorModels.Clear();
            DataModel.Vertexs.Clear();
            DataModel.DisplacedVertices.Clear();
            DataModel.DisplacedMirrorVertices.Clear();
            DataModel.BoundingBoxOriginalVertices.Clear();
            DataModel.BoundingBoxDisplacedVertices.Clear();
            DataModel.BoundingBox.Clear();
            DataModel.ListPoints.Clear();
        }
    }

}