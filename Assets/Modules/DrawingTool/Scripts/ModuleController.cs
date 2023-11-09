using DrawingTool.States;
using Module.Core;
using Module.Core.Data;
using TDL.Models;
using TDL.Services;
using UnityEngine;
using Zenject;
using Debug = Module.Debug;

namespace DrawingTool.Core {
    public class ModuleController : ModuleControllerBase {
        #region Inject

        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly ICacheService _cacheService;

        #endregion

        #region Variables

        public ActivityDataObject ActivityData { get; private set; }

        private static ModuleController _instance;

        #endregion

        public new static ModuleController Instance {
            get {
                if (_instance == null) {
                    _instance = (ModuleController) FindObjectOfType(typeof(ModuleController));
                    if (FindObjectsOfType(typeof(ModuleController)).Length > 1) {
                        Debug.LogWarning("[Singleton] Something went really wrong " +
                                         " - there should never be more than 1 singleton!" +
                                         " Reopening the scene might fix it.");
                        return _instance;
                    }
                    if (_instance == null) {
                        var obj = new GameObject(typeof(ModuleController) + "_Singleton");
                        _instance = obj.AddComponent<ModuleController>();
                        Debug.Log("Create singleton: " + typeof(ModuleController));
                    }
                }
                return _instance;
            }
        }

        public override void Initialize() {
            base.Initialize();
            CreateActivityData();
            ActiveState<StateDrawing>();
        }
        
        #region Activity Data

        private void CreateActivityData() { }

        #endregion
    }
}
