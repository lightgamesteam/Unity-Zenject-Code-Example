using System.Collections.Generic;
using UnityEngine;

namespace Module.PeriodicTable
{
    public class PeriodicTableController : MonoBehaviour
    {
        public static void FillingPeriodicTable(Transform transform)
        {
            Transform[] AllObjects = transform.GetComponentsInChildren<Transform>();

            foreach (Transform item in AllObjects)
            {
                if (item.parent == null || item.Equals(transform))
                    continue;
                if (item.parent.Equals(transform))
                {
                    PeriodicModel.PeriodicType periodicType = new PeriodicModel.PeriodicType
                    {
                        name = item.name,
                        gameObject = item.gameObject
                    };
                    PeriodicModel.PeriodicTypes.Add(periodicType);
                }
                else
                {
                    PeriodicModel.PeriodicElement periodicElement = new PeriodicModel.PeriodicElement
                    {
                        name = item.name,
                        type = item.parent.name,
                        gameObject = item.gameObject,
                        position = item.position,
                        material = item.GetComponent<MeshRenderer>().sharedMaterials[0]
                    };
                    PeriodicModel.PeriodicElements.Add(periodicElement);
                    ApplicationView.AddNewComponent(item.gameObject, typeof(PeriodicElementView));
                    ApplicationView.AddNewComponent(item.gameObject, typeof(BoxCollider));

                    var elementDataViewObject = Instantiate(ApplicationView.instance.ElementDataViewPref, item);
                    var elementDataView = elementDataViewObject.GetComponent<ElementDataView>();
                    var symbol = item.name;
                    var name = PeriodicElementsController.GetNameElement(symbol);
                    name = LocalizationController.GetWord(name) != null 
                        ? LocalizationController.GetWord(name)
                        : name;
                    var physicalProperties = PeriodicElementsController.GetPhysicalProperties(symbol);
                    var atomicProperties = PeriodicElementsController.GetAtomicProperties(symbol);
                    var boilingPoint_C = physicalProperties.Find(type => type.Name == "BoilingPoint_C").Value.ToString().Replace("°C", "");
                    var atomicNumber = atomicProperties.Find(type => type.Name == "AtomicNumber").Value;
                    var atomicMass = atomicProperties.Find(type => type.Name == "RelativeAtomicMass").Value;
                    elementDataView.SetTextElement(symbol, name, boilingPoint_C, atomicNumber, atomicMass);
                }
            }
        }

        public static void FillingInspector()
        {
            for (int i = 0; i < PeriodicModel.PeriodicTypes.Count; i++)
            {
                var periodicElements =
                    PeriodicModel.PeriodicElements.FindAll(x => x.type == PeriodicModel.PeriodicTypes[i].name);
                var periodicType = new PeriodicModel.PeriodicType
                {
                    name = PeriodicModel.PeriodicTypes[i].name,
                    gameObject = PeriodicModel.PeriodicTypes[i].gameObject,
                    periodicElement = periodicElements
                };
                PeriodicModel.PeriodicTypes[i] = periodicType;
            }
        }

        public static void UpdatePeriodicElements()
        {
            PeriodicModel.PeriodicElements.Clear();
            foreach (var periodicTypes in PeriodicModel.PeriodicTypes)
            {
                var periodicElements = periodicTypes.periodicElement;
                foreach (var periodicElement in periodicElements)
                {
                    PeriodicModel.PeriodicElements.Add(periodicElement);
                }
            }
        }

        public static PeriodicModel.PeriodicType GetType(GameObject gameObject)
        {
            var type = PeriodicModel.PeriodicTypes.Find(x => x.name.Equals(gameObject.name));
            return type;
        }

        public static PeriodicModel.PeriodicElement GetElement(GameObject gameObject)
        {
            var element = PeriodicModel.PeriodicElements.Find(x => x.gameObject.Equals(gameObject));
            return element;
        }

        public static List<PeriodicModel.PeriodicElement> GetElements(GameObject type)
        {
            var elements = PeriodicModel.PeriodicElements.FindAll(x => x.type.Equals(type.name));
            return elements;
        }
    }
}