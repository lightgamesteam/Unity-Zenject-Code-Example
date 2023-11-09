/* ========================================================================================================
 * 70:30 SmoothOrbitCam Script - created by D.Michalke / 70:30 / http://70-30.de / info@70-30.de
 * used to orbit smoothly around an object! drag and drop on your camera and drag the targed object on the target slot
 * ========================================================================================================
 */

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.EventSystems;

//add a menu to Component in Unity
[AddComponentMenu("Camera-Control/SmoothOrbitCam")]

public class SmoothOrbitCam : MonoBehaviour
{
    public bool interactable = true;
    
	//transform to drag and drop the target object
	public Transform target;

    //useable or not, used for viewchanger
    [HideInInspector]
    public bool useable = true;

    [Space(20)]
    [Header("Orbiting")]
    //enable orbiting. deactivate if you need a pan-only cam (i.e. strategy games etc.)
    public bool EnableOrbiting = true;

    [Tooltip("Choose a key for orbiting. Default is Mouse1.")]
    //the key for orbiting
    public KeyCode orbitKey;

    [Tooltip("Orbiting speed.")]
    //add speed variables for orbiting speed
    public float xSpeed = 10.0f;
    public float ySpeed = 10.0f;

    [Tooltip("The starting distance to the target object.")]
    //add the distance variable for zooming in and out with the mouse wheel
    public float distance = 5.0f;
    private float lerpDistance = 0;

    [Tooltip("The limits for the zoom distance.")]
    //add limits for the zoom distance
    public float distanceMin = 3f;
    public float distanceMax = 15f;

    [Tooltip("Limits the orbiting to the X or Y axis.")]
    //enable or disable axes
    public bool limitToXAxis = false;
	public bool limitToYAxis = false;

    [Tooltip("The limits for the rotation axes.")]
    //add limits for the rotation axes
    public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	public float xMinLimit = -360;
	public float xMaxLimit = 360;
    private float storedLimit = 0;

    [Tooltip("The amount of smooth-out-effect.")]
    //add the smoothing variable
    public float smoothTime = 2f;

    //define the rotation axes
    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;
    [HideInInspector]
    public Quaternion rotation;

    //define the main velocity
    [HideInInspector]
    public float velocityX = 0.0f;
    [HideInInspector]
    public float velocityY = 0.0f;

    [Space(20)]
    [Header("Zooming")]

    [Tooltip("Zooming works via mouse wheel on desktop and 2-finger-pinch-gesture on mobile.")]
    //zoom
    public bool enableZooming = true;
    //add a modifyer for zooming in and out (for both touch and mouse)
    [Tooltip("The zooming speed.")]
    public float zoomSpeed = 1;

    [Space(20)]
    [Header("Panning")]
    //enable panning - for panning, the camera has to be assigned to target pan cam and the smoothorbitcam.cs script has to be on a parent object with the camera as child object!
    public bool enablePanning = false;
    [HideInInspector]
    public GameObject targetPanCam;

    [Tooltip("Choose a key for panning. Default is Mouse2.")]
    public KeyCode panKey;

    [Tooltip("Panning speed.")]
    public float panSpeed = 1;
    [Tooltip("Limit panning (Add invisible borders).")]
    public bool LimitPan = false;
    [Tooltip("Enter the panning limits in units here if required.")]
    public Vector2 PanLimitsLeftRight;
    public Vector2 PanLimitsUpDown;

    [Space(20)]
    [Header("Miscellaneous")]

    [Tooltip("Offset to the center of the focus point (sometimes useful, if the pivot of a 3D-object is not centered).")]
    //add offset variables to get more control over the cam
    public float xOffset;
	public float yOffset;

    [Tooltip("Enables automatic orbiting.")]
    //automatic orbiting
    public bool enableAutomaticOrbiting = false;
    [Tooltip("Automatic orbiting speed. Use negative values for opposite orbiting direction.")]
    public float orbitingSpeed = 1f;

    [Tooltip("If you want the camera to move in front of objects that are in the way to the target object, set this to true. Default value is false.")]
    //for objects that might be between the target object and the cam
    public bool NoObjectsBetween = false;

