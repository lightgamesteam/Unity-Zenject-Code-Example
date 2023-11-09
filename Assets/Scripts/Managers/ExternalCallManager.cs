using System;
using UnityEngine;

namespace Managers
{
    public class ExternalCallManager: MonoBehaviour
    {
        public bool IsTeams { get; private set; }
        
        public string SSOAuthToken { get; private set; }

        public Action OnSSOSet;
        
        public bool IsSSOSet { get; private set; }
        
        public void SetIsTeams(string isTeams)
        {
            IsTeams = isTeams == "true";
            Debug.Log($"Unity: Teams is set, is teams: {IsTeams}");
        }
        
        public void SetSSOTeamsToken(string token)
        {
            IsSSOSet = true;
            SSOAuthToken = token;
            OnSSOSet?.Invoke();
            Debug.Log($"Unity: Token is set: {SSOAuthToken}");
        }
    }
}