using System.Collections.Generic;
using Module.Core.Attributes;
using TMPro;
using UnityEngine;

namespace TDL.Views
{
    public class LocalizationTextView : MonoBehaviour
    {
        [ShowOnly] public int ID = -1;
        public Dictionary<string, string> LocalizedText = new Dictionary<string, string>();
        private const string FallbackCultureCode = "en-US";

   
        public void AddLocalization(int id, Dictionary<string, string> localizedText)
        {
            ID = id;
            LocalizedText = localizedText;
        }

        public void SetLocalizedText(string cultureCode)
        {
            if (!LocalizedText.ContainsKey(cultureCode))
            {
                if (LocalizedText.ContainsKey(FallbackCultureCode))
                    cultureCode = FallbackCultureCode;
                else
                    return;
            }
      
            if (gameObject.HasComponent<TextMeshProUGUI>())
            {
                gameObject.GetComponent<TextMeshProUGUI>().text = LocalizedText[cultureCode];
            } 
      
            if (gameObject.HasComponent<TextMesh>())
            {
                gameObject.GetComponent<TextMesh>().text = LocalizedText[cultureCode];
            } 
      
            if (gameObject.HasComponent<LabelData>())
            {
                gameObject.GetComponent<LabelData>().SetText(LocalizedText[cultureCode]);
            } 
        }
    }
}
