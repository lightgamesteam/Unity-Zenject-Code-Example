using System.Collections;
using UnityEngine;

namespace Module.IK
{

    public class CameraController
    {

        public static void SetEnable(bool value, int layer)
        {
            if (DataModel.SmoothOrbitCam.ContainsKey(layer))
                DataModel.SmoothOrbitCam[layer].enabled = value;

            if (DataModel.SmoothOrbitCamViewportInput.ContainsKey(layer))
                DataModel.SmoothOrbitCamViewportInput[layer].enabled = value;
        }


//        public static void SetTarget(Transform tr)
//        {
//            DataModel.SmoothOrbitCam[tr.gameObject.layer].target = tr;
//        }


//        public static bool CompareTarget(Transform tr)
//        {
//            return DataModel.SmoothOrbitCam[tr.gameObject.layer].target == tr;
//        }


//        public static void AbilityToMoveCamera()
//        {
//            if (UIController.IsPointerOverUIObject() && DataModel.SmoothOrbitCam.ySpeed != 0)
//            {
//                DataModel.SmoothOrbitCam.ySpeed = 0;
//                DataModel.SmoothOrbitCam.xSpeed = 0;
//            }
//            else if (!UIController.IsPointerOverUIObject() && DataModel.SmoothOrbitCam.ySpeed != 2)
//            {
//                DataModel.SmoothOrbitCam.ySpeed = 2;
//                DataModel.SmoothOrbitCam.xSpeed = 2;
//            }
//        }
//
//
//        public static void DontMoveCamera()
//        {
//            DataModel.CurentTimeLimitToMoveCamera = 0.2f;
//            if (DataModel.IsMoveCamera)
//            {
//                DataModel.MonoBehaviour.StartCoroutine(StartTimeToDontMoveCamera());
//            }
//        }


        public static void SetDefaultSettings(int layer)
        {
            if (DataModel.SmoothOrbitCam[layer].xMaxLimit != 0
                    || DataModel.SmoothOrbitCam[layer].yMaxLimit != 0
                    || DataModel.SmoothOrbitCam[layer].xMinLimit != 0
                    || DataModel.SmoothOrbitCam[layer].yMinLimit != 0)
            {
                DataModel.SmoothOrbitCam[layer].xMaxLimit = 0;
                DataModel.SmoothOrbitCam[layer].yMaxLimit = 0;
                DataModel.SmoothOrbitCam[layer].xMinLimit = 0;
                DataModel.SmoothOrbitCam[layer].yMinLimit = 0;
            }
        }


//        public static void SetDefaultTarget()
//        {
//            SetTarget(DataModel.Lens);
//        }


//        public static void SetDefaultPosition(int layer)
//        {
//            DataModel.SmoothOrbitCam[layer].transform.SetPositionAndRotation(DataModel.DefaultCameraPosition, DataModel.DefaultCameraRotation);
//            DataModel.SmoothOrbitCam[layer].ResetValues();
//        }


//        private static IEnumerator StartTimeToDontMoveCamera(int layer)
//        {
//            DataModel.IsMoveCamera = false;
//            SetEnable(false, layer);
//
//            while (DataModel.CurentTimeLimitToMoveCamera > 0)
//            {
//                var tempTime = 0.05f;
//                DataModel.CurentTimeLimitToMoveCamera -= tempTime;
//                yield return new WaitForSeconds(tempTime);
//            }
//
//            SetEnable(true, layer);
//            DataModel.IsMoveCamera = true;
//        }


    }

}