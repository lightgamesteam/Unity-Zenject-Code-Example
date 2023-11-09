using XDPaint.Controllers;

namespace XDPaint.Tools
{
	public class ControllersContainer : Singleton<ControllersContainer>
	{
		private new void Awake()
		{
			if (InputController.Instance == null)
			{
				gameObject.AddComponent<InputController>();
			}

			if (RaycastController.Instance == null)
			{
				gameObject.AddComponent<RaycastController>();
			}
		}
	}
}