    [Tooltip("Keeps the camera above the ground (beta).")]
    //the minimum distance the cam stays away from a possible ground (for RPGs, racing games, etc to keep the cam in a good position)
    public bool EnableGroundHovering = true;
    public float GroundHoverDistance = 5;

    [Tooltip("If you want UI Elements to block the orbiting camera interaction, set this to true.")]
    //if ui should block interaction with orbit cam
    public bool UiBlocksInteraction = false;
	private bool uiBlocking = false;

    [Space(20)]
    [Header("Input")]
    [Tooltip("Force the OrbitCam to use mouse or touch if i.e. your device is a desktop device but uses mobile controlling (touch)")]
    public InputType InputSelection = InputType.AUTOMATIC;
    public enum InputType { AUTOMATIC, MOUSE, TOUCH}

    public bool isBlockUserInputOnStart = true;

    //temporary pan position and speed values
    [HideInInspector]
	public Vector3 tempPanPosition;
	[HideInInspector]
	public float velocityPanX;
	[HideInInspector]
	public float velocityPanY;

    private bool doOrbit = false;

    //get the event system
    private EventSystem eventSystem;

    [HideInInspector] public float ResetCameraDistance;
    [HideInInspector] public Vector3 ResetCameraPosition;
    [HideInInspector] public Vector3 ResetCameraAngles;

    protected Camera DefaultCamera;
    protected Transform DefaultCameraTarget;
    protected float DefaultCameraDistance;
    protected Vector3 DefaultCameraPosition;
    protected Vector3 DefaultCameraAngles;

    private Vector3 offsetCameraPosition;
    private static bool _blockUserInput;
    private static bool _blockUserKeyInput;

    private bool _isGizmos;
    private Vector3 _triangleCenter;
    private Vector3 _normalCenter;
    private Vector3[] _trianglePoints;
    private Vector3[] _normalPoints;

	void Start()
	{
	    DefaultCameraTarget = target;
	    DefaultCameraPosition = ResetCameraPosition = offsetCameraPosition = default;

		//define the angle vector3 and assign the axes
	    DefaultCameraAngles = ResetCameraAngles = transform.eulerAngles;
		rotationYAxis = DefaultCameraAngles.y;
		rotationXAxis = DefaultCameraAngles.x;

        //distance application
	    lerpDistance = distance;
	    DefaultCameraDistance = ResetCameraDistance = distance;
	    //_blockUserInput = true;
	    BlockUserInput(isBlockUserInputOnStart);
		
		// ensure the rigid body does not change rotation
		if (GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}

        //set stored limit to defined limit first
        storedLimit = yMinLimit;

	    DefaultCamera = GetComponentInChildren<Camera>();

        //get pan cam
        targetPanCam = GetComponentInChildren<Camera>().gameObject;

        //get eventsystem
        if (EventSystem.current != null)
        {
            eventSystem = EventSystem.current;
        }
	}

    public Camera GetMyCamera()
    {
        return DefaultCamera;
    }

    public void SetZoomDistance(float dis)
    {
        ResetCameraDistance = dis;
        distance = dis;
    }

    //to get the current rotation before normal orbit cam mode is active again
    public void ResetValues() {
        offsetCameraPosition = ResetCameraPosition;
        distance = ResetCameraDistance;
        //define the angle vector3 and assign the axes
        rotationYAxis = ResetCameraAngles.y;
        rotationXAxis = ResetCameraAngles.x;
        velocityX = 0f;
        velocityY = 0f;
        velocityPanX = 0f;//-(tempPanPosition.x / (distance / 10));
        velocityPanY = 0f;//-(tempPanPosition.y / (distance / 10));

        _isGizmos = false;
    }
    
    public void ResetMainValues() 
    {
        distance = ResetCameraDistance;
        //define the angle vector3 and assign the axes
        rotationYAxis = ResetCameraAngles.y;
        rotationXAxis = ResetCameraAngles.x;
        velocityX = 0f;
        velocityY = 0f;
        velocityPanX = 0f;//-(tempPanPosition.x / (distance / 10));
        velocityPanY = 0f;//-(tempPanPosition.y / (distance / 10));

        _isGizmos = false;
    }

