using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using XDPaint.Core;
using XDPaint.Core.PaintObject.Base;
using XDPaint.Tools;

namespace XDPaint.Editor
{
	[CustomEditor(typeof(PaintManager3DL))]
	public class PaintManager3DLInspector : UnityEditor.Editor
	{
		SerializedProperty _textTemplateProperty;
		SerializedProperty _objectForPaintingProperty;
		SerializedProperty _materialProperty;
		SerializedProperty _shaderTextureNameProperty;
		SerializedProperty _brushProperty;
		SerializedProperty _brushSizeProperty;
		SerializedProperty _brushColorProperty;
		SerializedProperty _paintModeProperty;
		SerializedProperty _renderTextureModeProperty;
		SerializedProperty _overrideCameraProperty;
		SerializedProperty _cameraProperty;
		SerializedProperty _previewProperty;
		SerializedProperty _useNeighborsVerticesForRaycastsProperty;

		private PaintManager3DL _paintManager;
		private BasePaintObject _paintObject;
		private Component _component;
		private string[] _paintModes;
		private GUIContent[] _paintModesContent;
		private int _paintModeId;
		private string[] _renderTextureModes;
		private GUIContent[] _renderTextureModesContent;
		private int _renderTextureModeId;
		private int _shaderTextureNameSelectedId;
		private bool _isMeshObject;
		private bool _isUseNeighborsVerticesForRaycastsChanged;
		private PaintMode _paintMode;

		[MenuItem("Component/2D\u22153D Paint 3DL")]
		static void AddPaintManagerComponent()
		{
			if (Selection.activeGameObject.GetComponent<PaintManager3DL>() == null)
			{
				Selection.activeGameObject.AddComponent<PaintManager3DL>();
			}
		}
		
		private static void AddTrianglesData(MenuCommand command, bool fillNeighbors)
		{
			var paintManager = (PaintManager3DL)command.context;
			paintManager.FillTrianglesData(fillNeighbors);
			if (!Application.isPlaying)
			{
				EditorUtility.SetDirty(paintManager);
				EditorSceneManager.MarkSceneDirty(paintManager.gameObject.scene);
			}
		}
		
		[MenuItem("CONTEXT/PaintManager/Fill Triangles Data")]
		static void AddTrianglesDataWithNeighbors(MenuCommand command)
		{
			AddTrianglesData(command, true);
		}

		[MenuItem("CONTEXT/PaintManager/Fill Triangles Data", true)]
		static bool ValidationAddTrianglesDataWithNeighbors()
		{
			var paintManager = Selection.activeGameObject.GetComponent<PaintManager3DL>();
			var supportedComponent = PaintManagerHelper.GetSupportedComponent(paintManager.objectForPainting);
			return supportedComponent != null && PaintManagerHelper.IsMeshObject(supportedComponent);
		}

		[MenuItem("CONTEXT/PaintManager/Clear Triangles Data")]
		static void ClearTrianglesData(MenuCommand command)
		{
			var paintManager = (PaintManager3DL)command.context;
			paintManager.ClearTrianglesData();
			if (!Application.isPlaying)
			{
				EditorUtility.SetDirty(paintManager);
				EditorSceneManager.MarkSceneDirty(paintManager.gameObject.scene);
			}
		}
		
		[MenuItem("CONTEXT/PaintManager3DL/Clear Triangles Data", true)]
		static bool ValidationClearTrianglesData()
		{
			var paintManager = Selection.activeGameObject.GetComponent<PaintManager3DL>();
			var supportedComponent = PaintManagerHelper.GetSupportedComponent(paintManager.objectForPainting);
			return supportedComponent != null && PaintManagerHelper.IsMeshObject(supportedComponent);
		}
		
		void Awake()
		{
			_paintModes = Enum.GetNames(typeof(PaintMode));
			_paintModesContent = new GUIContent[_paintModes.Length];
			for (int i = 0; i < _paintModesContent.Length; i++)
			{
				_paintModesContent[i] = new GUIContent(_paintModes[i]);
			}
			_renderTextureModes = Enum.GetNames(typeof(RenderTexturesMode));
			_renderTextureModesContent = new GUIContent[_renderTextureModes.Length];
			for (int i = 0; i < _renderTextureModesContent.Length; i++)
			{
				_renderTextureModesContent[i] = new GUIContent(_renderTextureModes[i]);
			}
		}

