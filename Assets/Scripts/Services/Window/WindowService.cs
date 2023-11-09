using System;
using System.Collections.Generic;
using TDL.Constants;
using UnityEngine;

namespace TDL.Services
{
    public class WindowService : IWindowService
    {
        private Dictionary<Enum, GameObject> _windowList = new Dictionary<Enum, GameObject>();
        private Enum _currentWindow;

        public WindowService()
        {
#if UNITY_STANDALONE
        SetFullScreenInNativeResolution();
#endif
        }
    
        private void SetFullScreenInNativeResolution()
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }

        public void AddWindow(Enum key, GameObject window)
        {
            if (!_windowList.ContainsKey(key))
            {
                _windowList.Add(key, window);
            }
        }

        public void ShowWindow(Enum key)
        {
            HideCurrentWindow();

            if (_windowList.ContainsKey(key))
            {
                _currentWindow = key;
                if (_windowList[key] != null)
                {
                    _windowList[key].SetActive(true);
                }
            }
        }

        private void HideCurrentWindow()
        {
            if (_currentWindow != null && _windowList.ContainsKey(_currentWindow))
            {
                if (_windowList[_currentWindow] != null)
                {
                    _windowList[_currentWindow].SetActive(false);
                }
            }
        }

        public WindowConstants GetCurrentWindow()
        {
            return (WindowConstants)_currentWindow;
        }

        public bool IsCurrentWindow(WindowConstants windowConstants)
        {
            return (WindowConstants)_currentWindow == windowConstants;
        }
    }   
}