    public void Zoom(float value) {
        if (enableZooming) {
            distance = Mathf.Clamp(distance - value, distanceMin, distanceMax);
        }
    }

    public void DebugGizmos(Vector3 triangleCenter = default, Vector3 normalCenter = default, Vector3[] trianglePoints = default, Vector3[] normalPoints = default) {
        _triangleCenter = triangleCenter;
        _normalCenter = normalCenter;
        _trianglePoints = trianglePoints;
        _normalPoints = normalPoints;
        _isGizmos = true;
    }
    
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = new Color(0.8f, 1, 0, 0.3f);
            Bounds b = target.gameObject.GetBounds();
            Gizmos.DrawCube(target.position, b.size);
        }
    }
#endif

    public void SetDefaultValue(float distance, Vector3 position, Vector3 angle)
    {
        DefaultCameraDistance = ResetCameraDistance = distance;
        DefaultCameraPosition = ResetCameraPosition = position;
        DefaultCameraAngles = ResetCameraAngles = angle;
    }
    
    public void AutoZoomOnTarget(bool applyNewZoomDistance = false, bool fastCrop = false)
    {
        if(DefaultCamera == null)
            DefaultCamera = GetComponentInChildren<Camera>();

        if (target != null)
        {
            this.enabled = true;
            Bounds value = target.gameObject.GetBounds();
            
            var contentWidth = DefaultCamera.rect.width * Screen.width;
            var contentHeight = DefaultCamera.rect.height * Screen.height;
            var contentRatio = contentWidth / contentHeight;

            var frustumHeight = 2.0f * Mathf.Tan(DefaultCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var frustumWidth = frustumHeight * contentRatio;

            var frustumModelWidth = value.size.x + frustumWidth;
            var frustumModelHeight = value.size.y + frustumHeight;
            var targetRatio = frustumModelWidth / frustumModelHeight;

            float differenceInSize;
            if (contentRatio < targetRatio) {
                differenceInSize = frustumModelHeight * 0.5f * (targetRatio / contentRatio);
            } else {
                differenceInSize = frustumModelHeight * 0.5f;
            }

            float zoomDistance = Mathf.Clamp(differenceInSize * 2.14f, distanceMin, distanceMax);

            if (fastCrop)
            {
                
                ResetCameraDistance = zoomDistance;
                ResetCameraAngles = DefaultCameraAngles;
                ResetCameraPosition = DefaultCameraPosition;
                
                DefaultCamera.transform.SetPositionZ(-zoomDistance);
            }
            else
            {
                if (applyNewZoomDistance)
                {
                    ResetCameraDistance = zoomDistance;
                    ResetCameraAngles = DefaultCameraAngles;
                    ResetCameraPosition = DefaultCameraPosition;
                    ResetTarget();
                    ResetMainValues();
                }
                else
                {
                    distance = zoomDistance;
                }
            }
        }
    }

    public void FocusModel() {
        if (target != null) {
            FocusModel(target, target.gameObject.GetBounds());
        }
    }

    public void FocusModel(Transform model, Bounds value) {
        //var valueWithScale = model.rotation * new Vector3(
        //    value.size.x * model.lossyScale.x, 
        //    value.size.y * model.lossyScale.y, 
        //    value.size.z * model.lossyScale.z);
        //valueWithScale = new Vector3(
        //    Mathf.Abs(valueWithScale.x),
        //    Mathf.Abs(valueWithScale.y),
        //    Mathf.Abs(valueWithScale.z));
        
        var contentWidth = DefaultCamera.rect.width * Screen.width;
        var contentHeight = DefaultCamera.rect.height * Screen.height;
        var contentRatio = contentWidth / contentHeight;

        var frustumHeight = 2.0f * Mathf.Tan(DefaultCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * contentRatio;

        var frustumModelWidth = value.size.x + frustumWidth;
        var frustumModelHeight = value.size.y + frustumHeight;
        var targetRatio = frustumModelWidth / frustumModelHeight;

        float differenceInSize;
        if (contentRatio < targetRatio) {
            differenceInSize = frustumModelHeight * 0.5f * (targetRatio / contentRatio);
            //differenceInSize = frustumModelHeight * (targetRatio / contentRatio);
        } else {
            differenceInSize = frustumModelHeight * 0.5f;
            //differenceInSize = frustumModelHeight;
        }
        ResetCameraDistance = Mathf.Clamp(differenceInSize * 1.5f, distanceMin, distanceMax);
        //ResetCameraDistance = Mathf.Clamp(differenceInSize, distanceMin, distanceMax);
        ResetCameraAngles = DefaultCameraAngles;
        ResetCameraPosition = DefaultCameraPosition;
        ResetTarget();
        ResetValues();
    }

    public void Focus(Transform partTarget, Bounds value, Vector3 direction) {
        SetTarget(partTarget);
        var cameraPosition = partTarget.rotation * new Vector3(
            value.center.x * partTarget.lossyScale.x,
            value.center.y * partTarget.lossyScale.y,
            value.center.z * partTarget.lossyScale.z);
        var ratio = Mathf.Max(
            value.size.x * target.lossyScale.x, 
            value.size.y * target.lossyScale.y, 
            value.size.z * target.lossyScale.z);
        ResetCameraDistance = Mathf.Clamp(ratio * 1.5f, distanceMin, distanceMax);
        ResetCameraAngles = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
        ResetCameraPosition = cameraPosition;
        ResetValues();
    }

    public void ResetFocus() {
        ResetCameraDistance = DefaultCameraDistance;
        ResetCameraAngles = DefaultCameraAngles;
        ResetCameraPosition = DefaultCameraPosition;
        ResetTarget();
        ResetValues();
    }

    private void ResetTarget() {
        SetTarget(DefaultCameraTarget);
    }

    private void SetTarget(Transform selectTarget) {
        target = selectTarget;
    }

    void OnDrawGizmos() {
        if (_isGizmos) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_triangleCenter, 0.075f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_normalCenter, 0.075f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(_triangleCenter, _normalCenter);

            if (_trianglePoints != null) {
                Gizmos.color = Color.black;
                foreach (var vector3 in _trianglePoints) {
                    Gizmos.DrawSphere(vector3, 0.01f);
                }
            }

            if (_normalPoints != null) {
                Gizmos.color = Color.magenta;
                foreach (var vector3 in _normalPoints) {
                    Gizmos.DrawSphere(vector3, 0.01f);
                }
            }

            if (_trianglePoints != null && _normalPoints != null && _trianglePoints.Length == _normalPoints.Length) {
                Gizmos.color = Color.red;
                for (int i = 0; i < _trianglePoints.Length; i++) {
                    Gizmos.DrawLine(_trianglePoints[i], _normalPoints[i]);
                }
            }
        }
    }

    void Update()
    {
        if(!interactable)
            return;
        
        if (!_blockUserKeyInput)
        {
            KeyInput();
        }

        //check if UI is blocking
        if (eventSystem != null)
        {
            if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject(1) || EventSystem.current.IsPointerOverGameObject(2))
            {
                uiBlocking = true;
            }
            else
            {
                uiBlocking = false;
            }
        }
    }

    private void KeyInput()
    {
        //pan
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
//        if(Mathf.Abs(h) > 0)
//            velocityPanX = Mathf.Clamp(velocityPanX + panSpeed * h * 0.1f, PanLimitsLeftRight.x, PanLimitsLeftRight.y);
//
//        if (Mathf.Abs(v) > 0)
//            velocityPanY = Mathf.Clamp(velocityPanY + panSpeed * v * 0.1f, PanLimitsUpDown.x, PanLimitsUpDown.y);
        
        //clamp the rotation by the set limits and assign the rotation to the x axis
        if (Mathf.Abs(h) > 0)//(yMinLimit != 0 || yMaxLimit != 0)
        {
            velocityX += xSpeed * h * 0.04f;
            //velocityX = Mathf.Clamp(velocityX + h * 180, yMinLimit, yMaxLimit);
        }
    
        if(Mathf.Abs(v) > 0) //(xMinLimit != 0 || xMaxLimit != 0) 
        {
            //velocityY = Mathf.Clamp(velocityY + v * 180, xMinLimit, xMaxLimit);
            velocityY += ySpeed * v * 0.04f;

        }
        
        //rotate
        if(Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus))
            distance = Mathf.Clamp(distance + 0.01f * zoomSpeed * 5, distanceMin, distanceMax);
		
        if(Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Equals))
            distance = Mathf.Clamp(distance - 0.01f  * zoomSpeed * 5, distanceMin, distanceMax);

        //reset
        if (Input.GetKeyDown(KeyCode.Backspace))
            ResetValues();
    }

    void CalcPan() {
       
        if (!_blockUserInput)
        //function to calculate pan values 
        //on mouse down, calculate the orbital velocity with the given speed values and the axes
        if (UiBlocksInteraction)
        {
            if (!uiBlocking)
            {
                velocityPanX += panSpeed * Input.GetAxis("Mouse X") * 0.2f;
                velocityPanY += panSpeed * Input.GetAxis("Mouse Y") * 0.2f;
            }
        }
        else
        {
            velocityPanX += panSpeed * Input.GetAxis("Mouse X") * 0.2f;
            velocityPanY += panSpeed * Input.GetAxis("Mouse Y") * 0.2f;
        }
    }

    void CalcPanMobile(float veloDeltaX, float veloDeltaY)
    {
        if (!_blockUserInput)
            if (UiBlocksInteraction)
            {
                if (!uiBlocking)
                {
                    velocityPanX += panSpeed * veloDeltaX * 0.2f;
                    velocityPanY += panSpeed * veloDeltaY * 0.2f;
                }
            }
            else
            {
                velocityPanX += panSpeed * veloDeltaX * 0.2f;
                velocityPanY += panSpeed * veloDeltaY * 0.2f;
            }
    }

    void LateUpdate() 
    {
        if(!interactable)
            return;
        
        //only if the target exists/is assigned, perform the orbit
        if (target) {
            //orbit settings
            ToOrbit(InputSelection);
            //zoom settings
            ToZoom(InputSelection);

            //give the calculated values to the rotation axes
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;

            //clamp the rotation by the set limits and assign the rotation to the x axis
            if (yMinLimit != 0 || yMaxLimit != 0) {
                rotationXAxis = Mathf.Clamp(rotationXAxis, yMinLimit, yMaxLimit);
            }
            if (xMinLimit != 0 || xMaxLimit != 0) {
                rotationYAxis = Mathf.Clamp(rotationYAxis, xMinLimit, xMaxLimit);
            }

            //define the target rotation (including the calculated rotation axes)
            var toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            //give over the rotation
            if (useable && EnableOrbiting){
                rotation = toRotation;
            }

            //pan settings
            ToPan(InputSelection);

            //include a raycast for potential other objects (between the target and the cam) obscuring the view
            if (NoObjectsBetween) {
                RaycastHit hit;
                if (Physics.Linecast(target.position, transform.position, out hit)) {
                    var tempDistance = distance;
                    tempDistance -= hit.distance;
                    if (tempDistance < distanceMin) {
                        tempDistance = distanceMin;
                    }
                    distance = Mathf.Lerp(distance, tempDistance, Time.deltaTime * smoothTime * 0.5f);
                }
            }

            //ground hovering: CURRENTLY IN DEVELOPMENT FOR MORE SMOOTHNESS
            if (EnableGroundHovering) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position,Vector3.down, out hit)) {
                    var storedAngle = rotationXAxis;
                    if (hit.distance > GroundHoverDistance) {
                        yMinLimit = storedLimit;
                    }
                    if (hit.distance < GroundHoverDistance) {
                        yMinLimit = storedAngle;
                        rotationXAxis = storedAngle;
                    }

                }
            }

            //set the temporary position
            if (target != null) {
                //lerp the distance
                lerpDistance = Mathf.Lerp(lerpDistance, distance, smoothTime * Time.deltaTime);
                //create the inverted distance to move the cam away from the object
                var negDistance = new Vector3(0.0f, 0.0f, -lerpDistance);
                var position = rotation * negDistance + target.position + offsetCameraPosition;
                //create yet another vec3 to include the defined offset
                var offsetPosition = new Vector3(position.x + xOffset, position.y + yOffset, position.z);
                //finally set the transform by giving over the temporary position/rotation to the object transform
                transform.rotation = rotation;
                transform.position = offsetPosition;
            }

            //orbiting mode
            if (enableAutomaticOrbiting) {
                velocityX = Mathf.Lerp(velocityX,orbitingSpeed, Time.deltaTime * smoothTime);
            }

            //assign the smoothing effect to the velocity with lerp
            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

            //panning
            tempPanPosition = new Vector3(-velocityPanX * distance / 10, -velocityPanY * distance / 10, 0);

            //apply panning
            if (targetPanCam != null && useable && enablePanning) {
                targetPanCam.transform.localPosition = Vector3.Lerp(targetPanCam.transform.localPosition, tempPanPosition, Time.deltaTime * smoothTime * 1.5f);
                //pan limitations
                if (LimitPan) {
                    var clampX = Mathf.Clamp(targetPanCam.transform.localPosition.x, PanLimitsLeftRight.x, PanLimitsLeftRight.y);
                    var clampY = Mathf.Clamp(targetPanCam.transform.localPosition.y, PanLimitsUpDown.x, PanLimitsUpDown.y);
                    targetPanCam.transform.localPosition = new Vector3(clampX, clampY, targetPanCam.transform.localPosition.z);
                }
            }
        }
    }

    public static void OnBlockUser() {
        _blockUserInput = true;
    }

    public static void OnUnblockUser() {
        _blockUserInput = false;
    }

    public void BlockUserInput(bool isBlock) 
    {
	    _blockUserInput = isBlock;
    }

    public void BlockUserKeyInput(bool isBlock)
    {
        _blockUserKeyInput = isBlock;
    }

    #region Private methods

    private void ToOrbit(InputType inputType) {
        switch (inputType) {
            case InputType.MOUSE: {
                ToOrbitMouse();
                break;
            }
            case InputType.TOUCH: {
                ToOrbitTouch();
                break;
            }
            default: {
                if (Input.touchCount == 0) {
                    ToOrbitMouse();
                } else {
                    ToOrbitTouch();
                }
                break;
            }
        }
    }

    private void ToZoom(InputType inputType) {
        switch (inputType) {
            case InputType.MOUSE: {
                ToZoomMouse();
                break;
            }
            case InputType.TOUCH: {
                ToZoomTouch();
                break;
            }
            default: {
                if (Input.touchCount == 0) {
                    ToZoomMouse();
                } else {
                    ToZoomTouch();
                }
                break;
            }
        }
    }

    private void ToPan(InputType inputType) {
        switch (inputType) {
            case InputType.MOUSE: {
                ToPanMouse();
                break;
            }
            case InputType.TOUCH: {
                ToPanTouch();
                break;
            }
            default: {
                if (Input.touchCount == 0) {
                    ToPanMouse();
                } else {
                    ToPanTouch();
                }
                break;
            }
        }
    }

    #region Mouse

    private void ToOrbitMouse() {
        var isNotValid = false;
        isNotValid |= !Input.GetKey(orbitKey);
        isNotValid |= _blockUserInput;
        isNotValid |= UiBlocksInteraction && (!UiBlocksInteraction || uiBlocking);
        if (!isNotValid) {
            if (!limitToXAxis) {
                velocityX += xSpeed * Input.GetAxis("Mouse X") * 0.2f;
            }
            if (!limitToYAxis) {
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.2f;
            }
        }
    }

    private void ToZoomMouse() {
        if (enableZooming && !uiBlocking) {
            var axisValue = Input.GetAxis("Mouse ScrollWheel");
            distance = Mathf.Clamp(distance - axisValue * zoomSpeed * 5, distanceMin, distanceMax);
        }
    }

    private void ToPanMouse() {
        var isNotValid = false;
        isNotValid |= !useable;
        isNotValid |= !Input.GetKey(panKey);
        isNotValid |= !enablePanning;
        if (!isNotValid) {
            CalcPan();
        }
    }

    #endregion

    #region Touch

    private void ToOrbitTouch() {
        var isNotValid = false;
        isNotValid |= Input.touchCount != 1;
        isNotValid |= _blockUserInput;
        isNotValid |= UiBlocksInteraction && (!UiBlocksInteraction || uiBlocking);
        if (!isNotValid) {
            var touch = Input.GetTouch(0);
            var fixDeltaTime = Time.deltaTime / (touch.deltaTime + 0.001f) * 0.01f;
            if (!limitToXAxis) {
                velocityX += xSpeed * touch.deltaPosition.x * fixDeltaTime;
            }
            if (!limitToYAxis) {
                velocityY += ySpeed * touch.deltaPosition.y * fixDeltaTime;
            }
        }
    }

    private void ToZoomTouch() {
        var isNotValid = false;
        isNotValid |= Input.touchCount != 2;
        isNotValid |= !enableZooming;
        isNotValid |= _blockUserInput;
        isNotValid |= UiBlocksInteraction && (!UiBlocksInteraction || uiBlocking);
        if (!isNotValid) {
            // Store both touches.
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);
            // Find the position in the previous frame of each touch.
            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            // Find the magnitude of the vector (the distance) between the touches in each frame.
            var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            // Find the difference in the distances between each frame.
            var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            var clampDeltaMagnitude = Mathf.Clamp(distance + deltaMagnitudeDiff * 100000 * zoomSpeed, distanceMin, distanceMax);
            distance = Mathf.Lerp(distance, clampDeltaMagnitude, Time.deltaTime * smoothTime * 0.1f * zoomSpeed); //FIXME
        }
    }

    private void ToPanTouch() {
        var isNotValid = false;
        isNotValid |= Input.touchCount != 2;
        isNotValid |= !enablePanning;
        isNotValid |= !useable;
        isNotValid |= panKey == KeyCode.Mouse0;
        if (!isNotValid) {
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);
            var fix0DeltaTime = Time.deltaTime / (touch0.deltaTime + 0.001f) * 0.05f;
            var fix1DeltaTime = Time.deltaTime / (touch1.deltaTime + 0.001f) * 0.05f;
            //on touch down calculate the velocities from the input touch position and the modifiers
            var tempVelocityXa = panSpeed * touch0.deltaPosition.x * fix0DeltaTime;
            var tempVelocityXb = panSpeed * touch1.deltaPosition.x * fix1DeltaTime;
            var tempVelocityYa = panSpeed * touch0.deltaPosition.y * fix0DeltaTime;
            var tempVelocityYb = panSpeed * touch1.deltaPosition.y * fix1DeltaTime;
            var velocityDeltaX = (tempVelocityXa + tempVelocityXb) / 2;
            var velocityDeltaY = (tempVelocityYa + tempVelocityYb) / 2;
            CalcPanMobile(velocityDeltaX, velocityDeltaY);
        }

        isNotValid = false;
        isNotValid |= Input.touchCount != 1;
        isNotValid |= !enablePanning;
        isNotValid |= !useable;
        isNotValid |= panKey != KeyCode.Mouse0;
        if (!isNotValid) {
            var touch0 = Input.GetTouch(0);
            var fix0DeltaTime = Time.deltaTime / (touch0.deltaTime + 0.001f) * 0.05f;
            var velocityDeltaX = panSpeed * touch0.deltaPosition.x * fix0DeltaTime;
            var velocityDeltaY = panSpeed * touch0.deltaPosition.y * fix0DeltaTime;
            CalcPanMobile(velocityDeltaX, velocityDeltaY);
        }
    }

    #endregion

    #endregion
}
