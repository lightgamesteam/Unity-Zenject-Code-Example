using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.IK
{

    public class ApplicationView : MonoBehaviour
    {
        public Camera MainCamera;
        public SmoothOrbitCam SmoothOrbitCam;
        public GameObject TargetIK;
        public GameObject ElbowIK;
        

        [Header("UI")]
        public RectTransform PanelUI;
        public Canvas Canvas;


//        public void SetDefaultSettings() // Used in UI
//        {
//        }
//
//
//        public void SetColorPicker(Color value) // Used in UI
//        {
//            DataModel.MainCamera.backgroundColor = value;
//        }


        private void Start()
        {
            DataModel.MonoBehaviour = this;
            //DataModel.MainCamera = MainCamera;
            //DataModel.SmoothOrbitCam[9] = SmoothOrbitCam;
            DataModel.PanelUI = PanelUI;
            DataModel.Canvas = Canvas;
            DataModel.TargetIK = TargetIK;
            DataModel.ElbowIK = ElbowIK;

            DataModel.DefaultCameraPosition = MainCamera.transform.position;
            DataModel.DefaultCameraRotation = MainCamera.transform.rotation;

            DataModel.ListPoints = new List<Transform>();
            DataModel.MirrorModels = new Dictionary<GameObject, GameObject>();
            DataModel.Vertexs = new Dictionary<Transform, Dictionary<string, Transform>>();
            DataModel.OriginalVertices = new Dictionary<Transform, Vector3[]>();
            DataModel.DisplacedVertices = new Dictionary<Transform, Vector3[]>();
            DataModel.DisplacedMirrorVertices = new Dictionary<Transform, Vector3[]>();
            DataModel.BoundingBoxOriginalVertices = new Dictionary<Transform, Vector3[]>();
            DataModel.BoundingBoxDisplacedVertices = new Dictionary<Transform, Vector3[]>();
            DataModel.BoundingBox = new Dictionary<Transform, Mesh>();
            DataModel.RaysCenter = new Dictionary<Transform, LineRenderer>();
            DataModel.RaysUp = new Dictionary<Transform, LineRenderer>();
            DataModel.RaysDown = new Dictionary<Transform, LineRenderer>();
            DataModel.MinVertices = new Dictionary<Transform, Vector3>();
            DataModel.MaxVertices = new Dictionary<Transform, Vector3>();

            //SetDefaultSettings();
        }


        private void Update()
        {
            //CameraController.SetDefaultSettings(9);
        }

    }

}