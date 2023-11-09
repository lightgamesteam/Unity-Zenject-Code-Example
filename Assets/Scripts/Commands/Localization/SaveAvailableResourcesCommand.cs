using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CI.TaskParallel;
using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class SaveAvailableResourcesCommand : ICommandWithParameters
    {
        [Inject] private readonly LocalizationModel _localizationModel;
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
#if UNITY_WEBGL
            RunInBackgroundWebGL(signal);
#else
            RunInBackground(signal);
#endif
        }

        private void RunInBackground(ISignal signal)
        {
            UnityTask.Run(() =>
            {
                var parameter = (SaveAvailableResourcesCommandSignal) signal;

                var reader = XmlReader.Create(new StringReader(parameter.ResourcesResponse));
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                var xmlLanguages = xmlDoc["Data"].ChildNodes;
                for (var indexParent = 0; indexParent < xmlLanguages.Count; indexParent++)
                {
                    _localizationModel.AllSystemTranslations.Add(xmlLanguages[indexParent].LocalName,
                        new Dictionary<string, string>());
                    var data = xmlLanguages[indexParent].ChildNodes;

                    for (var indexChild = 0; indexChild < data.Count; indexChild++)
                    {
                        if (data[indexChild].NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }

                        _localizationModel.AllSystemTranslations[xmlLanguages[indexParent].LocalName]
                            .Add(data[indexChild].Attributes["Key"].Value, data[indexChild].Attributes["Value"].Value);
                    }
                }
                    
            }).ContinueOnUIThread(task =>
            {
                _signal.Fire<GetAvailableLanguagesCommandSignal>();
            });
        }
        
        private void RunInBackgroundWebGL(ISignal signal)
        {
            var parameter = (SaveAvailableResourcesCommandSignal) signal;

            var reader = XmlReader.Create(new StringReader(parameter.ResourcesResponse));
            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(reader);
            }
            catch (Exception e)
            {
                Debug.LogError($"EXCEPTION: {e}");
                throw;
            }

            var xmlLanguages = xmlDoc["Data"].ChildNodes;
            for (var indexParent = 0; indexParent < xmlLanguages.Count; indexParent++)
            {
                _localizationModel.AllSystemTranslations.Add(xmlLanguages[indexParent].LocalName,
                    new Dictionary<string, string>());
                var data = xmlLanguages[indexParent].ChildNodes;

                for (var indexChild = 0; indexChild < data.Count; indexChild++)
                {
                    if (data[indexChild].NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }

                    _localizationModel.AllSystemTranslations[xmlLanguages[indexParent].LocalName]
                        .Add(data[indexChild].Attributes["Key"].Value, data[indexChild].Attributes["Value"].Value);
                }
            }

            //MainScreenLocal();
            _signal.Fire<GetAvailableLanguagesCommandSignal>();
        }

        private void MainScreenLocal()
        {
            MainScreenEN(_localizationModel.AllSystemTranslations["en-US"]);
            MainScreenNorw(_localizationModel.AllSystemTranslations["nb-NO"]);
        }

        private void MainScreenEN(Dictionary<string, string> items)
        {
           items.Add(LocalizationConstants.MainScreenTextHome, "Home");
           items.Add(LocalizationConstants.MainScreenTextVr, "VR");
           items.Add(LocalizationConstants.MainScreenTextAr, "AR");
           items.Add(LocalizationConstants.MainScreenTextDownload, "Downloads");
           items.Add(LocalizationConstants.MainScreenTextRequestDemo, "Request a demo");
           items.Add(LocalizationConstants.MainScreenTextAboutAs, "About us");
           items.Add(LocalizationConstants.MainScreenTextCamp3d, "Rental and CAMP 3D");
           items.Add(LocalizationConstants.MainScreenTextAccessoriesForVr, "Accessories for VR");
           items.Add(LocalizationConstants.MainScreenTextAbout3dl, "About 3dl");
           items.Add(LocalizationConstants.MainScreenTextPrivacy, "Privacy");
           items.Add(LocalizationConstants.MainScreenTextContactUs, "Contact us");
           items.Add(LocalizationConstants.MainScreenTextButtonContacktUs, "Contact us");
           items.Add(LocalizationConstants.MainScreenTextCountTitle1, "Content Elements");
           items.Add(LocalizationConstants.MainScreenTextCountTitle2, "Years experience in Edtech");
           items.Add(LocalizationConstants.MainScreenTextMicrosoft, "Download for windows PCs.");
           items.Add(LocalizationConstants.MainScreenTextMacAppStore, "Download for your Apple computer.");
           items.Add(LocalizationConstants.MainScreenTextAppStore, "Download for iPhone and iPad.");
           items.Add(LocalizationConstants.MainScreenTextGooglePlay, "Download for the chromebook and other android devices.");
        }

        private void MainScreenNorw(Dictionary<string, string> items)
        {
            items.Add(LocalizationConstants.MainScreenTextHome, "Hjem");
            items.Add(LocalizationConstants.MainScreenTextVr, "3dl VR");
            items.Add(LocalizationConstants.MainScreenTextAr, "3dl AR");
            items.Add(LocalizationConstants.MainScreenTextDownload, "Nedlastinger");
            items.Add(LocalizationConstants.MainScreenTextRequestDemo, "Personvern");
            items.Add(LocalizationConstants.MainScreenTextAboutAs, "Om Oss");
            items.Add(LocalizationConstants.MainScreenTextCamp3d, "Utleie og CAMP 3D");
            items.Add(LocalizationConstants.MainScreenTextAccessoriesForVr, "Tilbehør til VR");
            items.Add(LocalizationConstants.MainScreenTextAbout3dl, "Om 3dl");
            items.Add(LocalizationConstants.MainScreenTextPrivacy, "Personvern");
            items.Add(LocalizationConstants.MainScreenTextContactUs, "Kontakt oss");
            items.Add(LocalizationConstants.MainScreenTextButtonContacktUs, "Kontakt oss");
            items.Add(LocalizationConstants.MainScreenTextCountTitle1, "3D Modell, video, animasjon, quiz og puslespill​");
            items.Add(LocalizationConstants.MainScreenTextCountTitle2, "skoler i norge");
            items.Add(LocalizationConstants.MainScreenTextMicrosoft, "Last ned for Windows-PCer.");
            items.Add(LocalizationConstants.MainScreenTextMacAppStore, "Last ned for Apple-datamaskinen din.");
            items.Add(LocalizationConstants.MainScreenTextAppStore, "Last ned for iPhone og iPad");
            items.Add(LocalizationConstants.MainScreenTextGooglePlay, "Last ned for Chrome - nettverket og andre Android - enheter.");
        }
    }
}