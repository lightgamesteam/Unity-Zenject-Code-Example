using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using BestHTTP;
using Newtonsoft.Json;
using TDL.Commands;
using UnityEngine;
using Zenject;
using TDL.Signals;
using TDL.Models;
using TDL.Constants;
using TDL.Server;
using TDL.Views;
using UnityEngine.Networking;
using BestHTTP.Forms;
using IdentityModel;
using IdentityModel.Client;
using Managers;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Signals.Login;
using HTTPRequest = BestHTTP.HTTPRequest;

namespace TDL.Services
{
    public class ClaimsLoginModel
    {
        [JsonProperty("authorizationToken")]
        public string AuthorizationToken { get; set; }
        
        [JsonProperty("user")]
        public ClientUser User { get; set; }
        
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
    
    public class TeamsTokenResponse
    {
        [JsonProperty("id_token")]
        public string IdentityToken { get; set; }
        
        [JsonProperty("access_token")]
        public string UserToken { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
    
    public class DiscoveryDocumentModel
    {
        [JsonProperty("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonProperty("jwks_uri")]
        public string JwksUri { get; set; }
        
        [JsonProperty("change_password_uri")]
        public string ChangePasswordUrl { get; set; }
        
        [JsonProperty("console_login_uri")]
        public string ConsoleLoginUrl {get; set; }
        
        [JsonProperty("grant_types_supported")]
        public List<string> GrandTypesSupported { get; set; }

        [JsonProperty("servers")]
        public Servers Servers { get; set; }

        public List<JsonWebKey> Keys { get; set; }
    }
    
    public class LoginModel
    {
        [JsonProperty("login_id")]
        public string LoginId { get; set; }
        
        [JsonProperty("login_provider")]
        public string LoginProvider { get; set; }
        
        [JsonProperty("org_uid")]
        public string OrgUid { get; set; }
        
        [JsonProperty("org_id")]
        public string OrgId { get; set; }
        
        [JsonProperty("org_name")]
        public string OrgName { get; set; }
        
        [JsonProperty("role")]
        public string Role {get; set; }
        
        [JsonProperty("has_subscriptions")]
        public bool HasSubscriptions { get; set; }
    }

    public class Servers
    {
        [JsonProperty("Api")] 
        public string Api { get; set; }
    }

    public class ServerService
    {
        [Inject] private DiscoveryDocumentModel discoveryDocumentModel;
        [Inject] private readonly ApplicationSettingsInstaller.ServerSettings _serverSettings;
        [Inject] private UserLoginModel _userLoginModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private AsyncProcessorService _asyncProcessor;
        [Inject] private ICacheService _cacheService;
        [Inject] private ExternalCallManager _externalCallManager;

        private string ClientPlatform
        {
            get
            {
                if (Application.isEditor)
                {
                    return _serverSettings.EditorPlatform.ToString();
                }

                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    return "WebGL";
                }

                return Application.platform.ToString();
            }
        }

        private static void DebugRequest(string message, HTTPRequest request, HTTPResponse response)
        {
            if (!PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.DebugRequest))
                return;

            switch (request.State)
            {
                // The request finished without any problems.
                case HTTPRequestStates.Finished:
                    Debug.Log($"{message} :: Request Finished Successfully!\n" + response.DataAsText);
                    break;

                // The request finished with an unexpected error.
                // The request's Exception property may contain more information about the error.
                case HTTPRequestStates.Error:
                    Debug.LogError($"{message} :: Request Finished with Error! " + (request.Exception != null
                        ? (request.Exception.Message + "\n" + request.Exception.StackTrace)
                        : "No Exception"));
                    break;

                // The request aborted, initiated by the user.
                case HTTPRequestStates.Aborted:
                    Debug.LogWarning($"{message} :: Request Aborted!");
                    break;

                // Connecting to the server timed out.
                case HTTPRequestStates.ConnectionTimedOut:
                    Debug.LogError($"{message} :: Connection Timed Out!");
                    break;

                // The request didn't finished in the given time.
                case HTTPRequestStates.TimedOut:
                    Debug.LogError($"{message} :: Processing the request Timed Out!");
                    break;
            }
        }

        #region TeamsLogin

        private enum TokenType
        {
            AccessToken = 0,
            RefreshToken = 1,
            IdToken = 2
        }
        
        private TokenRequest GenerateTokenRequest(string token, TokenType type)
        {
            var grantType = type == TokenType.AccessToken ? "urn:ietf:params:oauth:grant-type:teams_token" : "refresh_token";
            var r = new TokenRequest
            {
                ClientId = "Teams",
                ClientSecret = "/////////////////////////////////////",
                RequestUri = new Uri(discoveryDocumentModel.TokenEndpoint),
                GrantType = $"{grantType}",
                ClientCredentialStyle = ClientCredentialStyle.PostBody,
                ClientAssertion = new ClientAssertion
                {
                    Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                    Value = token
                }
            };
            return r;
        }
        
        public void RefreshTeamsLogin(Action onRefreshCallback = null)
        {
            var token = _userLoginModel.RefreshToken;
            var r = GenerateTokenRequest(token, TokenType.RefreshToken);
            
            var uri = new Uri(discoveryDocumentModel.TokenEndpoint);

            var tokenRequest = new HTTPRequest(uri, HTTPMethods.Post,
                (req, res) =>
                {
                    if (res.IsSuccess)
                    {
                        var tokenResponse = JsonConvert.DeserializeObject<TeamsTokenResponse>(res.DataAsText);
                        _userLoginModel.AuthorizationToken = tokenResponse.UserToken;
                        _userLoginModel.RefreshToken = tokenResponse.RefreshToken;
                        _userLoginModel.IdentityToken = tokenResponse.IdentityToken;

                        _signal.Fire(new PopupOverlaySignal(false));
                        onRefreshCallback?.Invoke();
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(true, "Invalid authorization." +
                                                                  "Please, login again or refresh this page."));
                    }
                });
            
            tokenRequest.FormUsage = HTTPFormUsage.UrlEncoded;
            tokenRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            tokenRequest.AddHeader("Accept", "application/json");
            tokenRequest.AddField("grant_type", r.GrantType);
            tokenRequest.AddField("client_id", r.ClientId);
            tokenRequest.AddField("refresh_token", token);
            tokenRequest.AddField("client_secret", r.ClientSecret);
            tokenRequest.AddField("client_assertion_type", r.ClientAssertion.Type);
            tokenRequest.AddField("client_assertion", r.ClientAssertion.Value);
            tokenRequest.AddField("scope", "openid profile");
            tokenRequest.AddField("platform", ClientPlatform);
            tokenRequest.AddField("client_version", Application.version);
            tokenRequest.Send();
        }

        public void TryLoginByTeams(string token)
        {
            var r = GenerateTokenRequest(token, TokenType.AccessToken);
            var uri = new Uri(discoveryDocumentModel.TokenEndpoint);
            
            var tokenRequest = new HTTPRequest(uri, HTTPMethods.Post,
                (req, res) =>
                {
                    if (res.IsSuccess)
                    {
                        var tokenResponse = JsonConvert.DeserializeObject<TeamsTokenResponse>(res.DataAsText);
                        var tokenValidator = new JwtSecurityTokenHandler();
                        var validationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "effective_id",
                            ValidAudiences = new [] 
                            {
                                r.ClientId,
                                "//////////////////////////////////////////////////"
                            },
                            ValidIssuer = _serverSettings.AuthUrl + "/",
                            IssuerSigningKeys = discoveryDocumentModel.Keys.Select(key 
                                => new JsonWebKey(JsonConvert.SerializeObject(key)))
                        };
            
                        tokenValidator.InboundClaimTypeMap.Clear();

                        Console.WriteLine(validationParameters.ValidIssuer);
                        var principal = tokenValidator.ValidateToken(tokenResponse.IdentityToken, validationParameters,
                            out var securityToken);

                        var roles = principal.Claims.Where(x => x.Type == "role").Select(s => s.Value);
                        
                        var model = new ClaimsLoginModel
                        {
                            AuthorizationToken = "Bearer " + tokenResponse.UserToken,
                            User = new ClientUser
                            {
                                Firstname = principal.Claims.First(x => x.Type == "given_name").Value,
                                Lastname = principal.Claims.First(x => x.Type == "family_name").Value,
                                Roles = roles.ToList(),
                                TermAccepted = true
                            },
                            Success = true
                        };

                        var serializedData = JsonConvert.SerializeObject(model);
                        _signal.Fire(new CreateUserLoginModelSignal(serializedData));
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(res));
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });
            
            tokenRequest.FormUsage = HTTPFormUsage.UrlEncoded;
            tokenRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            tokenRequest.AddHeader("Accept", "application/json");
            tokenRequest.AddField("grant_type", r.GrantType);
            tokenRequest.AddField("client_id", r.ClientId);
            tokenRequest.AddField("client_secret", r.ClientSecret);
            tokenRequest.AddField("client_assertion_type", r.ClientAssertion.Type);
            tokenRequest.AddField("client_assertion", r.ClientAssertion.Value);
            tokenRequest.AddField("scope", "openid email profile roles logins offline_access api");
            tokenRequest.AddField("platform", ClientPlatform);
            tokenRequest.AddField("client_version", Application.version);
            tokenRequest.Send();
        }

