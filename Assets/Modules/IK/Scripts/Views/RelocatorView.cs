using System;
using System.Collections;
using UnityEngine;

namespace Module.IK
{
    public class RelocatorView : MonoBehaviour
    {
        public static Action<bool, int> IsRelocatorActivatedAction = delegate {  };
        
        public bool isInteractable = true;
        private bool _isSelected = false;
        private Color _color;
        private Vector3 _startPosition;
        private Vector3 _defaultPosition;
        private Camera _camera => DataModel.VuforiaCamera ? DataModel.VuforiaCamera :  DataModel.MainCamera[gameObject.layer];
        [HideInInspector]
        public IKView _IKView;

        private void Start()
        {
            _defaultPosition = transform.position;
            
            if (gameObject.GetComponent<MeshRenderer>().material.HasProperty("_Color"))
            {
                _color = gameObject.GetComponent<MeshRenderer>().material.color;
            }

            StartCoroutine(StartCheckToMove());
        }

        public void ResetPosition()
        {
            transform.position = _defaultPosition;
            IsRelocatorActivatedAction?.Invoke(false, gameObject.layer);
        }
        
        public void OnMouseDownIK()
        {
            if (UIController.IsPointerOverUIObject() && _isSelected)
                return;
            
            _isSelected = true;
            
            CameraController.SetEnable(false, gameObject.layer);
            var pos = Input.mousePosition;
            
            pos.z = _camera.WorldToScreenPoint(transform.position).z;
            pos = _camera.ScreenToWorldPoint(pos);
            
            _startPosition = pos - transform.position;
            _IKView.SetEnableLineIK(true);
            
            // TODO : Called 2 times
            IsRelocatorActivatedAction?.Invoke(true, gameObject.layer);
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && isInteractable)
            {
                OnMouseUpIK();
            }
        }
        
        private void OnMouseUpIK()
        {
            if(!_isSelected)
                return;
            
            if(isInteractable)
                CameraController.SetEnable(true, gameObject.layer);
            
            _isSelected = false;

            _startPosition = Vector3.zero;
            _IKView.SetEnableLineIK(false);
            _IKView.UpdateIKPosition();

            IsRelocatorActivatedAction?.Invoke(false, gameObject.layer);
        }
        
//        private void OnMouseDown()
//        {
//            if (!UIController.IsPointerOverUIObject())
//            {
//                CameraController.SetEnable(_isSelected);
//                _isSelected = true;
//
//                var pos = Input.mousePosition;
//                pos.z = DataModel.MainCamera.WorldToScreenPoint(transform.position).z;
//                pos = DataModel.MainCamera.ScreenToWorldPoint(pos);
//                _startPosition = pos - transform.position;
//                _IKView.SetEnableLineIK(true);
//            }
//        }

//        private void OnMouseUp()
//        {
//            CameraController.SetEnable(_isSelected);
//            _isSelected = false;
//            _startPosition = Vector3.zero;
//            _IKView.SetEnableLineIK(false);
//            _IKView.UpdateIKPosition();
//        }


        private void OnMouseEnter()
        {
            if (!UIController.IsPointerOverUIObject()  && isInteractable)
            {
                if (_color != Color.black)
                {
                    SelectedController.SetColor(gameObject, _color + new Color(0.2f, 0.2f, 0.2f, 0.2f));
                }
                else
                {
                    SelectedController.SetColor(gameObject,  new Color(0.6f, 0.6f, 0.6f, 1f));
                }
            }
        }


        private void OnMouseExit()
        {
            SelectedController.SetColor(gameObject, _color);
        }


        private void FixedUpdate()
        {
            //CheckToMove();
        }


        IEnumerator StartCheckToMove()
        {
            while (true)
            {
                CheckToMove();
                yield return new WaitForSeconds(0.01f);
            }
        }


        private void CheckToMove()
        {
            if (_isSelected && Input.GetMouseButton(0)  && isInteractable)
            {
                var pos = Input.mousePosition;
                pos.z = _camera.WorldToScreenPoint(transform.position).z;
                pos = _camera.ScreenToWorldPoint(pos);
                transform.position = pos - _startPosition;

                _IKView.UpdateIKPosition(transform);

                if (DataModel.ElbowIK != null)
                {
                    var posT = transform.position;
                    DataModel.ElbowIK.transform.position = new Vector3(-posT.x, -posT.y, -posT.z);
                }
            }
        }

    }

}