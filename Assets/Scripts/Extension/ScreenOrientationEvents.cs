using System;
using UnityEngine;
using UnityEngine.Events;

public class ScreenOrientationEvents : MonoBehaviour
{
	public bool invokeOnRectTransformDimensionsChange = true;
	public bool invokeOnAwake = false;
	public bool invokeOnEnable = false;
	public bool invokeOnValidateInEditor = false;

	[Space]
	public UnityEvent onPortraitOrientation;
	public UnityEvent onLandscapeOrientation;

	private void Awake()
	{
		if(invokeOnAwake)
			CheckOrientation();
	}

	private void OnEnable()
	{
		if(invokeOnEnable)
			CheckOrientation();
	}

	private void OnRectTransformDimensionsChange()
	{
		if (invokeOnRectTransformDimensionsChange)
			CheckOrientation();
	}

	private void OnValidate()
	{
		if (invokeOnValidateInEditor)
			CheckOrientation();
	}

	private void CheckOrientation()
	{
		if (Screen.height > Screen.width)
		{
			onPortraitOrientation.Invoke();
		}
		else
		{
			onLandscapeOrientation.Invoke();
		}
	}
}
