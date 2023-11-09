using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class CreateDescriptionViewCommand : ICommandWithParameters
    {
        [Inject] private readonly DescriptionView.Factory _factory;
        [Inject] private readonly AccessibilityModel _accessibilityModel;
        [Inject] private readonly LocalizationModel _localizationModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private UserLoginModel _loginModel;
        [Inject] protected readonly SignalBus _signal;

        private DescriptionView view;
        
        public void Execute(ISignal signal)
        {
            _signal.Fire<CloseDescriptionViewSignal>();
            var parameter = (CreateDescriptionViewCommandSignal) signal;

            view = _factory.Create();
            view.AssetId = parameter.AssetId;
            view.LabelId = parameter.LabelId;
            view.CultureCode = parameter.CultureCode;
            view.IsMultiViewSecond = parameter.IsMultiViewSecond;
            
            view.SetParentContainer(view.transform.parent);
            view.SetTitle(parameter.Title);
            view.SetDescription(parameter.Description);
            view.SetFontSize(_accessibilityModel.ModulesFontSizeScaler);
            Debug.Log($">> @@@ CreateDescriptionViewCommand: AssetId = {view.AssetId}  LabelId = {view.LabelId}");
            SetAudioFileURL(parameter.IncludeTeacherAudioFile);
            view.UpdatePlayButtonVisibility();
            if (IsTTSEnabled())
            {
                view.PlaySound(true);
            }

            if (parameter.IsMultiViewSecond)
            {
                view.RepositionView();
            }

            UpdateTooltips(view.gameObject, parameter.CultureCode);
            SaveCreatedViewToArray(string.IsNullOrEmpty(parameter.LabelId) ? parameter.AssetId : parameter.LabelId, view);
        }

        private void SetAudioFileURL(bool includeTeacherAudioFile )
        {
            view.AudioFileURL = string.Empty;
            
            var asset = _contentModel.GetAssetById(int.Parse(view.AssetId));

            if (string.IsNullOrEmpty(view.LabelId))
            {
                if (includeTeacherAudioFile)
                {
                    view.AudioFileURL = _loginModel.IsTeacher 
                        ? asset?.Asset?.LocalizedDescription?.Single(item => item.Culture.Equals(view.CultureCode))?.AudioFileUrl 
                        : asset?.Asset?.LocalizedStudentDescription?.Single(item => item.Culture.Equals(view.CultureCode))?.AudioFileUrl;
                }
                else
                {
                    view.AudioFileURL = asset?.Asset?.LocalizedStudentDescription?.Single(item => item.Culture.Equals(view.CultureCode))?.AudioFileUrl;
                }
            }
            else
            {
                view.AudioFileURL = asset?.AssetDetail?.AssetContentPlatform?.assetLabel?.ToList()
                    ?.First(label => label.labelId.Equals(int.Parse(view.LabelId.Split('_')[1])))?.labelLocal.ToList()
                    ?.First(item => item.Culture.Equals(view.CultureCode))?.AudioFileUrl;
            }
        }

        private bool IsTTSEnabled()
        {
            return PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio);
        }

        private void SaveCreatedViewToArray(string id, DescriptionView view)
        {
            _homeModel.OpenedDescriptions.Add(id, view);
        }
        
        private void UpdateTooltips(GameObject go, string cultureCode)
        {
            go.gameObject.GetAllComponentsInChildren<TooltipEvents>().ForEach(t =>
            {
                var keyTranslation = t.GetKey();

                if (!string.IsNullOrEmpty(keyTranslation))
                {
                    t.SetHint(_localizationModel.GetSystemTranslations(cultureCode, keyTranslation));
                }
            });
        }
    }
    
    public class CreateDescriptionViewCommandSignal : ISignal
    {
        public string AssetId { get; }
        public string LabelId { get; }
        public string Title { get; }
        public string CultureCode { get; }
        public string Description { get; }
        public bool IsMultiViewSecond { get; }
        public bool IncludeTeacherAudioFile { get; }

        public CreateDescriptionViewCommandSignal(string assetId, string labelId, string title, string cultureCode, string description,
            bool includeTeacherAudioFile, bool isMultiViewSecond)
        {
            AssetId = assetId;
            LabelId = labelId;
            Title = title;
            CultureCode = cultureCode;
            Description = description;
            IsMultiViewSecond = isMultiViewSecond;
            IncludeTeacherAudioFile = includeTeacherAudioFile;
        }
    }
}