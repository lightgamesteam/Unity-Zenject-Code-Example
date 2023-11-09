using System.Collections.Generic;
using UnityEngine;

namespace Module.PeriodicTable
{
    public class DataModel : MonoBehaviour
    {
        public enum Language
        {
            English,
            Norwegian_NB,
            Norwegian_NN
        }

        public static Language CurrentLanguage = Language.English;
        public static ScreenOrientation ScreenOrientation;

        public static float SpeedTweenColorBackgroundPopUp = 0.5f;
        public static float SpeedTweenColorText = 0.25f;

        public static float DistanceToCamera = -2.5f;
        public static float DistanceMovementToLeft = 0.32f;

        public static float SpeedTweenColorDescription = 0.5f;
        public static float SpeedTweenMoveToLeft = 0.5f;
        public static float SpeedTweenMoveToCenter = 0.5f;
        public static float SpeedTweenColorBackground = 1f;
        public static float SpeedTweenFastColorBackground = 0.5f;
        public static float SpeedTweenReturnSelectedItem = 0.5f;

        public static Color VisibleColor = new Color(0, 0, 0, 0.7f);
        public static Color InvisibleColor = new Color(0, 0, 0, 0.0f);
        public static Color ButtonOnDescriptionColor = new Color(0.05f, 0.05f, 0.05f, 0f);


        public static Vector3 offsetGroupe = new Vector3(0.01f, 0.08f, -0.2f);

        public static Dictionary<string, Color> groupColors = new Dictionary<string, Color>
        {
            {"Alkali_Metal", new Color32(248, 234, 234, 255)},
            {"Alkaline_Metal", new Color32(251, 243, 235, 255)},
            {"Transition_Metal", new Color32(255, 255, 231, 255)},
            {"Basic_Metal", new Color32(228, 248, 241, 255)},
            {"Semimetal", new Color32(223, 247, 249, 255)},
            {"Nonmetal", new Color32(211, 236, 244, 255)},
            {"Halogen", new Color32(230, 234, 251, 255)},
            {"Noble_Gas", new Color32(234, 232, 248, 255)},
            {"Lanthanide", new Color32(238, 249, 227, 255)},
            {"Actinide", new Color32(226, 237, 225, 255)}

        };
    }
}