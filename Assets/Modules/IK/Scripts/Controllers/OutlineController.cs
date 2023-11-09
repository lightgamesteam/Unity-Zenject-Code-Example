using System.Collections;
using UnityEngine;

namespace Module.IK
{

    public class OutlineController
    {

        static public void ShowOutline(Outline3D Outline)
        {
            DataModel.MonoBehaviour.StartCoroutine(StartAnimation(Outline));
        }


        static public IEnumerator StartAnimation(Outline3D Outline)
        {
            Outline.OutlineWidth = 10;

            while (Outline.OutlineWidth > 0)
            {
                Outline.OutlineWidth -= 0.7f;
                yield return new WaitForSeconds(0.1f);
            }

            Outline.OutlineWidth = 0;
        }
    }

}
