using System.Collections.Generic;
using UnityEngine;

public class TranslationAR : MonoBehaviour
{
    public Transform model;
    public Transform label;
    public Camera camera;
    private string floorName = "Floor";
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

    public void ActivateFloor(bool status)
    {
        transform.Find(floorName).gameObject.SetActive(status);
    }

    public void ResetPosition()
    {
        if(model)
            model.transform.localPosition = Vector3.zero;
        
        if(label)
            label.transform.localPosition = Vector3.zero;
    }

    void Update() 
    {
        if(!Interactable)
            return;
        
        if (model == null || label == null)
            FindRef();
        
        //if (Input.touchCount == 1  && Input.GetTouch(0).phase == TouchPhase.Moved) 
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            Vector2 middlePoint = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2;
                
            Ray ray = camera.ScreenPointToRay(middlePoint);
            
            List<RaycastHit> hits = new List<RaycastHit>();
            hits.AddRange(Physics.RaycastAll(ray, 1000f));

            RaycastHit hit = hits.Find(item => item.collider.name.Equals(floorName));

            if (hit.collider != null)
            {
                if(model)
                    model.transform.position = hit.point;
                
                if(label)
                    label.transform.position = hit.point;
            }
        }
    }
}
