using UnityEngine;

public class ControllerAR : MonoBehaviour
{
    private TranslationAR _translationAR;
    private RotationAR _rotationAr;
    private ScaleAR _scaleAr;

    private Camera _camera;
    private ControllerAR _secondController;
    public void GetReference()
    {
        _translationAR = GetComponent<TranslationAR>();
        _rotationAr = GetComponent<RotationAR>();
        _scaleAr = GetComponent<ScaleAR>();

        _camera = _translationAR.camera;

        gameObject.GetAllInScene<ControllerAR>().ForEach(c =>
        {
            if (c != this)
                _secondController = c;
        });
    }

    public float GetDistance()
    {
        if (_camera == null)
            return float.PositiveInfinity;
        
        return Vector3.Dot(_camera.transform.forward, (_translationAR.model.position - _camera.transform.position).normalized);
    }

    void Update()
    {
        if(_camera == null || _secondController == null)
            GetReference();
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (GetDistance() < _secondController.GetDistance())
            {
                _translationAR.enabled = false;
                _rotationAr.enabled = false;
                _scaleAr.enabled = false;
            }
            else
            {
                _translationAR.enabled = true;
                _rotationAr.enabled = true;
                _scaleAr.enabled = true;
            }
        }
        
        if (GetDistance() < _secondController.GetDistance())
        {
            _rotationAr.OnActive.Invoke(false);
        }
        else
        {
            _rotationAr.OnActive.Invoke(true);
        }
    }
}
