using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XDPaint.Core.PaintObject.States
{
    public class StateKeeper
    {
        public Action OnChangeState;
        public Action OnResetState;
        public Action<State> OnMouseDown;
        public Action OnMouseUp;
        public Action OnReDraw;

        private Dictionary<int, State> _drawingActions = new Dictionary<int, State>();
        private Action<int, int, Dictionary<int, State>> _extraDraw;
        private int _currentStateIndex;
        private bool _isMouseDown;
        private bool _isEnabled;

        public void Init(Action<int, int, Dictionary<int, State>> extraDraw, bool enabled)
        {
            _isEnabled = enabled;
            _extraDraw = extraDraw;
            OnMouseDown = (state) =>
            {
                if (!_isEnabled)
                    return;
                
                _isMouseDown = true;
                if (!_drawingActions.ContainsKey(_currentStateIndex))
                {
                    _drawingActions.Add(_currentStateIndex, new State(state.PaintMode, state.BrushTexture, state.BrushColor, state.BrushSize));
                }
                else
                {
                    _drawingActions[_currentStateIndex].DrawingStates.Clear();
                    _drawingActions[_currentStateIndex] = new State(state.PaintMode, state.BrushTexture, state.BrushColor, state.BrushSize);
                }
            };
            OnMouseUp = () =>
            {
                if (!_isEnabled)
                    return;
                
                _isMouseDown = false;
                _currentStateIndex = _drawingActions.Count;
            };
        }

        public void AddState(Vector2 position, float brushSize)
        {
            if (!_isEnabled)
                return;
            
            if (_drawingActions.Count > 0)
            {
                var statePosition = new DrawingState()
                {
                    Positions = new[] { position },
                    BrushSizes = new[] { brushSize }
                };
                for (int i = _drawingActions.Count - 1; i > _currentStateIndex; i--)
                {
                    _drawingActions.Remove(_drawingActions.Keys.ElementAt(i));
                }
                _drawingActions[_currentStateIndex].DrawingStates.Add(statePosition);
            }
        }
        
        public void AddState(Vector2[] positions, float[] brushSizes)
        {
            if (!_isEnabled)
                return;

            if (_drawingActions.Count > 0)
            {
                var statePosition = new DrawingState()
                {
                    Positions = positions,
                    BrushSizes = brushSizes
                };
                for (int i = _drawingActions.Count - 1; i > _currentStateIndex; i--)
                {
                    _drawingActions.Remove(_drawingActions.Keys.ElementAt(i));
                }
                _drawingActions[_currentStateIndex].DrawingStates.Add(statePosition);
            }
        }

        public void Undo()
        {
            if (!_isEnabled)
                return;
            
            OnResetState();
            OnChangeState();
            var index = _currentStateIndex - 1;
            var newDict = new Dictionary<int, State>(_drawingActions);
            OnReDraw = () => _extraDraw(0, index, newDict);
            _currentStateIndex--;
            if (_currentStateIndex < 0)
            {
                _currentStateIndex = 0;
            }
        }

        public void Redo()
        {
            if (!_isEnabled)
                return;
            
            OnChangeState();
            _currentStateIndex++;
            var index = _currentStateIndex;
            var newDict = new Dictionary<int, State>(_drawingActions);
            OnReDraw = () => _extraDraw(index - 1, index, newDict);
            if (_currentStateIndex > _drawingActions.Count)
            {
                _currentStateIndex = _drawingActions.Count;
            }
        }

        public void Reset()
        {
            if (!_isEnabled)
                return;
            
            _currentStateIndex = 0;
            _drawingActions.Clear();
        }

        public bool CanUndo()
        {
            return _drawingActions.Count > 0 && _currentStateIndex > 0;
        }

        public bool CanRedo()
        {
            return _drawingActions.Count > 0 && _currentStateIndex < _drawingActions.Count && !_isMouseDown;
        }
    }
}