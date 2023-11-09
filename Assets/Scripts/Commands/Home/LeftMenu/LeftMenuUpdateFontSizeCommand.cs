using System;
using TDL.Constants;
using TDL.Models;
using TMPro;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class LeftMenuUpdateFontSizeCommand : ICommand
    {
        [Inject] private readonly HomeModel _homeModel;
        [Inject] private readonly AccessibilityModel _accessibilityModel;
        
        public void Execute()
        {
            if (_accessibilityModel.MainAppFontSizeScaler != AccessibilityConstants.FontSizeDefaultScaleFactor)
            {
                foreach (Transform child in _homeModel.LeftMenuContent)
                {
                    var textElements = child.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
                    if (textElements != null)
                    {
                        foreach (var textElement in textElements)
                        {
                            textElement.fontSize = (int) Math.Round(textElement.fontSize / _accessibilityModel.MainAppFontSizeScaler);
                        }
                    }
                }   
            }
        }
    }   
}