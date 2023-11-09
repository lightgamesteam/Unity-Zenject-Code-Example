using UnityEngine;

namespace Module.PeriodicTable
{
    public class AnimationController
    {
        public static void ReturnSelectedElement()
        {
            TweenController.TweenColorImage(ApplicationView.instance.ImageBackground, DataModel.InvisibleColor,
                DataModel.SpeedTweenColorBackground, false);

            var gameObject = SelectedModel.SelectElement.gameObject;
            var currentPos = gameObject.transform.position;
            var endPos = SelectedModel.SelectElement.position;

            if (SelectedModel.IsSelectedElement() && SelectedModel.IsSelectedGroup())
            {
                endPos = endPos + DataModel.offsetGroupe;
            }

            ApplicationView.SetActiveCollider(gameObject, false);
            ApplicationView.SetActivePopUp(false);
            TweenController.TweenReturnSelectedElement(gameObject, currentPos, endPos);
        }

        public static void MoveElementToCenter(Camera camera, GameObject gameObject)
        {
            var currentPos = gameObject.transform.position;
            var centerPos = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
            var endPos = new Vector3(centerPos.x, centerPos.y, DataModel.DistanceToCamera);
            var leftPos = new Vector3(endPos.x - DataModel.DistanceMovementToLeft, endPos.y,
                DataModel.DistanceToCamera);

            ApplicationView.instance.BackgroundView.SetActive(true);
            ApplicationView.instance.BackgroundView.SetVisibility(false);
            TweenController.TweenColorImage(ApplicationView.instance.ImageBackground, DataModel.VisibleColor,
                DataModel.SpeedTweenColorBackground, true);
            TweenController.TweenMoveElementToCenter(gameObject, currentPos, endPos, leftPos);
        }

        public static void MoveElementToForward(GameObject gameObject)
        {
            var currentPos = gameObject.transform.position;
            var endPos = currentPos + DataModel.offsetGroupe;

            ApplicationView.instance.BackgroundView.SetActive(true);
            ApplicationView.instance.BackgroundView.SetVisibility(false);
            TweenController.TweenColorImage(ApplicationView.instance.ImageBackground, DataModel.VisibleColor,
                DataModel.SpeedTweenColorBackground, true);
            TweenController.MoveElementsToForward(gameObject, currentPos, endPos);
        }

        public static void ReturnElement(GameObject gameObject, Vector3 endPos)
        {
            var currentPos = gameObject.transform.position;

            ApplicationView.SetActiveCollider(gameObject, false);
            TweenController.TweenReturnSelectedElement(gameObject, currentPos, endPos);
        }
    }
}