        #endregion

        public void GetDiscoveryDocument()
        {
            Debug.Log(
                $">>> GetDiscoveryDocument | ClientPlatform={ClientPlatform} ClientVersion={Application.version}");
            _signal.Fire<GetAvailableResourcesCommandSignal>();
            _signal.Fire<CheckLoginAsGuestCommandSignal>();
        }

        #region GuestLogin

        public void GetLoginGuest(string userName, string userPassword)
        {
            Debug.Log("ServerService => Get login as guest");
            var loginRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.GetLogin),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            //Set as guest and update then.
                            _signal.Fire(new CreateUserLoginModelSignal(response.DataAsText, true));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(new SendCrashAnalyticsCommandSignal("Login as guest http response | " + userName +
                                                                             " | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(new CheckInternetConnectionCommandSignal(new LoginClickViewSignal()));
                    }
                });

            loginRequest.AddField(ServerConstants.UserName, userName);
            loginRequest.AddField(ServerConstants.UserPassword, userPassword);
            loginRequest.AddField(ServerConstants.ClientPlatform, ClientPlatform);
            loginRequest.AddField(ServerConstants.ClientVersion, Application.version);
            loginRequest.AddField(ServerConstants.DeviceIdentifierField, DeviceInfo.GetDeviceUID());
            loginRequest.AddField("platform", ClientPlatform);
            loginRequest.AddField("client_version", Application.version);
            loginRequest.Send();
        }

        #endregion

        #region Login
        
