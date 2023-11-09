using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CopyPositionUI : MonoBehaviour
{
   public RectTransform copyPositionTo;
   public Vector3 displace;

   private RectTransform copyPositionFrom;

   private void Awake()
   {
      copyPositionFrom = GetComponent<RectTransform>();
   }

   private void OnEnable()
   {
      if(copyPositionTo == null)
         return;

      copyPositionTo.position = copyPositionFrom.position + displace;
   }
}