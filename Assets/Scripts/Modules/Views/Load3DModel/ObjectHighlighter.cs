using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Module.Core.Attributes;
using TDL.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectHighlighter : MonoBehaviour
{
	public static Action<bool, ObjectHighlighter> SelectModelPart = delegate {  };
	public static Action<string, GameObject> ttsAction = delegate {  };

	[ShowOnly] public int ID;
	public bool interactable = true;
	private float highlightSpeed = 0.5f;

	[SerializeField] private Toggle myToggle;
	private TextMeshProUGUI myToggleText;
	private IEnumerator waitTTS;

	private Color colorStart;
	private Color colorEnd;
	private Color xRayColor;

	private Renderer render;
	private bool isMouseEnter = false;
	private Vector2 clickMousePosition;
	private float mouseClickDistance => DeviceInfo.IsMobile() ? 10f : 0.5f;
	private bool isTouchControl = false;

	private Outline3D outline;
	[SerializeField] private List<Material> highlightMaterials = new List<Material>();
	
	private const string ShaderName = "Highlight/XRay";
	private const string ShaderPropertyHighlightColor = "_HighlightColor";
	private const string ShaderPropertyXRAyColor = "_XRAyColor";
	private const string ShaderPropertyZTestXRay = 	"_ZTestXRay";

	private void Awake()
	{
		if (gameObject.HasComponent(out SkinnedMeshRenderer smr))
		{
			smr.updateWhenOffscreen = true;
		}
	}

	public void SetColor(Color color)
	{
		xRayColor = color;
		colorStart = colorEnd = color;
		colorStart.a = 0f;
		colorEnd.a = 0.8f;
		
		AddMaterial(colorStart);
	}
	
    public void TTS()
    {
	    if(interactable)
			ttsAction?.Invoke(myToggleText.text, gameObject);
    }

	public void SetToggle(Toggle toggle)
	{
		myToggle = toggle;
		myToggleText = myToggle.GetComponentInChildren<TextMeshProUGUI>();
		myToggle.onValueChanged.AddListener((value) =>
		{
			SetHighlightColor(value);
			SelectModelPart.Invoke(value, this);
		});
	}

	public Toggle GetMyToggle()
	{
		return myToggle;
	}

	public void ClickToggle()
	{
		myToggle.isOn = !myToggle.isOn;
		
		if(!myToggle.isOn)
			SetOutline(false);
	}

	private void AddMaterial(Color color)
	{
		if(gameObject.GetComponent<Renderer>() == null)
			return;

		Renderer renderer = gameObject.GetComponent<Renderer>();

		Mesh mesh = gameObject.GetMesh();
		if (mesh.subMeshCount > 1)
		{
			List<GameObject> allGO = new List<GameObject>(); 
			allGO.AddRange(gameObject.ExtractSubMeshes());

			renderer.enabled = false;

			foreach (GameObject o in allGO)
			{
				Renderer r = o.GetComponent<Renderer>();

				var mat = AddMaterial(color, r);
				highlightMaterials.Add(mat);
			}
		}
		else
		{
			var mat = AddMaterial(color, renderer);
			highlightMaterials.Add(mat);
		}
	}

	private Material AddMaterial(Color color, Renderer renderer)
	{
		var materials = renderer.sharedMaterials.ToList();

		Material highlightMaterial = new Material(Shader.Find(ShaderName));
		highlightMaterial.SetColor(ShaderPropertyHighlightColor, color);
		highlightMaterial.SetColor(ShaderPropertyXRAyColor, xRayColor);

		materials.Add(highlightMaterial);

		renderer.materials = materials.ToArray();
		
		return highlightMaterial;
	}

	public bool OutlineIsEnabled()
	{
		return highlightMaterials[0].GetFloat(ShaderPropertyZTestXRay).Equals(8);
	}

	public void SetOutline(bool isActive)
	{
		foreach (Material highlightMaterial in highlightMaterials)
		{
			if (isActive)
			{
				highlightMaterial.SetFloat(ShaderPropertyZTestXRay, 8);
			}
			else
			{
				highlightMaterial.SetFloat(ShaderPropertyZTestXRay, 1);
			}
		}
	}
	
	public void SetHighlightColor(bool isHighlighted)
	{
		foreach (Material highlightMaterial in highlightMaterials)
		{
			if (isHighlighted)
				highlightMaterial.DOColor(colorEnd, ShaderPropertyHighlightColor, highlightSpeed).SetEase(Ease.OutExpo);
			else
				highlightMaterial.DOColor(colorStart, ShaderPropertyHighlightColor, highlightSpeed).SetEase(Ease.OutExpo);
		}
	}
	
	public void OffHighlightMaterial(bool findMaterials = false)
	{
		if (!findMaterials)
		{
			foreach (Material highlightMaterial in highlightMaterials)
			{
				highlightMaterial.SetColor(ShaderPropertyHighlightColor, colorStart);
			}
		}
		else
		{
			if (gameObject.HasComponent(out SkinnedMeshRenderer smr))
			{
				TurnOff(smr.materials);
			}
			else if (gameObject.HasComponent(out MeshRenderer meshRenderer))
			{
				TurnOff(meshRenderer.materials);

				gameObject.GetAllComponentsInChildren<MeshRenderer>().ForEach(mr =>
				{
					TurnOff(mr.materials);
				});
			}

			void TurnOff(Material[] mats)
			{
				mats.ToList().ForEach(m =>
				{
					if (m.shader == Shader.Find(ShaderName))
					{
						m.SetColor(ShaderPropertyHighlightColor, colorStart);
					}
				});
			}
		}
	}
	
	public void OnHighlightMaterial()
	{
		foreach (Material highlightMaterial in highlightMaterials)
		{
			highlightMaterial.SetColor(ShaderPropertyHighlightColor, colorEnd);
		}
	}

	void Update()
	{
		if (touchDown)
		{
			timer += Time.deltaTime;
		}
	}
	
	private bool touchDown = false;
	private float timer = 0;
	private float waitTouch = 0.4f;
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;
        
		if (Input.touchCount > 0)
		{
			if (Input.touchCount == 1)
			{
				clickMousePosition = Input.GetTouch(0).position;
				timer = 0;
				touchDown = isMouseEnter = true;
				
				if (!myToggle.isOn)
					SetHighlightColor(true);

				HighlightLabel(true);
			}
		}
		else
		{
			clickMousePosition = Input.mousePosition;
		}
	}

	private void OnMouseUpAsButton()
	{
		if (EventSystem.current.IsPointerOverGameObject() || !interactable)
			return;

		if (Input.touchCount > 0)
		{
			MouseExit();
			
			if (Input.touchCount == 1)
			{
				if(EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
					return;

				if (touchDown)
				{
					if (timer <= waitTouch && Vector2.Distance(clickMousePosition, Input.GetTouch(0).position) <= mouseClickDistance)
					{
						ClickToggle();
						touchDown = false;

						if (waitTTS != null)
							StopCoroutine(waitTTS);
						TTS();
					}
					else
					{
						touchDown = false;
					}
				}
			}
		}
		else
		{
			if (Vector2.Distance(clickMousePosition, Input.mousePosition) <= mouseClickDistance)
			{
				ClickToggle();
				if(waitTTS != null)
					StopCoroutine(waitTTS);
			    TTS();
			}
		}
	}

	public void OnMouseEnter()
	{
		if (EventSystem.current.IsPointerOverGameObject() || !interactable || Input.touchCount > 0 || isTouchControl)
			return;

		if (!isMouseEnter)
		{
			MouseEnter();
		}
	}

	private void MouseEnter()
	{
		waitTTS = WaitTTS();
		StartCoroutine(waitTTS);
		if (!myToggle.isOn)
			SetHighlightColor(true);

		HighlightLabel(true);
		isMouseEnter = true;
	}

	private void OnMouseExit()
	{
		if (Input.touchCount > 0)
			return;

		if(waitTTS != null)
			StopCoroutine(waitTTS);

		MouseExit();
	}
	
	private IEnumerator WaitTTS()
	{
		yield return new WaitForSeconds(1f);
		TTS();
	}

	public void HighlightLabel(bool value)
	{
		List<LabelData> objects = FindComponentExtension.GetAllInSceneOnLayer<LabelData>(SceneNameConstants.Module3DModel, gameObject.layer);

		LabelData oh = objects.Find(o => o.modelPartName  == transform.name  && o.ID == ID);
        
		oh?.EnableHighlight(value);
	}

	private void MouseExit()
	{
		if (isMouseEnter)
		{
			if (!myToggle.isOn)
				SetHighlightColor(false);

			HighlightLabel(false);
			isMouseEnter = false;
		}
	}

	private Vector2 lastMousePos;
	void LateUpdate()
	{
		if (!Input.touchSupported) return;
		
		if (Input.touchCount > 0)
		{
			isTouchControl = true;
			lastMousePos = Input.mousePosition;
		}
		else
		{
			if(!lastMousePos.Equals(Input.mousePosition))
				isTouchControl = false;
		}
		
		if(!isMouseEnter && touchDown) 
			MouseExit();
	}
}