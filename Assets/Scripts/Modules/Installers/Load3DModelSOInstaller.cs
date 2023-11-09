using System;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Load3DModelSOInstaller", menuName = "Installers/Load3DModelSOInstaller")]
public class Load3DModelSOInstaller : ScriptableObjectInstaller
{
    public ScreenContainer ScreenContainer;

    [Header(" 3D Model View Camera Settings")] 
    public Camera3DModelSettings camera3DModelSettings;
    
    public override void InstallBindings()
    {
        Container.BindInstance(ScreenContainer);

        if (DeviceInfo.IsPCInterface())
        {
            Container.BindInstance(ScreenContainer.Label3DPrefab);
        }
        else
        {
            Container.BindInstance(ScreenContainer.Label3DPrefabMobile);
        }

        Container.BindInstance(camera3DModelSettings);
    }
}

[Serializable]
public class Camera3DModelSettings
{
    [SerializeField] private float rotationSpeed = 5f;
    public float RotationSpeed
    {
        get
        {
            if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                return rotationSpeed * 3;
            }
 
            return rotationSpeed;
        }
        
        set => rotationSpeed = value;
    }

    public float Distance = 8f;

}