using System;

namespace TDL.Models
{
    public class PopupModel
    {
        public Action OnResetChanged;

        #region Internet Connection

        public Action<bool> OnShowInternetConnectionChanged;
        public ISignal InternetConnectionRetrySource { get; set; }

        private bool _showInternetConnection;
        public bool ShowInternetConnection
        {
            get => _showInternetConnection;
            set
            {
                if (_showInternetConnection == value) return;
                _showInternetConnection = value;
                OnShowInternetConnectionChanged?.Invoke(_showInternetConnection);
            }
        }
        
        #endregion

        public void Reset()
        {
            OnResetChanged?.Invoke();
        }
    }
}