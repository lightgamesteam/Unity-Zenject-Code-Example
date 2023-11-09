using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Zenject;
using TDL.Models;
using System.Collections;
using TDL.Services;

namespace Module.PeriodicTable
{
    public class ApplicationView : MonoBehaviour
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private ICacheService _cacheService;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private SignalBus _signal;

        #region Init variables

        public static ApplicationView instance;

        [SerializeField] 
        private Camera _camera;
        
        public Image ImageBackground;
        public Image PopUpBackground;
        public GameObject Description;
        public Button ButtonAtomicProperties;
        public Button ButtonPhysicalProperties;
        public TextMeshProUGUI NameTabAtomic;
        public TextMeshProUGUI NameTabPhysical;
        public ElementDataView ElementDataViewPref;
        public TypeDataView TypeDataViewPref;

        [Space(10)] 
        public GameObject AtomicPropertiesGrid;
        public GameObject PhysicalPropertiesGrid;
        
        [Space(10)] [Header("XML Files")] 
        public TextAsset LanguageFile;
        public TextAsset PeriodicPropertiesFile;

        public BackgroundView BackgroundView { get; set; }
        public SignalBus Signal { get => _signal; set => _signal = value; }


        #endregion

        #region Unity callbacks

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance == this)
            {
                Destroy(gameObject);
            }

            StartCoroutine(Loaded());
        }

        private IEnumerator Loaded()
        {
            var modelPath = _cacheService.GetPathToAsset(_contentModel.SelectedAsset.Asset.Id, _contentModel.SelectedAsset.Asset.Version);
            var bundleLoadRequest = AssetBundle.LoadFromFileAsync(modelPath);
            yield return bundleLoadRequest;

            var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                yield break;
            }

            var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>("Resources_PeriodicTable");
            yield return assetLoadRequest;

            var prefab = assetLoadRequest.asset as GameObject;
            myLoadedAssetBundle.Unload(false);

            var periodicTableView = Instantiate(prefab, transform.parent);
            periodicTableView.name = "PeriodicTable";
            periodicTableView.AddComponent<PeriodicTableView>();

            Init();
        }

        private void Init()
        {
            var canvas = ImageBackground.GetComponentInParent<RectTransform>();
            canvas.sizeDelta = new Vector2(Screen.width, Screen.height);
            BackgroundView = ImageBackground.GetComponent<BackgroundView>();
            PeriodicElementsController.Init();
            LocalizationController.Init(_localizationModel.CurrentLanguageCultureCode);
        }

        private void OnDestroy()
        {
            instance = null;
        }

        #endregion

        public static void SetActiveCollider(GameObject gameObject, bool status)
        {
            var boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = status;
            }
            else
            {
                Debug.LogError("BoxCollider on <b>" + gameObject.name + "</b> is absent!");
            }
        }

        public static void AddNewComponent(GameObject gameObject, Type TypeComponent)
        {
            if (gameObject.GetComponent(TypeComponent) == null)
            {
                gameObject.AddComponent(TypeComponent);
            }
        }

        public static void ClearComponentsInChildren(GameObject gameObject, Type TypeComponent)
        {
            foreach (var item in gameObject.GetComponentsInChildren(TypeComponent))
            {
                DestroyImmediate(item);
            }
        }

        public static void SetActivePopUp(bool state)
        {
            var type = SelectedModel.SelectElement.type;
            if (DataModel.groupColors.ContainsKey(type))
            {
                foreach (var image in instance.Description.GetComponentsInChildren<Image>())
                {
                    image.color = DataModel.groupColors[type];

                    if (!image.gameObject.Equals(instance.ButtonPhysicalProperties.gameObject)
                        && !image.gameObject.Equals(instance.ButtonAtomicProperties.gameObject))
                    {
                        image.color -= DataModel.ButtonOnDescriptionColor;
                    }
                }
            }

            instance.ButtonAtomicProperties.onClick.Invoke();
            instance.PopUpBackground.gameObject.SetActive(state);
            instance.PopUpBackground.color = new Color(1, 1, 1, 0);
            TweenController.TweenColorImage(instance.PopUpBackground, DataModel.groupColors[type],
                DataModel.SpeedTweenFastColorBackground, state);
            instance.Description.SetActive(false);
        }

        public void SetActiveDescription()
        {
            Description.SetActive(true);
            ReturnAllPoolObjects();
            GetPoolObjects();
            SetLocalizatioPropertiesn();
        }

        public void SetLocalizatioPropertiesn()
        {
            NameTabAtomic.text = LocalizationController.GetWord("AtomicData");
            NameTabPhysical.text = LocalizationController.GetWord("PhysicalProperties");
        }

        private static void GetPoolObjects()
        {
            var nameSelectElement = SelectedModel.SelectElement.name;
            var physicalProperties = PeriodicElementsController.GetPhysicalProperties(nameSelectElement);
            var atomicProperties = PeriodicElementsController.GetAtomicProperties(nameSelectElement);
            var defaultProperties = LocalizationController.GetWord("NoDataForThisItem");

            if (nameSelectElement == "LanthanideSeries")
            {
                defaultProperties = LocalizationController.GetWord("LanthanideSeries");
            }
            else if (nameSelectElement == "ActinideSeries")
            {
                defaultProperties = LocalizationController.GetWord("ActinideSeries");
            }

            if (physicalProperties.Count == 0)
            {

                CreateDefaultProperties(instance.PhysicalPropertiesGrid, defaultProperties);
            }

            if (atomicProperties.Count == 0)
            {
                CreateDefaultProperties(instance.AtomicPropertiesGrid, defaultProperties);
            }

            for (int i = 0; i < physicalProperties.Count; i++)
            {
                CreateProperties(instance.PhysicalPropertiesGrid, physicalProperties, i);
            }

            for (int i = 0; i < atomicProperties.Count; i++)
            {
                CreateProperties(instance.AtomicPropertiesGrid, atomicProperties, i);
            }
        }

        private static void ReturnAllPoolObjects()
        {
            var poolPhysicalObjects = instance.PhysicalPropertiesGrid.GetComponentsInChildren<PoolObject>();
            var poolAtomicObjects = instance.AtomicPropertiesGrid.GetComponentsInChildren<PoolObject>();
            foreach (var poolObject in poolPhysicalObjects)
            {
                poolObject.ReturnToPool();
            }

            foreach (var poolObject in poolAtomicObjects)
            {
                poolObject.ReturnToPool();
            }
        }

        private static void CreateProperties(GameObject parent, List<Description> descriptions, int index)
        {
            GameObject item = PoolManager.GetObject("GridItem", parent);
            var properties = descriptions[index];
            var image = item.GetComponent<Image>();
            var named_Text = item.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var value_Text = item.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            var name = LocalizationController.GetWord(properties.Name);
            var value = LocalizationController.GetWord(properties.Value) ?? SeparatorLanguage(properties.Value);
            image.color = ((index % 2).Equals(0)) ? Color.clear : new Color(0.0f, 0.0f, 0.0f, 0.1f);
            named_Text.text = $"{name}";
            value_Text.text = value;
        }

        private static string SeparatorLanguage(string value)
        {
            if (DataModel.CurrentLanguage == DataModel.Language.Norwegian_NB
                || DataModel.CurrentLanguage == DataModel.Language.Norwegian_NN)
            {
                return value.Replace(".", ",");
            }

            return value;
        }

        private static void CreateDefaultProperties(GameObject parent, string text)
        {
            GameObject item = PoolManager.GetObject("GridItem", parent);
            var image = item.GetComponent<Image>();
            var named = item.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var value = item.transform.Find("Value").GetComponent<TextMeshProUGUI>();

            image.color = Color.clear;
            named.text = text;
            value.text = string.Empty;
        }

        public void MoveElementToCenter(GameObject elementObject)
        {
            SetActiveCollider(elementObject, false);
            BackgroundView.SetСlickable(false);
            SelectedModel.SelectElement = PeriodicTableController.GetElement(elementObject);
            AnimationController.MoveElementToCenter(_camera, elementObject);
        }

        public void ReturnSelectedElement()
        {
            SetActiveCollider(SelectedModel.SelectElement.gameObject, false);
            BackgroundView.SetСlickable(false);
            AnimationController.ReturnSelectedElement();
            SelectedModel.ClearSelectElement();
        }

        public void OnCloseModule()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            TweenController.StopTweenForActiveObject();
        }
    }
}