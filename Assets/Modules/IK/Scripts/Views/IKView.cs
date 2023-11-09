using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module.IK
{
    public class IKView : MonoBehaviour
    {
        private Transform _elbowIK;
        private Transform _targetIK;
        private Transform _lineIK;
        private Dictionary<Transform, Transform> _IKbones = new Dictionary<Transform, Transform>();
        private LineRenderer _lineRenderer;
        public static Action UpdateMeshColliderAction = delegate {  };
        private void Start()
        {
            UpdateMeshColliderAction += UpdateMeshCollider;
            DataModel.SmoothOrbitCam[gameObject.layer] =  gameObject.GetInSceneOnLayer<SmoothOrbitCam>();
            DataModel.SmoothOrbitCamViewportInput[gameObject.layer] = gameObject.GetInSceneOnLayer<SmoothOrbitCamViewportInput>();
            DataModel.MainCamera[gameObject.layer] = DataModel.SmoothOrbitCam[gameObject.layer].GetMyCamera();
            _elbowIK = Resources.Load<Transform>("Prefabs/ElbowIK");
            _targetIK = Resources.Load<Transform>("Prefabs/TargetIK");
            _lineIK = Resources.Load<Transform>("Prefabs/LineIK");
            _lineRenderer = Instantiate(_lineIK).GetComponent<LineRenderer>();
            SetEnableLineIK(false);
            InitIK();
            UpdateMeshCollider();
        }

        private void InitIK()
        {
            var rootBone = FindRootBone(transform);
            if (rootBone != null)
            {
                FindDeepChild(rootBone);
            }
        }

        public void SetEnableLineIK(bool value)
        {
            _lineRenderer.enabled = value;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RelocatorActivator();
            }

            if (Input.GetMouseButtonUp(0) && isRelocatorActivated)
            {
                isRelocatorActivated = false;
                UpdateMeshCollider();
            }
        }
        
        Vector3 modelScale = Vector3.one;
        private void UpdateMeshCollider()
        {
            gameObject.GetAllInScene<SkinnedMeshRenderer>().ForEach(meshRenderer =>
            {
                modelScale = transform.localScale;
                Mesh newColliderMesh = new Mesh();
                MemoryManager.Instance?.AddObjectToMemoryManager(gameObject.scene.name, newColliderMesh);
                
                if (meshRenderer.gameObject.HasComponent(out MeshCollider coll))
                {
                    transform.localScale = Vector3.one;
                    meshRenderer.BakeMesh(newColliderMesh);
                    coll.sharedMesh = null;
                    coll.sharedMesh = newColliderMesh;
                    transform.localScale = modelScale;
                }
            });
        }

        private bool isRelocatorActivated = false;
        private void RelocatorActivator()
        {
            Camera camera = MouseExtension.GetDepthCameraForMousePosition(5);
        
            if(camera == null)
                return;
            
            int layerMask = LayerMask.GetMask("UI");

            Ray ray = camera.ScreenPointToRay (Input.mousePosition);

            List<RaycastHit> hits = new List<RaycastHit>();
            hits.AddRange(Physics.RaycastAll(ray, layerMask));
            hits.RemoveAll(h => !h.collider.gameObject.HasComponent<RelocatorView>());
            hits.RemoveAll(h => !h.collider.gameObject.layer.Equals(camera.gameObject.layer));
            hits.Sort((a, b) => a.distance.CompareTo(b.distance));

            if (hits.Count > 0)
            {
                if(hits[0].collider.gameObject.HasComponent(out RelocatorView rlv))
                {
                    if (rlv.gameObject.layer == camera.gameObject.layer)
                    {
                        isRelocatorActivated = true;
                        rlv.OnMouseDownIK();
                    }
                }
            }
        }

        private Transform FindRootBone(Transform aParent)
        {
            var skinnedMeshRenderer = aParent.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                return skinnedMeshRenderer.rootBone;
            }
            return null;
        }

        private void AddFastIKFabric(Transform bone, Transform parent, int countBone)
        {
            var targetIK = Instantiate(_targetIK.gameObject, parent);
            var material_targetIK = Instantiate(targetIK.GetComponent<MeshRenderer>().material);
            //var elbowIK = Instantiate(_elbowIK.gameObject, parent);
            //var material_targetIK = targetIK.GetComponent<MeshRenderer>().material;
            targetIK.GetComponent<MeshRenderer>().material = material_targetIK;
            var relocatorView = targetIK.GetComponent<RelocatorView>();
            var outline3D_targetIK = targetIK.GetComponent<Outline3D>();
            var fastIKFabric = bone.gameObject.AddComponent<FastIKFabric>();
            relocatorView._IKView = this;
            fastIKFabric.ChainLength = countBone;
            fastIKFabric.Target = targetIK.transform;
            //FastIKFabric.Pole = elbowIK.transform;
            targetIK.transform.position = bone.position;
            var random_color = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), 0.2f);
            material_targetIK.SetColor("_Color", random_color);
            material_targetIK.SetColor("_EmissionColor", random_color);
            outline3D_targetIK.OutlineColor = random_color;
            _IKbones.Add(targetIK.transform, bone);
            fastIKFabric.Init();
        }

        private Transform FindDeepChild(Transform rootBone)
        {
            var bones = rootBone.GetComponentsInChildren<Transform>();
            for (int j = 0; j < bones.Length; j++)
            {
                var bone = bones[j];
                if (bone.name.LastIndexOf("_RIG") > 0)
                {
                    var child = bone.GetChild(0);
                    var index = 1;
                    while (child != null)
                    {
                        if (child.childCount > 0 && child.name.LastIndexOf("_RIG") <= 0)
                        {
                            child = child.GetChild(0);
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    AddFastIKFabric(child, rootBone.parent, index);
                }
            }
            
            return null;
        }

        public void UpdateIKPosition(Transform transform = null)
        {
            if (transform != null)
            {
                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, _IKbones[transform].position);
            }

            foreach (var IKbone in _IKbones)
            {
                if (transform != IKbone.Key)
                {
                    IKbone.Key.position = IKbone.Value.position;
                }
            }
        }
    }
}