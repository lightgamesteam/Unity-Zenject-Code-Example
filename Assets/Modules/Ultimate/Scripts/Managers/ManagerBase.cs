using Module;
using UnityEngine;
using Zenject;

namespace TDL.Modules.Ultimate.Core.Managers {
    public abstract class ManagerBase {
        protected DiContainer Container;
        
        [Inject]
        protected virtual void Construct(DiContainer container) {
            Container = container;
            this.Log("registered", Color.cyan, Color.yellow);
        }
    }
}