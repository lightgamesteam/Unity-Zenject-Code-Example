using TDL.Constants;
using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class SaveCurrentLanguageToCacheCommand : ICommand
    {
        [Inject] private LocalizationModel _local;
        
        public void Execute()
        {
            PlayerPrefs.SetString(LocalizationConstants.ChosenLanguage, _local.CurrentLanguageCultureCode);
        }
    }
}