		void OnEnable()
		{
			_paintManager = (PaintManager3DL)target;
			_paintObject = _paintManager.PaintObject;

			_textTemplateProperty = serializedObject.FindProperty("textInputTemplate");
			_objectForPaintingProperty = serializedObject.FindProperty("objectForPainting");
			_materialProperty = serializedObject.FindProperty("materialsContainer.sourceMaterial");
			_shaderTextureNameProperty = serializedObject.FindProperty("materialsContainer.shaderTextureName");
			_brushProperty = serializedObject.FindProperty("materialsContainer.brush.texture");
			_brushColorProperty = serializedObject.FindProperty("materialsContainer.brush.color");
			_brushSizeProperty = serializedObject.FindProperty("materialsContainer.brush.size");
			_paintModeProperty = serializedObject.FindProperty("materialsContainer.paintMode");
			_renderTextureModeProperty = serializedObject.FindProperty("renderTextureMode");
			_overrideCameraProperty = serializedObject.FindProperty("shouldOverrideCamera");
			_cameraProperty = serializedObject.FindProperty("overrideCamera");
			_previewProperty = serializedObject.FindProperty("preview");
			_useNeighborsVerticesForRaycastsProperty = serializedObject.FindProperty("useNeighborsVerticesForRaycasts");

			SetValues();
		}

		private void SetValues()
		{
			_paintModeId = _paintModeProperty.enumValueIndex;
			_renderTextureModeId = _renderTextureModeProperty.enumValueIndex;
			UpdateTexturesList();
		}

