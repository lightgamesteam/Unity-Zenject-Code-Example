using System.Collections;
using UnityEngine;

namespace Module.IK
{

    public class SelectedController
    {

        static public void SetColor(GameObject gameObject, Color value)
        {
            if (gameObject.GetComponent<MeshRenderer>().material.HasProperty("_Color"))
            {
                var color = gameObject.GetComponent<MeshRenderer>().material.color;
                gameObject.GetComponent<MeshRenderer>().material.color = value;
            }
        }


        static public void SetClick()
        {
            DataModel.MonoBehaviour.StartCoroutine(StartCounter());
        }


        static private IEnumerator StartCounter()
        {
            DataModel.IsClick = true;
            yield return new WaitForSeconds(0.1f);
            DataModel.IsClick = false;
        }
    }

}