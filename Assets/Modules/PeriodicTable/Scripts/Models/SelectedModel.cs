using UnityEngine;

namespace Module.PeriodicTable
{
    public class SelectedModel : MonoBehaviour
    {

        public static PeriodicModel.PeriodicElement SelectElement;
        public static PeriodicModel.PeriodicType SelectType;
        public static bool openGroup = false;


        public static void ClearSelectType()
        {
            openGroup = false;

            SelectType.button = null;
            SelectType.gameObject = null;
            SelectType.name = "";
            SelectType.periodicElement = null;
            SelectType.positionButton = Vector3.zero;
        }

        public static void ClearSelectElement()
        {
            SelectElement.description_en = "";
            SelectElement.description_no = "";
            SelectElement.name = "";
            SelectElement.type = "";
            SelectElement.gameObject = null;
            SelectElement.position = Vector3.zero;
            SelectElement.material = null;
        }

        public static bool IsSelectedElement()
        {
            if (SelectElement.gameObject == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsSelectedGroup()
        {
            if (SelectType.gameObject == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}