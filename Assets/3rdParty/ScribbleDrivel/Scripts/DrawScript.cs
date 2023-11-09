using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Module.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LylekGames
{
    public class DrawScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public static DrawScript Instance;

        public GameObject DrawingBoard;
        
        public GameObject brushPrefab;
        private Image brush;

        public Color brushColor = Color.black;
        [Range(10, 50)]
        public int brushSize = 16;
        [Range(0.1f, 1.0f)]
        public float spacing = 0.25f;

        //used to draw a line from our previous mouse pos to our current
        public Vector3 previousMousePosition = Vector3.zero;
        private bool drawingInProgress = false;
        public bool canDraw = false;

        //Used to position our brush tool
        private GameObject brushHolder;
        //Used to store our drawing history
        private GameObject historyHolder;
        //Used to store our current drawing
        private GameObject dotField;
        //store our draw history
        public List<GameObject> drawHistory = new List<GameObject>();
        
        public EventAction EventStartDrawing = new EventAction();
        public EventAction EventStopDrawing = new EventAction();
        
        private bool _mouseOverlay;
        private Camera _worldCamera;
        private bool _isDrawing;

        public void Awake() 
        {
            Instance = this;
            if (DrawingBoard == null) {
                DrawingBoard = this.gameObject;
            }

            var canvas = DrawingBoard.transform.GetComponentInParent<Canvas>();
            if (canvas == null || canvas.worldCamera == null) {
                _mouseOverlay = true;
            } else {
                _worldCamera = canvas.worldCamera;
            }
        }
        
        public void GetDefaultSettings()
        {
            if (!GetComponent<Mask>())
                gameObject.AddComponent<Mask>();
            if (!brushPrefab)
                brushPrefab = Resources.Load("Brush") as GameObject;
            if (!brushPrefab)
                Debug.LogError("Cannot locate Brush prefab. Please assign a brush to the DrawScript, in the Inspector. This may be a simple gameObject containing an Image Component.");
            LoadSpritesAndInputField();
            brushColor = Color.black;
            brushSize = 10;
            spacing = 0.25f;
            previousMousePosition = Vector3.zero;
            canDraw = false;
        }
        public void Start()
        {
            if (!historyHolder)
                historyHolder = NewUIObject("DrawHistory");
            if (!brushHolder)
                brushHolder = NewUIObject("BrushHolder");
            if (!brushPrefab)
                brushPrefab = Resources.Load("Brush") as GameObject;
            if (brushPrefab)
            {
                brush = Instantiate(brushPrefab.gameObject, GetMousePosition(), Quaternion.identity).GetComponent<Image>();
                brush.gameObject.transform.SetParent(brushHolder.transform, false);

                SetBrushSize(brushSize);
                SetBrushColor(brushColor);
            }
            else
                Debug.LogError("Brush is missing. Please assign a brush to the DrawScript, in the Inspector. This may be a simple gameObject containing an Image Component.");
            
            LoadSpritesAndInputField();
            UseBrushTool();
        }
        public GameObject NewUIObject(string name)
        {
            GameObject newObject = new GameObject(name);
            newObject.AddComponent<CanvasRenderer>();
            newObject.transform.SetParent(DrawingBoard.transform, false);
            newObject.transform.localScale = Vector3.one;
            newObject.transform.localPosition = Vector3.zero;
            //newObject.hideFlags = HideFlags.HideInHierarchy;
            return newObject;
        }
        public void SetBrushSize(int bSize)
        {
            brushSize = bSize;
            Vector2 newBrushSize = new Vector2(brushSize, brushSize);
            brush.rectTransform.sizeDelta = newBrushSize;
        }
        public void SetBrushColor(Color bColor)
        {
            brushColor = bColor;
            brush.color = brushColor;
        }
        public void SetBrushShape(Sprite bSprite)
        {
            brush.sprite = bSprite;
        }
        public void Undo()
        {
            if (drawHistory.Count > 0)
            {
                Destroy(drawHistory[drawHistory.Count - 1].gameObject);
                drawHistory.RemoveAt(drawHistory.Count - 1);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            canDraw = true;
            brush.gameObject.SetActive(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            canDraw = false;
            brush.gameObject.SetActive(false);
        }
        
        private void Update() {
            if (brush) {
                if (!_isDrawing) {
                    brush.gameObject.SetActive(IsDrawingBoard());                        
                }
                if (_isDrawing && _isTextTool) {
                    brush.gameObject.SetActive(false);
                }
                brush.transform.position = GetMousePosition();
            }

            if (_isTextTool) {
                if (Input.GetMouseButtonDown(0)) {
                    DrawText();
                }
                return;
            }
            
            if (Input.GetMouseButtonUp(0)) {
                EventStopDrawing.Invoke();
                _isDrawing = false;
            } else if (Input.GetMouseButtonDown(0)) {
                if (!IsDrawingBoard()) { return; }
                
                _isDrawing = true;
                EventStartDrawing.Invoke();
                
                SetBrushSize(brushSize);

                //Create history
                dotField = NewUIObject("drawHist" + drawHistory.Count);
                dotField.transform.SetParent(historyHolder.transform, false);
                drawHistory.Add(dotField);

                previousMousePosition = brush.transform.position;
                Draw(previousMousePosition);
            } else if (Input.GetMouseButton(0)) {
                if (!_isDrawing) { return; }
                
                if(!dotField) {
                    //Create history
                    dotField = NewUIObject("drawHist " + drawHistory.Count);
                    dotField.transform.SetParent(historyHolder.transform, false);
                    drawHistory.Add(dotField);
                }
                
                if (previousMousePosition != brush.transform.position && drawingInProgress == false) {
                    DrawDistance(previousMousePosition, brush.transform.position);
                }
            }
        }

        private void DrawText() {
            if (_isDrawing) {
                EventStopDrawing.Invoke();
                _isDrawing = false;
                if (_drawnInputField != null) {
                    if (!string.IsNullOrEmpty(_drawnInputField.text)) {
                        _drawnInputField.textComponent.transform.SetParent(dotField.transform);
                        Destroy(_drawnInputField);
                    } else {
                        drawHistory.Remove(dotField);
                        Destroy(dotField);
                    }
                    _drawnInputField = null;
                }
                return;
            }
            
            if (!IsDrawingBoard()) { return; }
            if (_isDrawing) { return; }
                
            _isDrawing = true;
            EventStartDrawing.Invoke();
                
            SetBrushSize(brushSize);
            
            //Create history
            dotField = NewUIObject("drawHist" + drawHistory.Count);
            dotField.transform.SetParent(historyHolder.transform, false);
            drawHistory.Add(dotField);
            
            previousMousePosition = brush.transform.position;
            
            _drawnInputField = Instantiate(_textInputField, dotField.transform, true).GetComponent<TMP_InputField>();
            _drawnInputField.transform.localScale = Vector3.one;
            _drawnInputField.transform.position = previousMousePosition;
            _drawnInputField.gameObject.SetActive(true);
            _drawnInputField.pointSize = brushSize;
            _drawnInputField.textComponent.color = brushColor;
            _drawnInputField.onEndEdit.AddListener(value => {
                if (string.IsNullOrEmpty(_drawnInputField.text)) {
                    drawHistory.Remove(dotField);
                    Destroy(dotField);
                    _drawnInputField = null;
                    EventStopDrawing.Invoke();
                    _isDrawing = false;
                }
            });
            _drawnInputField.Select();
        }

        private bool IsDrawingBoard() {
            if (_mouseOverlay) {
                //Check to see if our cursor if over our drawing board (in case the PointerHandlers miss it)
                RaycastHit2D hit = Physics2D.Raycast(GetMousePosition(), Vector2.zero);
                if (hit) {
                    if (hit.transform.gameObject == DrawingBoard)
                        return true;
                }
            } else {
                var pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                
                if (results.Count > 0) {
                    if (results[0].gameObject == DrawingBoard) {
                        return true;
                    } 
                }
            }
            return false;
        }
        
        private void DrawDistance(Vector3 oldPos, Vector3 newPos)
        {
            drawingInProgress = true;
            float spaceDist = Vector3.Distance(oldPos, newPos);
            float actualSpacing = spacing * brush.rectTransform.rect.height * brush.rectTransform.lossyScale.y;
            float newSpace = actualSpacing / spaceDist;
            float lerpStep = newSpace;
            if (spaceDist >= actualSpacing)
            {
                while (lerpStep <= 1)
                {
                    Vector3 newDotPos = Vector3.Lerp(oldPos, newPos, lerpStep);
                    Draw(newDotPos);

                    lerpStep += newSpace;
                }
                previousMousePosition = newPos;
            }
            
            drawingInProgress = false;
        }
        private void Draw(Vector3 pos)
        {
            var newDot = Instantiate(brush.gameObject, dotField.transform, true);
            newDot.transform.localScale = Vector3.one;
            newDot.transform.position = pos;
            newDot.SetActive(true);
        }

        private Vector3 GetMousePosition() {
            if (_mouseOverlay) {
                return Input.mousePosition;
            }
            var screenPoint = Input.mousePosition;
            screenPoint.z = 100.0f; //distance of the plane from the camera
            return _worldCamera.ScreenToWorldPoint(screenPoint);
        }
        
        
        
        #region Text tool
        
        [SerializeField] private bool _isTextTool;
        [SerializeField] private Sprite _textSprite;
        [SerializeField] private Sprite _brushSprite;
        [SerializeField] private GameObject _textInputField;
        
        //Used to store our current drawing
        private TMP_InputField _drawnInputField;

        public bool IsUseTextTool() {
            return _isTextTool;
        }
        
        public void UseBrushTool() {
            _isTextTool = false;
            SetBrushShape(_brushSprite);
        }

        public void UseTextTool() {
            _isTextTool = true;
            SetBrushShape(_textSprite);
        }
        
        private void LoadSpritesAndInputField() {
            if (!_textSprite)
                _textSprite = Resources.Load("icon-cursor-text") as Sprite;
            if (!_textSprite)
                Debug.LogError("Cannot locate text sprite. Please assign a text sprite to the DrawScript, in the Inspector.");
            
            if (!_brushSprite)
                _brushSprite = Resources.Load("icon-cursor-brush") as Sprite;
            if (!_brushSprite)
                Debug.LogError("Cannot locate brush sprite. Please assign a brush sprite to the DrawScript, in the Inspector.");
            
            if (!_textInputField)
                _textInputField = Resources.Load("TextInputField") as GameObject;
            if (!_textInputField)
                Debug.LogError("Cannot locate TextInputField. Please assign a TextInputField to the DrawScript, in the Inspector.");
        }
        
        #endregion
    }
}
