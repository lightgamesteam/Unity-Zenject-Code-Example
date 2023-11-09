using DigitalRuby.Tween;
using UnityEngine;

namespace Module.IK
{
    public class TweenController
    {

        public static void MoveUIPanel(float speed, RectTransform panelUI, Vector3 curentPos, Vector3 endPos)
        {
            panelUI.gameObject.Tween("MoveUIPanel", curentPos, endPos, speed,
                            TweenScaleFunctions.Linear, (t) => { panelUI.position = t.CurrentValue; }, (t2) =>
                            {
                                // After animation...
                            });
        }

    }
}