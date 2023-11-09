using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.UI;
using TMPro;

namespace Module.PeriodicTable
{
    public class TweenController : MonoBehaviour
    {
        private static List<ITween> _activeTweenList = new List<ITween>();

        public static void TweenReturnSelectedElement(GameObject gameObject, Vector3 startPos, Vector3 endPos)
        {
            var tween = gameObject.Tween("MoveReturn_" + gameObject.name, startPos, endPos,
                DataModel.SpeedTweenReturnSelectedItem,
                TweenScaleFunctions.CubicEaseIn, (t) => { gameObject.transform.position = t.CurrentValue; }, (t2) =>
                {
                    // After animation...
                    ApplicationView.SetActiveCollider(gameObject, true);
                });
            
            _activeTweenList.Add(tween);
        }

        public static void TweenMoveElementToCenter(GameObject gameObject, Vector3 startPos, Vector3 endPos,
            Vector3 leftPos)
        {
            var tween = gameObject.Tween("Move_" + gameObject.name, startPos, endPos,
                DataModel.SpeedTweenMoveToCenter,
                TweenScaleFunctions.Linear, (t) => { gameObject.transform.position = t.CurrentValue; }, (t) =>
                {
                    // After first animation...
                    ApplicationView.SetActivePopUp(true);
                    var tweenInner = gameObject.Tween("MoveLeft_" + gameObject.name, endPos, leftPos, DataModel.SpeedTweenMoveToLeft,
                        TweenScaleFunctions.CubicEaseIn, (t2) => { gameObject.transform.position = t2.CurrentValue; },
                        (t2) =>
                        {
                            // After second animation...
                            ApplicationView.instance.SetActiveDescription();
                            ApplicationView.instance.BackgroundView.SetСlickable(true);
                        });
                    
                    _activeTweenList.Add(tweenInner);
                });
            
            _activeTweenList.Add(tween);
        }


        public static void MoveElementsToForward(GameObject gameObject, Vector3 startPos, Vector3 endPos)
        {
            var tween = gameObject.Tween("Move_" + gameObject.name, startPos, endPos,
                DataModel.SpeedTweenMoveToCenter,
                TweenScaleFunctions.Linear, (t) => { gameObject.transform.position = t.CurrentValue; }, (t) =>
                {
                    // After first animation...
                    if (!SelectedModel.SelectType.button.Equals(gameObject))
                    {
                        ApplicationView.SetActiveCollider(gameObject, true);
                    }

                    ApplicationView.instance.BackgroundView.SetСlickable(true);
                });
            
            _activeTweenList.Add(tween);
        }

        public static void TweenColorImage(Image image, Color endColor, float speed, bool active)
        {
            var tween = image.gameObject.Tween(image.name, image.color, endColor, speed,
                TweenScaleFunctions.QuadraticEaseOut,
                (t) => { image.color = t.CurrentValue; }, (t) =>
                {
                    // After animation...
                    image.gameObject.SetActive(active);
                });
            
            _activeTweenList.Add(tween);
        }

        public static void TweenColorText(TextMeshProUGUI textMesh, Color endColor, float speed, bool active)
        {
            var tween = textMesh.gameObject.Tween(textMesh.name, textMesh.color, endColor, speed,
                TweenScaleFunctions.QuadraticEaseOut, (t) => { textMesh.color = t.CurrentValue; }, (t) =>
                {
                    // After animation...
                    textMesh.gameObject.SetActive(active);
                });
            
            _activeTweenList.Add(tween);
        }

        public static void StopTweenForActiveObject()
        {
            if (_activeTweenList.Count > 0)
            {
                foreach (var tween in _activeTweenList)
                {
                    tween.Stop(TweenStopBehavior.DoNotModify);
                }
                
                _activeTweenList.Clear();
            }
        }
    }
}