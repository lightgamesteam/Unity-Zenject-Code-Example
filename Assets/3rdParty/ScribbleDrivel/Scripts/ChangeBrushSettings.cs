using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LylekGames
{
    public class ChangeBrushSettings : MonoBehaviour
    {

        private Image myImage;

        public void Start()
        {
            myImage = GetComponent<Image>();
        }
        public void ChangeBrushColor()
        {
            DrawScript.Instance.SetBrushColor(myImage.color);
        }
        public void ChangeBrushSize()
        {
            DrawScript.Instance.SetBrushSize((int)myImage.rectTransform.rect.height);
        }
        public void ChangeBrushShape()
        {
            DrawScript.Instance.SetBrushShape(myImage.sprite);
        }
        public void Undo()
        {
            DrawScript.Instance.Undo();
        }
    }
}
