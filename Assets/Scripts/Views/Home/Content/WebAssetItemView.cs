using TDL.Constants;
using TDL.Core;
using TDL.Models;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class WebAssetItemView : AssetItemView
    {
        public new class Pool : MonoMemoryPool<Transform, WebAssetItemView>
        {
            protected override void Reinitialize(Transform viewParent, WebAssetItemView view)
            {
                if (view.transform.parent == null)
                {
                    view.transform.SetParent(viewParent, false);
                }

                view.ResetView();
            }
        }
    }
}