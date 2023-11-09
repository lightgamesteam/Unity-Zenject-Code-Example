using UnityEngine;

[CreateAssetMenu(fileName = "ScreenContainer", menuName = "Installers/ScreenContainer")]
public class ScreenContainer : ScriptableObject
{
   [Header("Screens")]
   public GameObject screenHome;
   public GameObject screenPC;
   public GameObject screenMobile;
   public GameObject multiView;
      
   [Header("3D Label")]
   public GameObject Label3DPrefab;
   public GameObject Label3DPrefabMobile;
   
   [Header("AR")]
   public GameObject VuforiaARCamera;
   public GameObject VuforiaPlaneFinder;
   public GameObject VuforiaPlaneFinder_2;
}