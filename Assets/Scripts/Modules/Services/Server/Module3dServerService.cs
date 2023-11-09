using System;
using System.Collections.Generic;
using BestHTTP;
using TDL.Constants;
using TDL.Models;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class Module3dServerService : IModule3dServerService
    {
        [Inject] private readonly ApplicationSettingsInstaller.ServerSettings _serverSettings;
        [Inject] private readonly UserLoginModel _userLoginModel;
        [Inject] private readonly SignalBus _signal;

        public void GetAssetDetails(int assetId)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetAssetDetails) {Query = ServerConstants.AssetIdDetailsQuery + assetId};

            var assetDetailsRequest = new HTTPRequest(uri.Uri,
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                        _signal.Fire(new Module3dCreateAssetDetailsCommandSignal(assetId, response.DataAsText));
                    }
                });

            assetDetailsRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            assetDetailsRequest.Send();
        }

        #region Search

        public void GetSearch(string searchValue, string cultureCode, string[] assetTypes, Action<List<ClientAssetModel>> clientAssetModels)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetSearch)
            {
                Query = ServerConstants.SearchQuery + searchValue + "&" + ServerConstants.SearchLanguageQuery + cultureCode
            };

            var assetDetailsRequest = new HTTPRequest(uri.Uri,
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                        _signal.Fire(new SearchAssetCommandSignal(response.DataAsText, assetTypes, clientAssetModels));
                    }
                });

            assetDetailsRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            assetDetailsRequest.Send();
        }
        
        #endregion
    }
}