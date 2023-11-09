using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace Module.PeriodicTable
{
    public class LocalizationController
    {
        private static Dictionary<string, Dictionary<string, string>> _languages;
        private static XmlDocument xmlDoc = new XmlDocument();
        private static XmlReader reader;

        public static void Init(string currentLanguage)
        {
            _languages = new Dictionary<string, Dictionary<string, string>>();
            reader = XmlReader.Create(new StringReader(ApplicationView.instance.LanguageFile.text));
            xmlDoc.Load(reader);

            XmlNodeList langs = xmlDoc["Data"].ChildNodes;
            for (int i = 0; i < langs.Count; i++)
            {
                _languages.Add(langs[i].LocalName, new Dictionary<string, string>());
                XmlNodeList data = langs[i].ChildNodes;

                for (int j = 0; j < langs[i].ChildNodes.Count; j++)
                {
                    _languages[langs[i].LocalName]
                        .Add(data[j].Attributes["Key"].Value, data[j].Attributes["Word"].Value);
                }
            }

            SetLanguage(currentLanguage);
        }

        private static void SetLanguage(string currentLanguage)
        {
            switch (currentLanguage)
            {
                case "nb-NO":
                    DataModel.CurrentLanguage = DataModel.Language.Norwegian_NB;
                    break;
                case "nn-NO":
                    DataModel.CurrentLanguage = DataModel.Language.Norwegian_NN;
                    break;
                default:
                    DataModel.CurrentLanguage = DataModel.Language.English;
                    break;
            }
        }

        public static string GetWord(string lang, string key)
        {
            return _languages[lang][key];
        }

        public static string GetWord(string key)
        {
            if (_languages[DataModel.CurrentLanguage.ToString()].ContainsKey(key))
            {
                return _languages[DataModel.CurrentLanguage.ToString()][key];
            }

            return null;
        }
    }
}