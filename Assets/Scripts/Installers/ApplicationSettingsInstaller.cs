using System;
using TDL.Constants;
using UnityEngine;
using Zenject;
using TDL.Core;

[CreateAssetMenu(fileName = "ApplicationSettings", menuName = "Installers/ApplicationSettingsInstaller")]
public class ApplicationSettingsInstaller : ScriptableObjectInstaller<ApplicationSettingsInstaller>
{
    public GameInstaller.DebugSettings Debug;
    public ServerSettings Server;
    public GameInstaller.ScreenPrefabs ScreenPrefabs;
    public GameInstaller.UIElementPrefabs UIElementPrefabs;
    public AssetItemResources AssetItemResource;

    [Serializable]
    public class ServerSettings
    {
        public PlayerPlatformConstants EditorPlatform = PlayerPlatformConstants.Windows;
        public bool UseApiTestUrl;

        public string apiUrl;
        public string ApiUrl
        {
            get { return UseApiTestUrl ? ApiTestUrl : apiUrl; }

            set => apiUrl = value;
        }

        public string GuestLogin;
        public string GuestPassword;
        public string AuthUrl;
        public string ApiTestUrl;
        public string ApiVersion;
        public string ConsoleLoginUrl;
        public string ChangePasswordUrl;
        public string GetAvailableLanguages;
        public string GetResources = "/Resource/getresources";
        public string GetLogin;
        public string GenerateInternalLink = "/Content/getAssetShareId?";
        public string FeideAuthRequest = "/feide/authRequest?token=";
        public string FeideLogin = "/feideLogin";
        public string TeasmSSO = "/teams/teamsSSO";
        public string GetContent;
        public string GetActivity;
        public string GetAssetDetails;
        public string AddFavoriteAsset;
        public string RemoveFavoriteAsset;
        public string AddRecentAsset;
        public string AddFeedback;
        public string Classification;
        public string GetSearch;
        public string GetMetaData;
        public string GetTeacherContent = "/Content/getTeacherContent";
        public string GetUserContent = "/Content/getUserContent";
        public string GetUserContentFile = "/Content/usercontentfile";
        public string DeleteUserContent = "/Content/deleteUserContent";
        public string SaveUserContent = "/Content/saveUserContent";
        public string AddNote = "/Note/AddNote";
        public string GetNoteByAsset = "/Note/GetNoteByAsset";
        public string GetNote = "/Note/GetNote";
        public string UpdateNote = "/Note/UpdateNote";
        public string DeleteNote = "/Note/DeleteNote";
        public string ValidateTerm = "/ValidateTerm";
        public string AcceptTerm = "/AcceptTerm";
        public string TermText = "/Resource/term";
        public string WarningAR = "/Resource/vrwarningimage";
    }
    
    [Serializable]
    public class AssetItemResources
    {
        public Sprite Type3DModel;
        public Sprite Type3DVideo;
        public Sprite Type2DVideo;
        public Sprite TypeMultilayered;
        public Sprite Type360Model;
        public Sprite TypeRiggedModel;
        public Sprite TypeModule;
        public Sprite TypeImage;
    }

    public override void InstallBindings()
    {
        Container.BindInstance(Debug);
        Container.BindInstance(Server);
        Container.BindInstance(ScreenPrefabs);
        Container.BindInstance(UIElementPrefabs);
        Container.BindInstance(AssetItemResource);
    }
}