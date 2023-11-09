using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothOrbitCamViewportInput : MonoBehaviour
{
    public SmoothOrbitCam smoothOrbitCam;
    public Camera cameraViewport;
    
    void Awake()
    {
        if (smoothOrbitCam == null)
            smoothOrbitCam = GetComponent<SmoothOrbitCam>();

        if (cameraViewport == null)
            cameraViewport = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)|| Input.GetMouseButtonDown(1) || Input.mouseScrollDelta.sqrMagnitude > 0)
            ViewportInput();
    }
    
    private void ViewportInput()
    {
        Camera cMouse = MouseExtension.GetDepthCameraForMousePosition(5);

        if (cameraViewport != null && cMouse != null && smoothOrbitCam.interactable)
        {
            if (cMouse.Equals(cameraViewport))
            {
                smoothOrbitCam.enabled = true;
            }
            else
            {
                smoothOrbitCam.enabled = false;
            }
        }
    }
}
