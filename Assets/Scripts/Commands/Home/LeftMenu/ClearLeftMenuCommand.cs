using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class ClearLeftMenuCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            foreach (Transform child in _homeModel.LeftMenuContent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}