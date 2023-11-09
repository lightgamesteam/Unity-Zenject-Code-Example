using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Zenject;

namespace Commands
{
#if UNITY_WEBGL && !UNITY_EDITOR
    public class SetCurrentHostCommand : ICommand
    {
        [Inject] private readonly ApplicationSettingsInstaller.ServerSettings _serverSettings;

        private const string STAGE_AUTH = "https://auth.stage.3dl.no";
        private const string DEV_AUTH = "https://auth.dev.3dl.no";
        private const string PROD_AUTH = "https://auth.3dl.no";
        
        private const string FALLBACK_HOST = "https://auth.dev.3dl.no"; 

        private const string NORWAY_STAGE_HOST = "3dlstagenwe.z1.web.core.windows.net";
        private const string NORWAY_DEV_HOST = "3dldevnwe.z1.web.core.windows.net";
        private const string NORWAY_PROD_HOST = "3dlnwe.z1.web.core.windows.net";
        
        private const string NORWAY_DEV_TEAMS_HOST = "teams.dev.3dl.no";
        private const string NORWAY_STAGE_TEAMS_HOST = "teams.stage.3dl.no";
        private const string NORWAY_PROD_TEAMS_HOST = "teams.3dl.no";
        
        private const string NORWAY_DEV_WEB_HOST = "web.dev.3dl.no";
        private const string NORWAY_STAGE_WEB_HOST = "web.stage.3dl.no";
        private const string NORWAY_PROD_WEB_HOST = "web.3dl.no";

        private static Dictionary<string, string> _norwayServers = new Dictionary<string, string>
        {
            [NORWAY_DEV_HOST] = DEV_AUTH,
            [NORWAY_STAGE_HOST] = STAGE_AUTH,
            [NORWAY_PROD_HOST] = PROD_AUTH,
            
            [NORWAY_DEV_TEAMS_HOST] = DEV_AUTH,
            [NORWAY_STAGE_TEAMS_HOST] = STAGE_AUTH,
            [NORWAY_PROD_TEAMS_HOST] = PROD_AUTH,
            
            [NORWAY_DEV_WEB_HOST] = DEV_AUTH,
            [NORWAY_STAGE_WEB_HOST] = STAGE_AUTH,
            [NORWAY_PROD_WEB_HOST] = PROD_AUTH,
        };

        public void Execute()
        {
            Debug.Log("Trying to retrieve current host.");

            var host = GetCurrentHost();

            if (_norwayServers.TryGetValue(host, out var value))
            {
                _serverSettings.AuthUrl = value;
                return;
            }

            _serverSettings.AuthUrl = FALLBACK_HOST;
            Debug.Log($"Invalid host '{host}'! fallback: '{FALLBACK_HOST}'");
        }

        [DllImport("__Internal")]
        private static extern string GetCurrentHost();
    }
#endif
}