using TMPro;
using UnityEngine;

namespace Module.PeriodicTable
{
    public class ElementDataView : MonoBehaviour
    {
        private TextMeshPro _symbol;
        private TextMeshPro _name;
        private TextMeshPro _boilingPoint;
        private TextMeshPro _atomicNumber;
        private TextMeshPro _atomicMass;

        private void Awake()
        {
            _symbol = transform.Find("Symbol").GetComponent<TextMeshPro>();
            _name = transform.Find("Name").GetComponent<TextMeshPro>();
            _boilingPoint = transform.Find("Boiling_Point").GetComponent<TextMeshPro>();
            _atomicNumber = transform.Find("Atomic_Number").GetComponent<TextMeshPro>();
            _atomicMass = transform.Find("Atomic_Mass").GetComponent<TextMeshPro>();
        }

        public void SetTextElement(string symbol, string name, string boilingPoint, string atomicNumber, string atomicMass)
        {
            _symbol.text = symbol;
            _name.text = name;
            _boilingPoint.text = boilingPoint;
            _atomicNumber.text = atomicNumber;
            _atomicMass.text = atomicMass;
        }
    }
}