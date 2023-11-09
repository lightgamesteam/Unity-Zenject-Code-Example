using TMPro;
using UnityEngine;

namespace Module.PeriodicTable
{
    public class TypeDataView : MonoBehaviour
    {
        private TextMeshPro _name;

        private void Awake()
        {
            _name = transform.Find("Name").GetComponent<TextMeshPro>();
        }

        public void SetTextType(string name)
        {
            _name.text = name;
        }
    }
}