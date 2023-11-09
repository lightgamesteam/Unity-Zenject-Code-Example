using TMPro;
using Zenject;

namespace TDL.Views
{
    public class DropdownActivityHeaderMobileView : ViewBase
    {
        private TextMeshProUGUI _title;        

        public override void InitUiComponents()
        {
            _title = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public class Factory : PlaceholderFactory<DropdownActivityHeaderMobileView>
        {
            
        }
    }
}