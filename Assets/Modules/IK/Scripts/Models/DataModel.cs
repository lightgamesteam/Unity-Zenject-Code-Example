using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Module.IK
{

    public class DataModel
    {
        static public Dictionary<int, Camera> MainCamera = new Dictionary<int, Camera>();
        static public Dictionary<int, SmoothOrbitCam> SmoothOrbitCam = new  Dictionary<int, SmoothOrbitCam>();
        static public Dictionary<int, SmoothOrbitCamViewportInput> SmoothOrbitCamViewportInput = new Dictionary<int, SmoothOrbitCamViewportInput>();
        static public Transform Vertex;
        static public Transform Lens;
        static public Transform MarksF;
        static public Transform Marks2F;
        static public Transform MarksF_Mirror;
        static public Transform Marks2F_Mirror;
        static public LineRenderer LineMarks;
        static public LineRenderer LineRays;
        static public Transform MirrorObjects;
        static public Transform SourceObjects;
        static public Transform BoundingBoxTran;
        static public Transform RaysObjectsTran;
        static public Slider SliderFocus;
        static public ScrollRect ScrollListObjets;
        static public MonoBehaviour MonoBehaviour;
        static public Mesh ConvexLensMesh;
        static public Mesh ConcaveLensMesh;
        static public Button ItemObject;
        static public GameObject MeshObject;
        static public RectTransform PanelUI;
        static public InputField InputFocus;
        static public Canvas Canvas;
        static public GameObject TargetIK;
        static public GameObject ElbowIK;
        static public Transform NumbersGrid;
        static public TextMeshProUGUI FocusText;
        static public TextMeshProUGUI TextTypeLens;
        static public TextMesh DistanceSourceObject;
        static public TextMesh DistanceMirrorObject;
        static public Camera VuforiaCamera;
        

        static public bool IsConcaveLens;
        static public bool IsClick;
        static public bool IsOpenUIPanel;
        static public bool IsMoveCamera = true;
        static public bool DynamicGrid = true;
        static public bool VisibilityRays = true;

        static public float CurentTimeLimitToMoveCamera = 0f;
        static public float Focus = 3f;
        static public float DefaultFocus = 3f;
        static public string DefaultNameModels;
        static public string DefaultPathModels;
        static public Texture2D DefaultUnknownImage;
        
        static public Vector3 DefaultCameraPosition;
        static public Quaternion DefaultCameraRotation;

        static public List<Transform> ListPoints;
        static public Dictionary<GameObject, GameObject> MirrorModels;
        static public Dictionary<Transform, Dictionary<string, Transform>> Vertexs;
        static public Dictionary<Transform, Vector3[]> OriginalVertices;
        static public Dictionary<Transform, Vector3[]> DisplacedVertices;
        static public Dictionary<Transform, Vector3[]> DisplacedMirrorVertices;
        static public Dictionary<Transform, Vector3[]> BoundingBoxOriginalVertices;
        static public Dictionary<Transform, Vector3[]> BoundingBoxDisplacedVertices;
        static public Dictionary<Transform, Vector3> MinVertices;
        static public Dictionary<Transform, Vector3> MaxVertices;
        static public Dictionary<Transform, Mesh> BoundingBox;
        static public Dictionary<Transform, LineRenderer> RaysCenter;
        static public Dictionary<Transform, LineRenderer> RaysUp;
        static public Dictionary<Transform, LineRenderer> RaysDown;
    }

}