        public void GetLogin(string userName, string userPassword)
        {
            Debug.Log("ServerService => Get login as user");
            var loginRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.GetLogin),
                HTTPMethods.Post,
                (request, response) =>
                {
                    Debug.Log(response);
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateUserLoginModelSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(new SendCrashAnalyticsCommandSignal("GetLogin http response | " + userName +
                                                                             " | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(new CheckInternetConnectionCommandSignal(new LoginClickViewSignal()));
                    }
                });

            loginRequest.AddField(ServerConstants.UserName, userName);
            loginRequest.AddField(ServerConstants.UserPassword, userPassword);
            loginRequest.AddField(ServerConstants.ClientPlatform, ClientPlatform);
            loginRequest.AddField(ServerConstants.ClientVersion, Application.version);
            loginRequest.AddField(ServerConstants.DeviceIdentifierField, DeviceInfo.GetDeviceUID());
            loginRequest.Send();
        }

        public void GetAvailableResources()
        {
            Debug.Log(
                $">>> GetAvailableResources | ClientPlatform={ClientPlatform} ClientVersion={Application.version}");
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetResources)
            {
                Query = ServerConstants.ClientVersion + "=" + Application.version
                        + "&" + ServerConstants.ClientPlatform + "=" + ClientPlatform
            };

            Debug.Log($"GetAvailableResources URI  {uri} and the query is {uri.Query}");
            var resourcesRequest = new HTTPRequest(uri.Uri,
                (request, response) =>
                {
                    DebugRequest("GetAvailableResources", request, response);
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            Debug.Log("GetAvailableResources responce  = " + response.DataAsText);
                            _signal.Fire(new DownloadAvailableResourcesCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(new SendCrashAnalyticsCommandSignal("GetAvailableResources http response | " +
                                                                             response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(
                            new CheckInternetConnectionCommandSignal(new GetAvailableResourcesCommandSignal()));
                    }
                });

            resourcesRequest.ConnectTimeout = TimeSpan.FromSeconds(10);
            resourcesRequest.Timeout = TimeSpan.FromSeconds(10);
            resourcesRequest.Send();
        }

        public void DownloadAvailableResources(string url)
        {
            var resourceRequest = new HTTPRequest(
                new Uri(url),
                (request, response) =>
                {
                    DebugRequest("DownloadAvailableResources", request, response);
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new SaveAvailableResourcesCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            Debug.Log("DownloadAvailableResources responce error  = " + response.DataAsText);
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("DownloadAvailableResources http response | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(
                            new CheckInternetConnectionCommandSignal(new GetAvailableResourcesCommandSignal()));
                    }
                });

            resourceRequest.Send();
        }

        public void GetAvailableLanguages()
        {
            Debug.Log("Get GetAvailableLanguages ");
            var contentRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.GetAvailableLanguages),
                (request, response) =>
                {
                    DebugRequest("GetAvailableLanguages", request, response);
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new SaveAvailableLanguagesCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(new SendCrashAnalyticsCommandSignal("GetAvailableLanguages http response | " +
                                                                             response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(
                            new CheckInternetConnectionCommandSignal(new GetAvailableLanguagesCommandSignal()));
                    }
                });

            contentRequest.Send();
        }

        #endregion

        #region UserContent

        public void GetUserContent()
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetUserContent); // {Query = "contentTypeId="};
            Debug.Log("ServerService => GetUserContent uri = " + uri.Uri);
            var getUserContentRequest = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateUserContentModelCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal(
                                    "GetUserContent http response | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            getUserContentRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            getUserContentRequest.SetHeader(ServerConstants.AcceptJsonKey, ServerConstants.AcceptJsonValue);
            getUserContentRequest.Send();
        }

        public void DeleteUserContent(int id)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.DeleteUserContent + "/" + id);

            var favoriteRequest = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                        _signal.Fire(new ShowDebugLogCommandSignal(
                            $"Delete UserContent Response: msg = {response.Message} || data = {response.DataAsText}"));
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                    }
                });

            favoriteRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            favoriteRequest.Send();
        }

        public void GetUserContentFile(string url, Action<bool, int, byte[]> callback = null)
        {
            var getUserContentFileRequest = new HTTPRequest(
                new Uri(url),
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                        callback?.Invoke(true, 100, response.Data);
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                        callback?.Invoke(false, 0, null);
                    }
                });

            getUserContentFileRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            getUserContentFileRequest.OnProgress +=
                (r, downloaded, length) =>
                {
                    float pr = downloaded / (float)length * 100f;
                    callback?.Invoke(false, (int)pr, null);
                };

            getUserContentFileRequest.ConnectTimeout = TimeSpan.FromDays(1);
            getUserContentFileRequest.Timeout = TimeSpan.FromDays(1);
            getUserContentFileRequest.Send();
        }
        
        public void GetUserContentFileWebGL(string url, Action<bool, int, string> callback = null)
        {
            var getUserContentFileRequest = new HTTPRequest(
                new Uri(url),
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                        callback?.Invoke(true, 100, response.DataAsText);
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                        callback?.Invoke(false, 0, null);
                    }
                });

            getUserContentFileRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            getUserContentFileRequest.OnProgress +=
                (r, downloaded, length) =>
                {
                    float pr = downloaded / (float)length * 100f;
                    callback?.Invoke(false, (int)pr, null);
                };

            getUserContentFileRequest.ConnectTimeout = TimeSpan.FromDays(1);
            getUserContentFileRequest.Timeout = TimeSpan.FromDays(1);
            getUserContentFileRequest.Send();
        }

        public void SaveUserContent(int contentTypeId, int assetId, string fileName, string assetName, byte[] fileBytes)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.SaveUserContent)
            {
                Query =
                    ServerConstants.AssetContentTypeKey + "=" + contentTypeId + "&" +
                    ServerConstants.AssetIdKey + "=" + assetId + "&" +
                    ServerConstants.FileNameQuery + fileName
            };

            Debug.Log("SaveUserContent uri = " + uri.Uri);
            Debug.Log("SaveUserContent Query = " + uri.Query);

            var saveContentRequest = new HTTPRequest(uri.Uri, HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new SaveUserContentServerResponseCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal($"SaveUserContent: Error > {response.Message}"));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(new PopupOverlaySignal(true, $"Server Error: {response.Message}",
                                type: PopupOverlayType.TextBox));
                        }
                    }
                    else
                    {
                        _signal.Fire(new SendCrashAnalyticsCommandSignal("SaveUserContent: response is NULL"));
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(new PopupOverlaySignal(true, "Server Error: response is NULL",
                            type: PopupOverlayType.TextBox));
                    }
                });

            saveContentRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            saveContentRequest.SetHeader(ServerConstants.AcceptJsonKey, ServerConstants.AcceptJsonValue);

            string assetFileAttachmentType = string.Empty;
            switch (contentTypeId)
            {
                case UserContentTypeIDConstants.Image:
                    assetFileAttachmentType = ServerConstants.AssetFileAttachmentType_ImagePNG;
                    break;
                case UserContentTypeIDConstants.Video:
                    #if UNITY_WEBGL
                    assetFileAttachmentType = ServerConstants.AssetFileAttachmentType_VideoWEBM;
                    #else
                    assetFileAttachmentType = ServerConstants.AssetFileAttachmentType_VideoMP4;
#endif
                    break;
            }

            saveContentRequest.AddBinaryData(ServerConstants.AssetFileAttachmentKey, fileBytes, assetName,
                assetFileAttachmentType);
            saveContentRequest.OnUploadProgress +=
                (r, downloaded, length) =>
                {
                    float pr = downloaded / (float)length * 100f;
                    _signal.Fire(new SaveUserContentServerResponseSignal(null, false, (int)pr));
                };

            saveContentRequest.ConnectTimeout = TimeSpan.FromDays(1);
            saveContentRequest.Timeout = TimeSpan.FromDays(1);
            saveContentRequest.Send();
        }

        #endregion

        #region Teacher Content

        public void GetTeacherContent()
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetTeacherContent);

            var teacherContentRequest = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateTeacherContentModelCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("GetTeacherContent http response | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            teacherContentRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            teacherContentRequest.SetHeader(ServerConstants.AcceptJsonKey, ServerConstants.AcceptJsonValue);
            teacherContentRequest.Send();
        }
        
        public void GetGenerateInternalAssetLink(string assetId)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GenerateInternalLink)
            {
                Query =
                    '?' + ServerConstants.NoteAssetIdQuery + assetId
            };
            Debug.Log($"ServerService => Get generate internal asset link {uri.Uri}");
            var request = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            var link = response.DataAsText;
                            Debug.Log(link);
                            _signal.Fire(new CopyInternalAssetLinkCommandSignal(link));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(new SendCrashAnalyticsCommandSignal(
                                "GetGenerateInternalAssetLink http response | " +
                                " | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(new CheckInternetConnectionCommandSignal(new LoginClickViewSignal()));
                    }
                });
            
            request.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);

            request.Send();
        }

        #endregion

        #region Login Feide

        private string _tokenApp;

        public void GetLoginFeide()
        {
            _tokenApp = Guid.NewGuid().ToString();

            _signal.Fire(new ShowDebugLogCommandSignal($"Api Url: {_serverSettings.ApiUrl}"));

            Application.OpenURL($"{_serverSettings.ApiUrl}/api/v1/feide/authRequest?token={_tokenApp}&platform=webgl");

            _asyncProcessor.OnApplicationFocusAction += OnAppFocus;
        }

        private void OnAppFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                _asyncProcessor.OnApplicationFocusAction -= OnAppFocus;
                //_signal.Fire<PopupOverlaySignal>();
                FeideResp();
            }
        }

        private void FeideResp()
        {
            var loginRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + "/api/v1/Feide/feideLogin"),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateUserLoginModelSignal(response.DataAsText));
                            _signal.Fire(new ShowDebugLogCommandSignal(
                                $"Get Login IsSuccess = {response.IsSuccess} | response.DataAsText = {response.DataAsText}"));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("Get Login error for user: Feide Login | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(
                            new SendCrashAnalyticsCommandSignal(
                                "Get Login error: response is NULL for user: Feide Login"));
                    }
                });

            loginRequest.AddField("authToken", _tokenApp);
            loginRequest.AddField("platform", ClientPlatform);
            loginRequest.AddField("clientVersion", Application.version);

            loginRequest.Send();
        }

        #endregion

        #region Login Skolon

        public void GetLoginSkolon()
        {
            _tokenApp = Guid.NewGuid().ToString();

            _signal.Fire(new ShowDebugLogCommandSignal($"Api Url: {_serverSettings.ApiUrl}"));

            var platform = string.Empty;
            if (Application.platform==RuntimePlatform.WebGLPlayer)
            {
                platform = "webgl";
            }
            
            Application.OpenURL($"{_serverSettings.ApiUrl}/api/v1/skolon/authRequest?token={_tokenApp}&platform={platform}");

            _asyncProcessor.OnApplicationFocusAction += OnAppFocusSkolon;
        }

        private void OnAppFocusSkolon(bool hasFocus)
        {
            if (hasFocus)
            {
                _asyncProcessor.OnApplicationFocusAction -= OnAppFocusSkolon;
                //_signal.Fire<PopupOverlaySignal>();
                SkolonResp();
            }
        }

        private void SkolonResp()
        {
            Debug.Log("On Focus: Skolon resp");
            var loginRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + "/api/v1/Skolon/skolonLogin"),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        Debug.Log($"Response: {response.DataAsText}");
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateUserLoginModelSignal(response.DataAsText));
                            _signal.Fire(new ShowDebugLogCommandSignal(
                                $"Get Login IsSuccess = {response.IsSuccess} | response.DataAsText = {response.DataAsText}"));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("Get Login error for user: Skolon Login | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(
                            new SendCrashAnalyticsCommandSignal(
                                "Get Login error: response is NULL for user: Skolon Login"));
                    }
                });

            loginRequest.AddField("authToken", _tokenApp);
            loginRequest.AddField("platform", ClientPlatform);
            loginRequest.AddField("clientVersion", Application.version);

            loginRequest.Send();
        }

        #endregion

        #region Login Microsoft

        public void GetLoginMicrosoft()
        {
            _tokenApp = Guid.NewGuid().ToString();
            
            var uri = new UriBuilder($"{_serverSettings.ApiUrl}/Teams/teamsLoginUrl")
            {
                Query = $"appToken={_tokenApp}"
            };
            Debug.Log(uri.Uri);
            var request = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new ShowDebugLogCommandSignal($"Api Url: {_serverSettings.ApiUrl}"));
                            Debug.Log($"Resp is {response.DataAsText}");
                            var url = response.DataAsText;
                            url = url.Replace(" ", "%20");
                            Application.OpenURL(url);
                            _asyncProcessor.OnApplicationFocusAction += OnAppFocusMicrosoft;
                            _signal.Fire(new LoginStateSignal(LoginState.Authenticating));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(new SendCrashAnalyticsCommandSignal(
                                "GetGenerateInternalAssetLink http response | " +
                                " | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(new CheckInternetConnectionCommandSignal(new LoginClickViewSignal()));
                    }
                });

            request.Send();
        }

        private void OnAppFocusMicrosoft(bool hasFocus)
        {
            if (hasFocus)
            {
                _asyncProcessor.OnApplicationFocusAction -= OnAppFocusMicrosoft;
                MicrosoftResp();
            }
        }

        private void MicrosoftResp()
        {
            var loginRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + "/Teams/externalLogin"),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            Debug.Log($"raw data success microsoft login {request.RawData}");
                            _signal.Fire(new CreateUserLoginModelSignal(response.DataAsText));
                            _userLoginModel.IsMicrosoftLogin = true;
                            _signal.Fire(new ShowDebugLogCommandSignal(
                                $"Get Login IsSuccess = {response.IsSuccess} | response.DataAsText = {response.DataAsText}"));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("Get Login error for user: Teams Login | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(
                            new SendCrashAnalyticsCommandSignal(
                                "Get Login error: response is NULL for user: Teams Login"));
                    }
                });

            loginRequest.AddField("authToken", _tokenApp);
            loginRequest.AddField("platform", ClientPlatform);
            loginRequest.AddField("clientVersion", Application.version);

            loginRequest.Send();
        }
        #endregion

        #region Teams

        public void GetTeamsSSO(string token)
        {
            var loginRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + "/Teams/externalLogin"),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateUserLoginModelSignal(response.DataAsText));
                            _signal.Fire(new ShowDebugLogCommandSignal(
                                $"Get Login IsSuccess = {response.IsSuccess} | response.DataAsText = {response.DataAsText}"));
                        }
                        else
                        {
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("Get Login error for user: Teams Login | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                        _signal.Fire(
                            new SendCrashAnalyticsCommandSignal(
                                "Get Login error: response is NULL for user: Teams Login"));
                    }
                });

            loginRequest.AddField("authToken", token);
            loginRequest.AddField("platform", ClientPlatform);
            loginRequest.AddField("isSSO", "true");
            loginRequest.AddField("clientVersion", Application.version);

            loginRequest.Send();
        }
        

        #endregion

        #region Content

        public void GetContent()
        {
            var contentRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.GetContent),
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            // GUIUtility.systemCopyBuffer = response.DataAsText;
                            _signal.Fire(new CreateContentModelSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("GetContent http response | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            contentRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            contentRequest.OnUploadProgress = OnUploadProgress;
            contentRequest.Send();

            void OnUploadProgress(HTTPRequest request, long downloaded, long length)
            {
                _signal.Fire(new LoginStateSignal(LoginState.UploadContentAssetsRequest));
            }
        }

        public void GetActivity(string activityName, int activityId)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetActivity)
                { Query = ServerConstants.ActivityIdQuery + activityId };

            var activityRequest = new HTTPRequest(uri.Uri,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateActivityContentCommandSignal(activityName, response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("GetActivity http response | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            activityRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            activityRequest.AddField(ServerConstants.ClientVersion, Application.version);
            activityRequest.Send();
        }

        #endregion

        #region Favourites

        public void AddToFavourites(int gradeId, int itemId)
        {
            var favoriteRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.AddFavoriteAsset),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(
                                new SaveAddedFavouriteAssetCommandSignal(gradeId, itemId, response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("AddToFavourites http response | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            favoriteRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            favoriteRequest.AddField(ServerConstants.GradeIdField, gradeId.ToString());
            favoriteRequest.AddField(ServerConstants.AssetIdField, itemId.ToString());
            favoriteRequest.Send();
        }

        public void RemoveFromFavourites(int gradeId, int itemId)
        {
            var favoriteRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.RemoveFavoriteAsset),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new RemovedFromFavouritesCommandSignal(gradeId, itemId, response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(new SendCrashAnalyticsCommandSignal("RemoveFromFavourites http response | " +
                                                                             response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            favoriteRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            favoriteRequest.AddField(ServerConstants.GradeIdField, gradeId.ToString());
            favoriteRequest.AddField(ServerConstants.AssetIdField, itemId.ToString());
            favoriteRequest.Send();
        }

        #endregion

        #region Recently Viewed Assets

        public void AddRecentAsset(int gradeId, int assetId)
        {
            var recentlyViewedRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.AddRecentAsset),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new UpdateRecentAssetCommandSignal(gradeId, assetId, response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal(
                                    "AddRecentAsset http response | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            recentlyViewedRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            recentlyViewedRequest.AddField(ServerConstants.GradeIdField, gradeId.ToString());
            recentlyViewedRequest.AddField(ServerConstants.AssetIdField, assetId.ToString());
            recentlyViewedRequest.Send();
        }

        #endregion

        #region Feedback

        public void SendFeedback(FeedbackModel.FeedbackRequest feedbackData)
        {
            var feedbackRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.AddFeedback),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire<FeedbackSentOkResponseCommandSignal>();
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire<FeedbackSentFailResponseCommandSignal>();

                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("SendFeedback http response | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire<FeedbackSentFailResponseCommandSignal>();

                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            feedbackRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            feedbackRequest.AddField(ServerConstants.FeedbackContentIdField, feedbackData.AssetContentId);
            feedbackRequest.AddField(ServerConstants.FeedbackMessageField, feedbackData.Message);

            if (feedbackData.GradeId != null)
            {
                feedbackRequest.AddField(ServerConstants.FeedbackGradeIdField, feedbackData.GradeId);
                feedbackRequest.AddField(ServerConstants.FeedbackSubjectIdField, feedbackData.SubjectId);
                feedbackRequest.AddField(ServerConstants.FeedbackTopicIdField, feedbackData.TopicId);
                feedbackRequest.AddField(ServerConstants.FeedbackSubtopicIdField, feedbackData.SubtopicId);
            }

            feedbackRequest.AddField(ServerConstants.FeedbackStatusField, feedbackData.Status);
            feedbackRequest.Send();
        }

        #endregion

        #region Asset

        public void GetAssetDetails(int assetId, int gradeId)
        {
            string gradeQuery = gradeId >= 0 ? "&" + ServerConstants.GradeIdQuery + gradeId : "";
            Debug.Log("ServerService =>GetAssetDetails = " + gradeQuery);
            Debug.Log("ServerService =>GetAssetDetails - Query = " + ServerConstants.AssetIdDetailsQuery + assetId +
                      gradeQuery);
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetAssetDetails)
            {
                Query = ServerConstants.AssetIdDetailsQuery + assetId + gradeQuery
            };

            Debug.Log("uri = " + uri.Uri);
            var assetDetailsRequest = new HTTPRequest(uri.Uri,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateAssetDetailsCommandSignal(assetId, gradeId, response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("GetAssetDetails http response | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });
            
            assetDetailsRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            assetDetailsRequest.Send();
        }

        public HTTPRequest DownloadAsset(int itemId, string url)
        {
            Debug.Log("itemId = " + itemId);
            Debug.Log("url = " + url);
            var downloadAssetRequest = new HTTPRequest(
                new Uri(url),
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new SaveAssetInCacheCommandSignal(itemId, response.Data));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("DownloadAsset http response | " +
                                                                    response.Message));
                        }
                    }
                });

            downloadAssetRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            downloadAssetRequest.Timeout = TimeSpan.FromSeconds(600);
            downloadAssetRequest.OnProgress += (request, downloaded, length) =>
            {
                _signal.Fire(new UpdateDownloadProgressCommandSignal(itemId, downloaded, length));
            };

            return downloadAssetRequest;
        }

        public void DownloadThumbnail(string itemId, string itemName, string url)
        {
            Debug.Log($"ServerService => DownloadThumbnail = {url} ");
            var thumbnailRequest = new HTTPRequest(
                new Uri(url),
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                        Debug.Log($"DownloadThumbnail {response.DataAsText}");
                        _signal.Fire(new SaveThumbnailInCacheCommandSignal(itemId, itemName, response.Data));
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                        _signal.Fire(new SendCrashAnalyticsCommandSignal(
                            "DownloadThumbnail http response | NULL for itemId: " + itemId + ", url: " + url));
                    }
                });

            thumbnailRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            thumbnailRequest.Timeout = TimeSpan.FromSeconds(600);
            thumbnailRequest.Send();
        }

        public void DownloadThumbnail(string itemId, string itemName, string url, Action<bool, string> isDownload)
        {
            var thumbnailRequest = new HTTPRequest(
                new Uri(url),
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                        _signal.Fire(new SaveThumbnailInCacheCommandSignal(itemId, itemName, response.Data));

                        isDownload.Invoke(true, itemName);
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                        isDownload.Invoke(false, default);
                    }
                });

            thumbnailRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            thumbnailRequest.Timeout = TimeSpan.FromSeconds(600);
            thumbnailRequest.Send();
        }

        public void DownloadBackground(string itemId, string itemName, string url)
        {
            var backgroundRequest = new HTTPRequest(
                new Uri(url),
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                        _signal.Fire(new SaveBackgroundInCacheCommandSignal(itemId, itemName, response.Data));
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                        _signal.Fire(new SendCrashAnalyticsCommandSignal(
                            "DownloadBackground http response | NULL for itemId: " + itemId + ", url: " + url));
                    }
                });

            backgroundRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            backgroundRequest.Timeout = TimeSpan.FromSeconds(600);
            backgroundRequest.Send();
        }

        public void DownloadWarningARImage(string cultureCode, Action<Texture2D> texture)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.WarningAR)
                { Query = ServerConstants.LanguageCodeQuery + cultureCode };

            var textureUrlRequest = new HTTPRequest(uri.Uri,
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                        var resp = JsonConvert.DeserializeObject<WarningARResponse>(response.DataAsText);

                        var textureRequest = new HTTPRequest(
                            new Uri(resp.ImageUrl),
                            (req, textureResponse) =>
                            {
                                if (textureResponse != null && textureResponse.IsSuccess)
                                {
                                    texture?.Invoke(textureResponse.DataAsTexture2D);
                                }
                            });

                        textureRequest.Timeout = TimeSpan.FromSeconds(600);
                        textureRequest.Send();
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                    }
                });

            textureUrlRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            textureUrlRequest.Timeout = TimeSpan.FromSeconds(600);
            textureUrlRequest.Send();
        }

        #endregion

        #region Classification

        public void GetClassificationDetails(int itemId)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.Classification)
                { Query = ServerConstants.ClassificationIdQuery + itemId };

            var getDetailsRequest = new HTTPRequest(uri.Uri,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateClassificationDetailsModelCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("GetClassificationDetails http response | " +
                                                                    response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            getDetailsRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            getDetailsRequest.Send();
        }

        #endregion

        #region Search

        public void GetSearch(string searchValue, string cultureCode)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetSearch)
            {
                Query = ServerConstants.SearchQuery + searchValue + "&" + ServerConstants.SearchLanguageQuery +
                        cultureCode
            };

            var assetDetailsRequest = new HTTPRequest(uri.Uri,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new ParseSearchResultsCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("GetSearch http response | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            assetDetailsRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            assetDetailsRequest.Send();
        }

        #endregion

        #region Meta Data

        public void GetMetaData()
        {
            var metaDataRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.GetMetaData),
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateMetaDataCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("GetMetaData http response | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            metaDataRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            metaDataRequest.Send();
        }

        #endregion

        #region Note

        public void GetAllNotes(int assetId)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetNoteByAsset)
                { Query = ServerConstants.NoteAssetIdQuery + assetId };

            var favoriteRequest = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new GetAllNotesResponseCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("GetAllNotes http response | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            favoriteRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);

            favoriteRequest.Send();
        }

        public void GetNote(int noteId)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion +
                                     _serverSettings.GetNoteByAsset) { Query = ServerConstants.NoteIdQuery + noteId };

            var favoriteRequest = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                    }
                });

            favoriteRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);

            favoriteRequest.Send();
        }

        public void AddNote(string note, int assetId, int gradeId = -1, int subjectId = -1, int topicId = -1,
            int subtopicId = -1)
        {
            var favoriteRequest = new HTTPRequest(
                new Uri(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.AddNote),
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new AddNoteResponseCommandSignal(response.DataAsText));
                        }
                        else
                        {
                            _signal.Fire(new HandleRequestErrorSignal(response));
                            _signal.Fire(new PopupOverlaySignal(false));
                            _signal.Fire(
                                new SendCrashAnalyticsCommandSignal("AddNote http response | " + response.Message));
                        }
                    }
                    else
                    {
                        _signal.Fire(new PopupOverlaySignal(false));
                    }
                });

            favoriteRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            favoriteRequest.AddField(ServerConstants.AssetIdField, assetId.ToString());
            favoriteRequest.AddField(ServerConstants.NoteField, note);

            if (gradeId > 0)
                favoriteRequest.AddField(ServerConstants.NoteGradeIdField, gradeId.ToString());

            if (subjectId > 0)
                favoriteRequest.AddField(ServerConstants.NoteSubjectIdField, subjectId.ToString());

            if (topicId > 0)
                favoriteRequest.AddField(ServerConstants.NoteTopicIdField, topicId.ToString());

            if (subtopicId > 0)
                favoriteRequest.AddField(ServerConstants.NoteSubtopicIdField, subtopicId.ToString());

            favoriteRequest.Send();
        }

        public void UpdateNote(int noteId, int assetId, string note)
        {
            _asyncProcessor.StartCoroutine(UpdateNoteUsingWebRequest(noteId, assetId, note));
        }

        private IEnumerator UpdateNoteUsingWebRequest(int noteId, int assetId, string note)
        {
            var form = new WWWForm();
            form.AddField(ServerConstants.AssetIdField, assetId);
            form.AddField(ServerConstants.NoteField, note);

            using (var www = UnityWebRequest.Post(
                       _serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.UpdateNote + "?" +
                       ServerConstants.NoteIdQuery + noteId, form))
            {
                //todo: error handler
                www.SetRequestHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
                yield return www.SendWebRequest();
            }
        }


        public void DeleteNote(int noteId)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.DeleteNote)
                { Query = ServerConstants.NoteIdQuery + noteId };
            var favoriteRequest = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response != null && response.IsSuccess)
                    {
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                    }
                });

            favoriteRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            favoriteRequest.Send();
        }

        #endregion

        #region Student description

        public void GetDescription(string assetId, string labelId, string title, string cultureCode,
            string descriptionUrl, bool includeTeacherAudioFile, bool isMultiViewSecond = false)
        {
            var studentRequest = new HTTPRequest(
                new Uri(descriptionUrl + "?token=" +
                        _userLoginModel.AuthorizationToken.Replace(ServerConstants.MetaDataTokenUselessWordField,
                            string.Empty)), HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new CreateDescriptionViewCommandSignal(assetId, labelId, title, cultureCode,
                                response.DataAsText, includeTeacherAudioFile, isMultiViewSecond));
                        }
                    }
                });

            studentRequest.Send();
        }

        public void GetLabelDescription(string labelDescriptionUrl, Action<string> callback)
        {
            var studentRequest = new HTTPRequest(
                new Uri(labelDescriptionUrl + "?token=" +
                        _userLoginModel.AuthorizationToken.Replace(ServerConstants.MetaDataTokenUselessWordField,
                            string.Empty)), HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            callback?.Invoke(response.DataAsText);
                        }
                        else
                        {
                            callback?.Invoke(string.Empty);
                        }
                    }
                });

            studentRequest.Send();
        }

        IEnumerator GetAudioClip(string audioFileUrl, Action<AudioClip> callback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            Debug.Log(audioFileUrl);

#if UNITY_WEBGL
            using (var r = UnityWebRequestMultimedia.GetAudioClip(audioFileUrl, AudioType.MPEG))
            {
                foreach (var h in headers)
                {
                    r.SetRequestHeader(h.Key, h.Value);
                }
                yield return r.SendWebRequest();
                if (r.isDone)
                {
                    try
                    {
                        var clip = DownloadHandlerAudioClip.GetContent(r);
                        callback?.Invoke(clip);
                    }
                    catch(Exception ex)
                    {
                        Debug.Log(ex.Message);
                        callback?.Invoke(null);
                    }
                }
                else
                {
                    //todo: Add logic for error handling
                    callback?.Invoke(null);
                }
            }
#else
            using (var www = new WWW(audioFileUrl, null, headers))
            {
                yield return www;
                if (www.isDone)
                {
                    try
                    {
                        AudioClip clip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
                        callback?.Invoke(clip);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        callback?.Invoke(null);
                    }
                }
                else
                {
                    callback?.Invoke(null);
                }
            }
#endif
        }

        public void GetAudioFile(string audioFileUrl, Action<AudioClip> callback)
        {
            _asyncProcessor.StartCoroutine(GetAudioClip(audioFileUrl, callback));
        }

        public void GetLanguageChangedDescription(string assetId, string labelId, string title, string cultureCode,
            string descriptionUrl)
        {
            var studentRequest = new HTTPRequest(
                new Uri(descriptionUrl + "?token=" +
                        _userLoginModel.AuthorizationToken.Replace(ServerConstants.MetaDataTokenUselessWordField,
                            string.Empty)), HTTPMethods.Get,
                (request, response) =>
                {
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            _signal.Fire(new ChangeLanguageDescriptionViewCommandSignal(assetId, labelId, title,
                                cultureCode, response.DataAsText));
                        }
                    }
                });

            studentRequest.Send();
        }

        #endregion

        #region CheckTerms

        public void AcceptTerm(bool isAccept = true, bool isShowLog = false)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.AcceptTerm)
            {
                Query = ServerConstants.DeviceIdentifierQuery + DeviceInfo.GetDeviceUID() + $"&termAccepted={isAccept}"
            };

            var validateTermRequest = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Post,
                (request, response) =>
                {
                    if (response.IsSuccess)
                        _userLoginModel.TermAccepted = isAccept;
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                    }

                    if (isShowLog)
                    {
                        Debug.Log($"AcceptTerm = {isAccept} > {response.DataAsText}");
                    }
                });

            validateTermRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);
            validateTermRequest.Send();
        }

        public void ValidateTerm(Action<bool> isAccepted)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.ValidateTerm)
                { Query = ServerConstants.DeviceIdentifierQuery + DeviceInfo.GetDeviceUID() };

            var validateTermRequest = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response.IsSuccess)
                    {
                        var resp = JsonConvert.DeserializeObject<ResponseBase>(response.DataAsText);
                        isAccepted?.Invoke(resp.Success);
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                    }
                });

            validateTermRequest.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);

            validateTermRequest.Send();
        }

        public void GetTermText(string cultureCode, Action<string> callback)
        {
            var uri = new UriBuilder(_serverSettings.ApiUrl + _serverSettings.ApiVersion + _serverSettings.TermText)
                { Query = ServerConstants.LanguageQuery + cultureCode };

            var getTermText = new HTTPRequest(
                uri.Uri,
                HTTPMethods.Get,
                (request, response) =>
                {
                    if (response.IsSuccess)
                    {
                        var resp = JsonConvert.DeserializeObject<TermTextResponse>(response.DataAsText);
                        callback?.Invoke(resp.PolictAndTerm);
                    }
                    else
                    {
                        _signal.Fire(new HandleRequestErrorSignal(response));
                    }
                });

            getTermText.SetHeader(ServerConstants.AuthorizationToken, _userLoginModel.AuthorizationToken);

            getTermText.Send();
        }

        #endregion
    }
}