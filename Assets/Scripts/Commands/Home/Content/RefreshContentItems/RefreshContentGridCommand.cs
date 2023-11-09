using System.Collections;
using TDL.Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Commands
{
    public class RefreshContentGridCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private AsyncProcessorService _asyncProcessor;

        public void Execute()
        {
            _homeModel.MainContent.anchoredPosition = Vector2.zero;
            _asyncProcessor.StartCoroutine(RunWithDelay());
        }

        private IEnumerator RunWithDelay()
        {
            yield return null;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_homeModel.MainContent);
        }
    }
}