		private void UpdateTexturesList()
		{
			var material = _materialProperty.objectReferenceValue as Material;
			if (material != null)
			{
				var shaderTextureNames = PaintManagerHelper.GetTexturesListFromShader(material);
				_shaderTextureNameSelectedId = Array.IndexOf(shaderTextureNames, _shaderTextureNameProperty.stringValue);
			}
			_paintManager.materialsContainer.InitMaterial(material);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			
			EditorGUILayout.PropertyField(_textTemplateProperty, new GUIContent("Text Template", "Text Template"));
			EditorGUILayout.PropertyField(_objectForPaintingProperty, new GUIContent("Object For Painting", PaintManagerHelper.ObjectForPaintingTooltip));
			EditorGUI.EndDisabledGroup();
			if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(_objectForPaintingProperty.objectReferenceValue != null)))
			{
				_paintManager = (PaintManager3DL)target;
				_paintObject = _paintManager.PaintObject;
				_component = PaintManagerHelper.GetSupportedComponent(_objectForPaintingProperty.objectReferenceValue as GameObject);
				_isMeshObject = PaintManagerHelper.IsMeshObject(_component);
				if (_isMeshObject && !_paintManager.HaveTriangles)
				{
					_paintManager.FillTrianglesData(false);
				}
				DrawMaterialBlock();
				if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(_materialProperty.objectReferenceValue != null)))
				{
					DrawBrushBlock();
					DrawPaintModeBlock();
					DrawRenderTextureModeBlock();
					DrawCheckboxesBlock();

					if (!_paintManager._initialized && Application.isPlaying)
					{
						if (GUILayout.Button(new GUIContent("Initialize", "Initialize Script"),
							GUILayout.ExpandWidth(true)))
						{
							_paintManager.Initialize();
						}
					}
					
					if (_paintManager._initialized && Application.isPlaying)
					{
						if (GUILayout.Button(new GUIContent("Select Canvas For Painting", "Select For Painting on this Mesh"),
							GUILayout.ExpandWidth(true)))
						{
							_paintManager.SelectCanvasForPainting();
						}
					}
					
					if (_paintManager._initialized && Application.isPlaying)
					{
						if (GUILayout.Button(new GUIContent("Deselect All Painting Canvas", "Deselect All Mesh"),
							GUILayout.ExpandWidth(true)))
						{
							_paintManager.DeselectAllCanvasForPainting();
						}
					}

					DrawButtonsBlock();
					
				}
				EditorGUILayout.EndFadeGroup();
			}
			EditorGUILayout.EndFadeGroup();
			DrawAutoFillButton();
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawAutoFillButton()
		{
			GUI.backgroundColor = PaintManagerHelper.MaterialButton1Color;
			var disabled = _objectForPaintingProperty.objectReferenceValue == null || _materialProperty.objectReferenceValue == null;
			EditorGUI.BeginDisabledGroup(!disabled);
			if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(disabled)))
			{
				if (GUILayout.Button(new GUIContent("Auto fill", PaintManagerHelper.AutoFillButtonTooltip), GUILayout.ExpandWidth(true)))
				{
					var objectForPaintingFillResult = FindObjectForPainting();
					var findMaterialResult = FindMaterial();
					if (!objectForPaintingFillResult && !findMaterialResult)
					{
						Debug.Log("Can't find ObjectForPainting and Material.");
					}
					else if (!objectForPaintingFillResult)
					{
						Debug.Log("Can't find ObjectForPainting.");
					}
					else if (!findMaterialResult)
					{
						Debug.Log("Can't find Material.");
					}
				}
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUI.EndDisabledGroup();
		}

		private bool FindObjectForPainting()
		{
			if (_objectForPaintingProperty.objectReferenceValue == null)
			{
				var supportedComponent = PaintManagerHelper.GetSupportedComponent(_paintManager.gameObject);
				if (supportedComponent != null)
				{
					_objectForPaintingProperty.objectReferenceValue = supportedComponent.gameObject;
					return true;
				}
				if (_paintManager.gameObject.transform.childCount > 0)
				{
					var compatibleComponents = new List<Component>();
					var allComponents = _paintManager.gameObject.transform.GetComponentsInChildren<Component>();
					foreach (var component in allComponents)
					{
						var childComponent = PaintManagerHelper.GetSupportedComponent(component.gameObject);
						if (childComponent != null)
						{
							compatibleComponents.Add(childComponent);
							break;
						}
					}
					if (compatibleComponents.Count > 0)
					{
						_objectForPaintingProperty.objectReferenceValue = compatibleComponents[0].gameObject;
						return true;
					}
				}
				return false;
			}
			return true;
		}

		private bool FindMaterial()
		{
			var result = false;
			_component = PaintManagerHelper.GetSupportedComponent(_objectForPaintingProperty.objectReferenceValue as GameObject);
			if (_component != null)
			{
				var renderer = _component as Renderer;
				if (renderer != null && renderer.sharedMaterial != null)
				{
					_materialProperty.objectReferenceValue = renderer.sharedMaterial;
					result = true;
				}
				var maskableGraphic = _component as RawImage;
				if (maskableGraphic != null && maskableGraphic.material != null)
				{
					_materialProperty.objectReferenceValue = maskableGraphic.material;
					result = true;
				}
			}
			UpdateTexturesList();
			return result;
		}

		private void DrawMaterialBlock()
		{
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			EditorGUILayout.PropertyField(_materialProperty, new GUIContent("Material", PaintManagerHelper.MaterialTooltip));
			if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(_materialProperty.objectReferenceValue != null)))
			{
				var shaderTextureNames = PaintManagerHelper.GetTexturesListFromShader(_materialProperty.objectReferenceValue as Material);
				var shaderTextureNamesContent = new GUIContent[shaderTextureNames.Length];
				for (int i = 0; i < shaderTextureNamesContent.Length; i++)
				{
					shaderTextureNamesContent[i] = new GUIContent(shaderTextureNames[i]);
				}

				var shaderTextureName = _paintManager.materialsContainer.ShaderTextureName;
				if (shaderTextureNames.Contains(shaderTextureName))
				{
					for (int i = 0; i < shaderTextureNames.Length; i++)
					{
						if (shaderTextureNames[i] == shaderTextureName)
						{
							_shaderTextureNameSelectedId = i;
							break;
						}
					}
				}
				
				_shaderTextureNameSelectedId = Mathf.Clamp(_shaderTextureNameSelectedId, 0, int.MaxValue);
				_shaderTextureNameSelectedId = EditorGUILayout.Popup(new GUIContent("Shader Texture Name", PaintManagerHelper.ShaderTextureNameTooltip), _shaderTextureNameSelectedId, shaderTextureNamesContent);
				if (shaderTextureNames.Length > 0 && shaderTextureNames.Length > _shaderTextureNameSelectedId && shaderTextureNames[_shaderTextureNameSelectedId] != shaderTextureName)
				{
					_shaderTextureNameProperty.stringValue = shaderTextureNames[_shaderTextureNameSelectedId];
					_paintManager.materialsContainer.ShaderTextureName = _shaderTextureNameProperty.stringValue;
					MarkAsDirty();
				}
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUI.EndDisabledGroup();
		}

		private void DrawBrushBlock()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_brushProperty, new GUIContent("Brush", PaintManagerHelper.BrushTooltip));
			if (EditorGUI.EndChangeCheck())
			{
				_paintManager.materialsContainer.Brush.Texture = _brushProperty.objectReferenceValue as Texture;
			}
			if (_brushProperty.objectReferenceValue == null)
			{
				_paintManager.materialsContainer.Brush.Texture = Settings.Instance.defaultBrush;
			}
			EditorGUILayout.Slider(_brushSizeProperty, PaintManagerHelper.MinValue, PaintManagerHelper.MaxValue, new GUIContent("Brush Size", PaintManagerHelper.BrushSizeTooltip));
			EditorGUI.BeginChangeCheck();
			_brushColorProperty.colorValue = EditorGUILayout.ColorField(new GUIContent("Brush Color", PaintManagerHelper.BrushColorTooltip), _brushColorProperty.colorValue);
			if (EditorGUI.EndChangeCheck())
			{
				_paintManager.materialsContainer.Brush.Color = _brushColorProperty.colorValue;
			}
		}

		private void DrawPaintModeBlock()
		{
			_paintModeId = EditorGUILayout.Popup(new GUIContent("Paint Mode", PaintManagerHelper.PaintingModeTooltip), _paintModeProperty.enumValueIndex, _paintModesContent);
			_paintMode = (PaintMode)Enum.Parse(typeof(PaintMode), _paintModes[_paintModeId]);
			_paintManager.materialsContainer.PaintMode = _paintMode;
		}

		private void DrawRenderTextureModeBlock()
		{
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			EditorGUI.BeginChangeCheck();
			_renderTextureModeId = EditorGUILayout.Popup(new GUIContent("RenderTexture Mode", PaintManagerHelper.RenderTextureModeTooltip), _renderTextureModeProperty.enumValueIndex, _renderTextureModesContent);
			if (EditorGUI.EndChangeCheck())
			{
				MarkAsDirty();
			}
			var mode = (RenderTexturesMode)Enum.Parse(typeof(RenderTexturesMode), _renderTextureModes[_renderTextureModeId]);
			_paintManager.renderTextureMode = mode;
			EditorGUI.EndDisabledGroup();
		}

		private void DrawCheckboxesBlock()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_overrideCameraProperty, new GUIContent("Override Camera", PaintManagerHelper.OverrideCameraTooltip));
			if (EditorGUI.EndChangeCheck())
			{
				_paintManager.shouldOverrideCamera = _overrideCameraProperty.boolValue;
			}
			using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(_overrideCameraProperty.boolValue)))
			{
				if (group.visible)
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(_cameraProperty, new GUIContent("Camera", PaintManagerHelper.CameraTooltip));
					if (EditorGUI.EndChangeCheck())
					{
						_paintManager.Camera = _cameraProperty.objectReferenceValue as Camera;
					}
				}
			}
			var hasOneTexture = _renderTextureModeId == (int) RenderTexturesMode.OneTexture && _isMeshObject;
			EditorGUI.BeginDisabledGroup(hasOneTexture);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_previewProperty, new GUIContent("Preview", PaintManagerHelper.PreviewTooltip));
			var change = EditorGUI.EndChangeCheck();
			if (hasOneTexture)
			{
				_previewProperty.boolValue = false;
			}
			else if (change)
			{
				_paintManager.Preview = _previewProperty.boolValue;
				MarkAsDirty();
			}
			EditorGUI.EndDisabledGroup();
			
			if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(_isMeshObject)))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(_useNeighborsVerticesForRaycastsProperty, new GUIContent("Use Neighbors Vertices For Raycasts", PaintManagerHelper.UseNeighborsVerticesForRaycastTooltip));
				if (EditorGUI.EndChangeCheck())
				{
					_isUseNeighborsVerticesForRaycastsChanged = true;
				}
				if (_isUseNeighborsVerticesForRaycastsChanged && Event.current.type != EventType.Used)
				{
					_isUseNeighborsVerticesForRaycastsChanged = false;
					_paintManager.UseNeighborsVerticesForRaycasts = _useNeighborsVerticesForRaycastsProperty.boolValue;
					MarkAsDirty();
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private void DrawButtonsBlock()
		{
			GUILayout.BeginHorizontal();
			GUI.backgroundColor = PaintManagerHelper.MaterialButton2Color;
			var disableClone = Application.isPlaying;
			EditorGUI.BeginDisabledGroup(disableClone);
			if (GUILayout.Button(new GUIContent("Clone Material", PaintManagerHelper.CloneMaterialTooltip), GUILayout.ExpandWidth(true)))
			{
				var clonedMaterial = PaintManagerHelper.CloneMaterial(_materialProperty.objectReferenceValue as Material);
				if (clonedMaterial != null)
				{
					_materialProperty.objectReferenceValue = clonedMaterial;
				}
			}
			if (GUILayout.Button(new GUIContent("Clone Texture", PaintManagerHelper.CloneTextureTooltip), GUILayout.ExpandWidth(true)))
			{
				PaintManagerHelper.CloneTexture(_materialProperty.objectReferenceValue as Material, _shaderTextureNameProperty.stringValue);
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
				
			GUILayout.BeginHorizontal();
			var disableUndo = !(Application.isPlaying && _paintObject != null && _paintObject.StateKeeper.CanUndo());
			EditorGUI.BeginDisabledGroup(disableUndo);
			GUI.backgroundColor = PaintManagerHelper.MaterialButton3Color;
			if (GUILayout.Button(new GUIContent("Undo", PaintManagerHelper.UndoTooltip), GUILayout.ExpandWidth(true)))
			{
				_paintObject.StateKeeper.Undo();
				_paintManager.Render();
			}
			EditorGUI.EndDisabledGroup();
			var disableRedo = !(Application.isPlaying && _paintObject != null && _paintObject.StateKeeper.CanRedo());
			EditorGUI.BeginDisabledGroup(disableRedo);
			if (GUILayout.Button(new GUIContent("Redo", PaintManagerHelper.RedoTooltip), GUILayout.ExpandWidth(true)))
			{
				_paintObject.StateKeeper.Redo();
				_paintManager.Render();
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
			
			GUI.backgroundColor = PaintManagerHelper.MaterialButton4Color;
			GUILayout.BeginHorizontal();
			var disableBake = !(Application.isPlaying && _isMeshObject);
			EditorGUI.BeginDisabledGroup(disableBake);
			if (GUILayout.Button(new GUIContent("Bake", PaintManagerHelper.BakeTooltip), GUILayout.ExpandWidth(true)))
			{
				PaintManagerHelper.Bake(_paintManager.materialsContainer.SourceTexture, _paintManager.Bake);
			}
			EditorGUI.EndDisabledGroup();
				
			EditorGUI.BeginDisabledGroup(!Application.isPlaying);
			if (GUILayout.Button(new GUIContent("Save", PaintManagerHelper.SaveToFileTooltip), GUILayout.ExpandWidth(true)))
			{
				PaintManagerHelper.SaveToFile(_paintManager);
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}

		private void MarkAsDirty()
		{
			if (!Application.isPlaying)
			{
				EditorUtility.SetDirty(_paintManager);
				EditorSceneManager.MarkSceneDirty(_paintManager.gameObject.scene);
			}
		}
	}
}