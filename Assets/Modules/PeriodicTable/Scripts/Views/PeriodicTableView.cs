using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Module.PeriodicTable
{
    public class PeriodicTableView : MonoBehaviour
    {
        #region Init variables

        public static Transform Transform;

        [SerializeField] private GameObject _parentTypesButton;
        [SerializeField] private List<PeriodicModel.PeriodicType> _periodicTypes = PeriodicModel.PeriodicTypes;
        private List<PeriodicModel.PeriodicElement> _periodicElements = PeriodicModel.PeriodicElements;

        #endregion

        #region Unity callbacks

        private void Start()
        {
            Transform = transform;
            PeriodicTableController.UpdatePeriodicElements();
            BuildTable();
            InitTextLocalization();
        }

        #endregion

        #region Public Methods

        public void BuildTable()
        {
            _parentTypesButton = transform.Find("Types").gameObject;

            if (!CheckElementInInspector())
                return;

            ClearTable();

            var elements = transform.Find("Elements");
            PeriodicTableController.FillingPeriodicTable(elements);
            PeriodicTableController.FillingInspector();
            SetTypeButtons();

            ClearButtonTypes(_parentTypesButton);
            FillingButtonList(_parentTypesButton);

            //DebugController.DebugInfo(PeriodicModel.PeriodicTypes);
            //DebugController.DebugInfo(PeriodicModel.PeriodicElements);
        }

        public void InitTextLocalization()
        {
            var _parentTexts = transform.Find("Texts");
            var explanation_1 = _parentTexts.Find("Explanation_1").GetComponent<TextMeshPro>();
            var lanthanideSeries = _parentTexts.Find("Lanthanide_Series").GetComponent<TextMeshPro>();
            var actinideSeries = _parentTexts.Find("Actinide_Series").GetComponent<TextMeshPro>();
            var title = _parentTexts.Find("Title").GetComponent<TextMeshPro>();
            var explanation = _parentTexts.Find("Explanation").Find("Element_Data").gameObject.AddComponent<ElementDataView>();
            var symbol_string = LocalizationController.GetWord("Symbol");
            var name_string = LocalizationController.GetWord("Name");
            var boilingPoint_string = LocalizationController.GetWord("BoilingPoint").Replace("[[", "<").Replace("]]", ">");
            var atomicNumber_string = LocalizationController.GetWord("AtomicNumberTitle").Replace("[[", "<").Replace("]]", ">");
            var atomicMass_string = LocalizationController.GetWord("AtomicMass").Replace("[[", "<").Replace("]]", ">");
            var explanation_string = LocalizationController.GetWord("Explanation").Replace("[[", "<").Replace("]]", ">");
            var title_string = LocalizationController.GetWord("Title").Replace("[[", "<").Replace("]]", ">");
            var actinideSeries_string = LocalizationController.GetWord("ActinideSeries").Replace("[[", "<").Replace("]]", ">");
            var lanthanideSeries_string = LocalizationController.GetWord("LanthanideSeries").Replace("[[", "<").Replace("]]", ">");

            explanation.SetTextElement(symbol_string, name_string, boilingPoint_string, atomicNumber_string, atomicMass_string);
            explanation_1.fontSize = 0.5f;
            lanthanideSeries.fontSize = 0.5f;
            actinideSeries.fontSize = 0.5f;
            lanthanideSeries.transform.position += Vector3.left * 0.1f;
            actinideSeries.transform.position += Vector3.left * 0.1f;
            explanation_1.text = explanation_string;
            lanthanideSeries.text = lanthanideSeries_string;
            actinideSeries.text = actinideSeries_string;
            title.text = title_string;
        }

        public void ClearTable()
        {
            if (!CheckElementInInspector())
                return;

            ApplicationView.ClearComponentsInChildren(gameObject, typeof(BoxCollider));
            ApplicationView.ClearComponentsInChildren(gameObject, typeof(PeriodicElementView));

            _periodicTypes.Clear();
            _periodicElements.Clear();

            ClearButtonTypes(_parentTypesButton);
        }

        #endregion

        #region Private Methods

        private void SetTypeButtons()
        {
            Transform[] AllObjects = _parentTypesButton.GetComponentsInChildren<Transform>();

            foreach (Transform item in AllObjects)
            {
                if (item.parent == null)
                    continue;

                for (int i = 0; i < _periodicTypes.Count; i++)
                {
                    if (_periodicTypes[i].gameObject.name.Equals(item.gameObject.name))
                    {
                        PeriodicModel.PeriodicType periodicType = _periodicTypes[i];
                        periodicType.button = item.gameObject;
                        periodicType.positionButton = item.transform.position;
                        _periodicTypes[i] = periodicType;

                        var typeDataViewObject = Instantiate(ApplicationView.instance.TypeDataViewPref, item);
                        var typeDataView = typeDataViewObject.GetComponent<TypeDataView>();
                        var name = LocalizationController.GetWord(_periodicTypes[i].name.Replace("_", ""));
                        typeDataView.SetTextType(name);
                    }
                }
            }
        }


        private bool CheckElementInInspector()
        {
            if (_parentTypesButton != null)
            {
                return true;
            }
            else
            {
                Debug.LogError("<color=red>Error!</color> \nNo items found...");
                return false;
            }
        }

        private static void ClearButtonTypes(GameObject gameObject)
        {
            ApplicationView.ClearComponentsInChildren(gameObject, typeof(PeriodicGroupView));
            ApplicationView.ClearComponentsInChildren(gameObject, typeof(BoxCollider));
        }

        private static void FillingButtonList(GameObject gameObject)
        {
            foreach (Transform item in gameObject.GetComponentsInChildren<Transform>())
            {
                if (item.parent == null)
                    continue;

                if (item.gameObject.HasComponent<MeshRenderer>())
                {
                    ApplicationView.AddNewComponent(item.parent.parent.gameObject, typeof(PeriodicGroupView));
                    ApplicationView.AddNewComponent(item.parent.parent.gameObject, typeof(BoxCollider));
                }
            }
        }

        #endregion
    }
}