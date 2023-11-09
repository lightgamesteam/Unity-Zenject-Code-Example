using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MouseExtension : MonoBehaviour
{
   public static Camera GetCameraForMousePosition() 
   {
       Camera[] allCameras = FindObjectsOfType<Camera>();
       
       foreach (Camera camera in allCameras) {
           
           if(!camera.gameObject.activeSelf)
               continue;
               
           Vector3 point = camera.ScreenToViewportPoint(Input.mousePosition);
           
           if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1) {
               return camera;
           }
       }
       return null;
   }
   
   public static Camera GetCameraForTouchPosition(Touch touch) 
   {
       Camera[] allCameras = FindObjectsOfType<Camera>();
       
       foreach (Camera camera in allCameras) {
           
           if(!camera.gameObject.activeSelf)
               continue;
               
           Vector3 point = camera.ScreenToViewportPoint(touch.position);
           
           if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1) {
               return camera;
           }
       }
       return null;
   }
   
   public static Camera GetDepthCameraForMousePosition(Scene scene) 
   {
       Camera[] allCameras = FindObjectsOfType<Camera>();
       List<Camera> allCams = new List<Camera>();
       
       foreach (Camera camera in allCameras) {
           
           if(!camera.gameObject.activeSelf || camera.gameObject.scene != scene)
               continue;
               
           Vector3 point = camera.ScreenToViewportPoint(Input.mousePosition);
           
           if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1) {
               allCams.Add(camera);
           }
       }

       Camera c;
       
       if(allCams.Count > 0)
           c = allCams[0];
       else
           c = null;
       
       foreach (Camera cam in allCams)
       {
           if (cam.depth > c.depth)
           {
               c = cam;
           }
       }
       
       return c;
   }

   public static Camera GetDepthCameraForMousePosition() 
   {
       Camera[] allCameras = FindObjectsOfType<Camera>();
       List<Camera> allCams = new List<Camera>();
       
       foreach (Camera camera in allCameras) {
           
           if(!camera.gameObject.activeSelf)
               continue;
               
           Vector3 point = camera.ScreenToViewportPoint(Input.mousePosition);
           
           if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1) {
               allCams.Add(camera);
           }
       }

       Camera c;
       
       if(allCams.Count > 0)
            c = allCams[0];
       else
           c = null;
       
       foreach (Camera cam in allCams)
       {
           if (cam.depth > c.depth)
           {
               c = cam;
           }
       }
       
       return c;
   }
   
   public static Camera GetDepthCameraForMousePosition(Camera[] cameras) 
   {
       List<Camera> allCams = new List<Camera>();
       
       foreach (Camera camera in cameras) 
       {
           if(!camera.gameObject.activeSelf)
               continue;
               
           Vector3 point = camera.ScreenToViewportPoint(Input.mousePosition);
           
           if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1) 
           {
               allCams.Add(camera);
           }
       }

       Camera c;
       
       if(allCams.Count > 0)
           c = allCams[0];
       else
           c = null;
       
       foreach (Camera cam in allCams)
       {
           if (cam.depth > c.depth)
           {
               c = cam;
           }
       }
       
       return c;
   }
   
   public static Camera GetDepthCameraForMousePosition(int excludeLayer) 
   {
       Camera[] allCameras = FindObjectsOfType<Camera>();
       List<Camera> allCams = new List<Camera>();
       
       foreach (Camera camera in allCameras) {
           
           if(!camera.gameObject.activeSelf)
               continue;
               
           Vector3 point = camera.ScreenToViewportPoint(Input.mousePosition);
           
           if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1) {
               allCams.Add(camera);
           }
       }

       allCams.RemoveAll(ac => ac.gameObject.layer == excludeLayer);

       Camera c;
       
       if(allCams.Count > 0)
           c = allCams[0];
       else
           c = null;
       
       foreach (Camera cam in allCams)
       {
           if (cam.depth > c.depth)
           {
               c = cam;
           }
       }
       
       return c;
   }
   
   public static Camera GetDepthCameraForTouchPosition(Touch touch) 
   {
       Camera[] allCameras = FindObjectsOfType<Camera>();
       List<Camera> allCams = new List<Camera>();
       
       foreach (Camera camera in allCameras) {
           
           if(!camera.gameObject.activeSelf)
               continue;
               
           Vector3 point = camera.ScreenToViewportPoint(touch.position);
           
           if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1) {
               allCams.Add(camera);
           }
       }

       Camera c;
       
       if(allCams.Count > 0)
           c = allCams[0];
       else
           c = null;
       
       foreach (Camera cam in allCams)
       {
           if (cam.depth > c.depth)
           {
               c = cam;
           }
       }
       
       return c;
   }
   
   public static Vector3 GetMousePositionViewport()
   {
       Camera camera = GetCameraForMousePosition();
            
       Vector3 viewportPoint = Vector3.zero;
       Vector2 viewportSize = Vector2.zero;

       if (Input.mousePosition.x > 0 && Input.mousePosition.y > 0)
       {
           viewportPoint = camera.ScreenToViewportPoint(Input.mousePosition);

           viewportSize = new Vector2(Screen.width * camera.rect.width,
                                      Screen.height * camera.rect.height);
       }
            
       Vector3 sc = new Vector3(
           viewportSize.x * viewportPoint.x,
           viewportSize.y * viewportPoint.y,
           0);
            
       return sc;
   }
   
   public static Vector3 GetTouchPositionViewport(Touch touch)
   {
       Camera camera = GetCameraForTouchPosition(touch);
            
       Vector3 viewportPoint = Vector3.zero;
       Vector2 viewportSize = Vector2.zero;

       if (touch.position.x > 0 && touch.position.y > 0)
       {
           viewportPoint = camera.ScreenToViewportPoint(touch.position);

           viewportSize = new Vector2(Screen.width * camera.rect.width,
               Screen.height * camera.rect.height);
       }
            
       Vector3 sc = new Vector3(
           viewportSize.x * viewportPoint.x,
           viewportSize.y * viewportPoint.y,
           0);
            
       return sc;
   }
   
   public static bool MouseOverRectTransform(Vector3 mousePosition, GameObject isOverThis, RectTransform inRectTransform)
   {
       if (EventSystem.current.IsPointerOverGameObject())
       {
           PointerEventData pointerData = new PointerEventData(EventSystem.current)
           {
               position = Input.mousePosition
           };

           List<RaycastResult> results = new List<RaycastResult>();
           EventSystem.current.RaycastAll(pointerData, results);

           foreach (RaycastResult raycastResult in results)
           {
               if (raycastResult.gameObject.Equals(isOverThis))
               {
                   if(inRectTransform.rect.Contains(mousePosition))
                       return true;
               }
           }
       }

       return false;
   }
   
   public static bool TouchOverRectTransform(Touch touch, Vector3 mousePosition, GameObject isOverThis, RectTransform inRectTransform)
   {
       if (EventSystem.current.IsPointerOverGameObject())
       {
           PointerEventData pointerData = new PointerEventData(EventSystem.current)
           {
               position = touch.position
           };

           List<RaycastResult> results = new List<RaycastResult>();
           EventSystem.current.RaycastAll(pointerData, results);

           foreach (RaycastResult raycastResult in results)
           {
               if (raycastResult.gameObject.Equals(isOverThis))
               {
                   if(inRectTransform.rect.Contains(mousePosition))
                       return true;
               }
           }
       }

       return false;
   }
}
