using System;
using System.Collections;
using UnityEngine;

public class RotationAR : MonoBehaviour
{
    public Transform model;
    public Transform label;
    public Camera camera;
    public Vector3 startRotation;
    
    public Action<bool> OnActive = delegate {  };
    public static bool Interactable = false;
    
    private void OnEnable()
    {
        FindRef();
    }

    private void FindRef()
    {
        model = transform.Find("model");
        label = transform.Find("3DLabel");
    }

    void Update()
    {
        if(!Interactable)
            return;
        
        if (model == null || label == null)
            FindRef();
        
        //if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved)
        if (Input.touchCount == 1  && Input.GetTouch(0).phase == TouchPhase.Moved) 
        {
            float rot = -Input.GetTouch(0).deltaPosition.x * 0.25f * Mathf.Deg2Rad;
            
            if(model)
                model.RotateAround(Vector3.up, rot);
            
            if(label)
                label.RotateAround(Vector3.up, rot);
        }
    }
    
    public void ResetRotation()
    {
        StartCoroutine(ResetAfterDelay());
    }
    
    private IEnumerator ResetAfterDelay()
    {
        yield return null;
        
        var mr = new Vector3(startRotation.x, camera.transform.rotation.eulerAngles.y + startRotation.y, startRotation.z);
        
        var rotation = Quaternion.Euler(mr);

        if (model)
            model.rotation = rotation;

        if (label)
            label.rotation = rotation;
    }
}

