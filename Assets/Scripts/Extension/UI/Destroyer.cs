using UnityEngine;

public class Destroyer : MonoBehaviour
{
   public void SelfDestroy()
   {
      gameObject.SelfDestroy();
   }
   
   public void SelfDestroy(float wait)
   {
      gameObject.SelfDestroy(wait);
   }
}
