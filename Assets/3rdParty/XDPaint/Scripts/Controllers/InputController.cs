using UnityEngine;
using UnityEngine.EventSystems;
using XDPaint.Tools;
using XDPaint.Tools.Raycast;

namespace XDPaint.Controllers
{
	public class InputController : Singleton<InputController>
	{
		public delegate void OnInputPosition(Vector3 position);
		public delegate void OnInputPositionHit(Vector3 position, Triangle triangle = null);
		public delegate void OnInputPositionHitPressure(Vector3 position, float pressure = 1.0f, Triangle triangle = null);
		
		public event OnInputPositionHit OnMouseHover;
		public event OnInputPositionHit OnMouseHoverWithHit;
		public event OnInputPositionHitPressure OnMouseDown;
		public event OnInputPositionHitPressure OnMouseDownWithHit;
		public event OnInputPositionHitPressure OnMouseButton;
		public event OnInputPositionHitPressure OnMouseButtonWithHit;
		public event OnInputPosition OnMouseUp;
		
		public Camera Camera { internal get; set; }
		private int _fingerId = -1;

		void Update()
		{
			if (Camera == null)
				return;
			
			MouseInput();
			TouchInput();
		}

		private void TouchInput()
		{
			if (Input.touchSupported)
			{
				foreach (var touch in Input.touches)
				{
					int id = touch.fingerId;
					if (EventSystem.current.IsPointerOverGameObject(id))
						return;
					
					Triangle triangle = null;
					Ray? ray = null;
					var pressure = 1f;
					if (Settings.Instance.pressureEnabled)
					{
						pressure = touch.pressure;
					}

					if (touch.phase == TouchPhase.Began && _fingerId == -1)
					{
						_fingerId = touch.fingerId;
						if (OnMouseDownWithHit != null)
						{
							ray = Camera.ScreenPointToRay(touch.position);
							RaycastController.Instance.Raycast(ray.Value, out triangle);
							OnMouseDownWithHit(Input.mousePosition, pressure, triangle);
						}

						if (OnMouseDown != null)
						{
							OnMouseDown(Input.mousePosition, pressure);
						}
					}

					if (touch.fingerId == _fingerId)
					{
						if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
						{
							if (OnMouseButtonWithHit != null)
							{
								if (ray == null)
								{
									ray = Camera.ScreenPointToRay(touch.position);
								}

								if (triangle == null)
								{
									RaycastController.Instance.Raycast(ray.Value, out triangle);
								}

								OnMouseButtonWithHit(Input.mousePosition, pressure, triangle);
							}

							if (OnMouseButton != null)
							{
								OnMouseButton(Input.mousePosition, pressure);
							}
						}

						if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
						{
							_fingerId = -1;
							if (OnMouseUp != null)
							{
								OnMouseUp(touch.position);
							}
						}
					}
				}
			}
		}

		private void MouseInput()
		{
			if (!Input.touchSupported || Input.mousePresent)
			{
				if (EventSystem.current.IsPointerOverGameObject())
					return;
				
				Triangle triangle = null;
				Ray? ray = null;
				if (OnMouseHover != null)
				{
					OnMouseHover(Input.mousePosition);
				}

				if (OnMouseHoverWithHit != null)
				{
					ray = Camera.ScreenPointToRay(Input.mousePosition);
					RaycastController.Instance.Raycast(ray.Value, out triangle);
					OnMouseHoverWithHit(Input.mousePosition, triangle);
				}

				if (Input.GetMouseButtonDown(0))
				{
					if (OnMouseDownWithHit != null && Camera.rect.width > 0 && Camera.rect.height > 0)
					{
						if (ray == null)
						{
							ray = Camera.ScreenPointToRay(Input.mousePosition);
						}

						if (triangle == null)
						{
							RaycastController.Instance.Raycast(ray.Value, out triangle);
						}

						OnMouseDownWithHit(Input.mousePosition, 1f, triangle);
					}

					if (OnMouseDown != null)
					{
						OnMouseDown(Input.mousePosition);
					}
				}

				if (Input.GetMouseButton(0))
				{
					if (OnMouseButtonWithHit != null && Camera.rect.width > 0 && Camera.rect.height > 0)
					{
						if (ray == null)
						{
							ray = Camera.ScreenPointToRay(Input.mousePosition);
						}

						if (triangle == null)
						{
							RaycastController.Instance.Raycast(ray.Value, out triangle);
						}

						OnMouseButtonWithHit(Input.mousePosition, 1f, triangle);
					}

					if (OnMouseButton != null)
					{
						OnMouseButton(Input.mousePosition);
					}
				}

				if (Input.GetMouseButtonUp(0))
				{
					if (OnMouseUp != null)
					{
						OnMouseUp(Input.mousePosition);
					}
				}
			}
		}
	}
}