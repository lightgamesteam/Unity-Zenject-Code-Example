using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace Module.PeriodicTable
{
    public class PeriodicElementsController : MonoBehaviour
    {
        private static Dictionary<string, Dictionary<string, string>> _atomicProperties;
        private static Dictionary<string, Dictionary<string, string>> _physicalProperties;
        private static Dictionary<string, string> _elements;

        private static XmlDocument xmlDoc = new XmlDocument();
        private static XmlReader reader;

        public static void Init()
        {
            DataModel.ScreenOrientation = Screen.orientation;
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            _atomicProperties = new Dictionary<string, Dictionary<string, string>>();
            _physicalProperties = new Dictionary<string, Dictionary<string, string>>();
            _elements = new Dictionary<string, string>();
            reader = XmlReader.Create(new StringReader(ApplicationView.instance.PeriodicPropertiesFile.text));
            xmlDoc.Load(reader);

            XmlNodeList periodicElements = xmlDoc["PeriodicElements"].GetElementsByTagName("Element");
            for (int i1 = 0; i1 < periodicElements.Count; i1++)
            {
                var shortName = periodicElements[i1].Attributes["ShortName"].Value;
                var nameElement = periodicElements[i1].Attributes["Name"].Value;
                _atomicProperties.Add(shortName, new Dictionary<string, string>());
                _physicalProperties.Add(shortName, new Dictionary<string, string>());
                _elements.Add(shortName, nameElement);

                XmlNodeList properties = periodicElements[i1].ChildNodes;
                for (int i2 = 0; i2 < properties.Count; i2++)
                {
                    var Key = properties[i2].Attributes["Key"].Value;
                    var Propertie = properties[i2].Attributes["Propertie"].Value;
                    Propertie = ReplaceSupText(Propertie);

                    if (properties[i2].LocalName.Equals("AtomicProperties"))
                    {
                        _atomicProperties[shortName].Add(Key, Propertie);
                    }

                    if (properties[i2].LocalName.Equals("PhysicalProperties"))
                    {
                        _physicalProperties[shortName].Add(Key, Propertie);
                    }
                }
            }
        }

        private static string ReplaceSupText(string Propertie)
        {
            var sups = new Dictionary<string, string>
            {
                {"[sup]", "<sup>"},
                {"[/sup]", "</sup>"}
            };
            foreach (var sup in sups)
            {
                Propertie = Propertie.Replace(sup.Key, sup.Value);
            }

            return Propertie;
        }

        public static List<Description> GetPhysicalProperties(string name)
        {
            List<Description> physicalProperties = new List<Description>();
            if (!_physicalProperties.ContainsKey(name))
            {
                return physicalProperties;
            }

            foreach (var physicalPropertie in _physicalProperties[name])
            {
                Description temp = new Description
                {
                    Name = physicalPropertie.Key,
                    Value = physicalPropertie.Value
                };
                physicalProperties.Add(temp);
            }

            return physicalProperties;
        }

        public static List<Description> GetAtomicProperties(string name)
        {
            List<Description> atomicProperties = new List<Description>();
            if (!_atomicProperties.ContainsKey(name))
            {
                return atomicProperties;
            }

            foreach (var atomicPropertie in _atomicProperties[name])
            {
                Description temp = new Description
                {
                    Name = atomicPropertie.Key,
                    Value = atomicPropertie.Value
                };
                atomicProperties.Add(temp);
            }

            return atomicProperties;
        }

        public static string GetNameElement(string name)
        {
            if (_elements.ContainsKey(name))
            {
                return _elements[name];
            }

            return "";
        }
    }
}