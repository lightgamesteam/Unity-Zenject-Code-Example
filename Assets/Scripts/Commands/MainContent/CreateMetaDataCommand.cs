using System.Linq;
using Newtonsoft.Json;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class CreateMetaDataCommand : ICommandWithParameters
    {
        [Inject] private MetaDataModel _metaDataModel;

        [Inject] private ApplicationSettingsInstaller.ServerSettings serverSettings;

        private const string MaxRecent = "maxRecent";
        private const string MaxFavorite = "maxFavourite";

        public void Execute(ISignal signal)
        {
            var parameter = (CreateMetaDataCommandSignal) signal;
            var contentResponse = JsonConvert.DeserializeObject<MetaDataResource>(parameter.MetaDataResponse);

            SaveLinks(contentResponse);
            SaveMaxFavoriteRecent(contentResponse);
            SaveTermLink(contentResponse);
        }
        
        private void SaveTermLink(MetaDataResource contentResponse)
        {
            if (contentResponse.Links != null && contentResponse.Links.Count > 0)
            {
                foreach (LinkResource link in contentResponse.Links)
                {
                    foreach (var linkLocal in link.LinkLocal)
                    {
                        if (linkLocal.Name == "3DL Term") 
                        {
                            _metaDataModel.LinkTerm = link.Uri;
                        }
                    }
                }
                
                _metaDataModel.OnTermLinksUpdate?.Invoke();
            }
        }

        private void SaveLinks(MetaDataResource contentResponse)
        {
            //_metaDataModel.Link.Uri = $"{serverSettings.ConsoleLoginUrl}" + "?token={token}&culture={culture}";
            if (contentResponse.Links != null && contentResponse.Links.Count > 0)
            {
                _metaDataModel.Link = contentResponse.Links[0];
                _metaDataModel.LinkLocal = _metaDataModel.Link.LinkLocal.ToDictionary(item => item.Culture, item => item.Name);
            }
        }

        private void SaveMaxFavoriteRecent(MetaDataResource contentResponse)
        {
            var metaData = contentResponse.MetaData.ToDictionary(item => item.Name, item => item.Value);
            if (metaData.ContainsKey(MaxRecent))
            {
                _metaDataModel.MaxRecent = int.Parse(metaData[MaxRecent]);
            }

            if (metaData.ContainsKey(MaxFavorite))
            {
                _metaDataModel.MaxFavorites = int.Parse(metaData[MaxFavorite]);
            }
        }
    }
}