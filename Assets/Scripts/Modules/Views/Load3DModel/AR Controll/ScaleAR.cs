using UnityEngine;

public class ScaleAR : MonoBehaviour
{
    private float minScale = 0.01f;
    public static bool Interactable = false;

    void Update()
    {
        if(!Interactable)
            return;
        
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float scale = (touchDeltaMag - prevTouchDeltaMag) * (1/(Screen.height*1.5f));

            if (transform.localScale.x >= minScale)
            {
                transform.Scale(scale);
                
                if (transform.localScale.x < minScale)
                {
                    transform.localScale = new Vector3(minScale, minScale, minScale);
                }
            }
        }
    }
}
