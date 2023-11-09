using TMPro;
using UnityEngine;

namespace Module.Engine.ScrollbarLabels.Item {
    public class ItemScrollbarLabelsGroupView : MonoBehaviour {
        #region Variables

        [SerializeField] protected TextMeshProUGUI DisplayText;

        #endregion

        public virtual void Initialize(string displayText) {
            DisplayText.text = displayText;
        }
    }
}
