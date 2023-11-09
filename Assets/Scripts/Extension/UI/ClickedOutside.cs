using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/Clicked Outside - Event Extension")]
public class ClickedOutside : MonoBehaviour
{
	[Serializable]
	public class OnClickedOutside : UnityEvent
	{
        
	}

	public GameObject WhenThisIsActive;
	public OnClickedOutside onClickedOutside = new OnClickedOutside();

	private Vector3 startPosition;
	private bool isSetStartPosition;

	public bool IgnoreGameObjects;
	public List<GameObject> IgnoredList;
	
	public void SetStartPosition()
	{
		startPosition = WhenThisIsActive.transform.position;
		isSetStartPosition = true;
	}
	
	private void Update()
	{
		if (!WhenThisIsActive.activeSelf)
			return;

		if (!IsEquals(startPosition, WhenThisIsActive.transform.position) && isSetStartPosition)
		{
			SendClickedOutsideEvent();
		}

        if (Input.GetMouseButtonUp(0))
		{
			if (EventSystem.current.currentSelectedGameObject != gameObject)
			{
				StartCoroutine(WaitEndOfFrame());
			}
		}
	}

	IEnumerator WaitEndOfFrame()
	{
		yield return new WaitForFixedUpdate();
		SendClickedOutsideEvent();
	}

	private void OnDisable()
	{
		if (WhenThisIsActive.activeSelf)
		{
			SendClickedOutsideEvent();
		}
	}

	private void SendClickedOutsideEvent()
	{
		if (CanSendEvent())
		{
			onClickedOutside.Invoke();
		}
	}

	private bool CanSendEvent()
	{
		if (EventSystem.current == null)
		{
			return false;
		}

		return !IgnoreGameObjects || !IgnoredList.Contains(EventSystem.current.currentSelectedGameObject);
	}

    private static bool IsEquals(Vector3 value1, Vector3 value2) {
        const float tolerance = 0.001f;
        var isNotValid = false;
        isNotValid |= !(Math.Abs(value1.x - value2.x) < tolerance);
        isNotValid |= !(Math.Abs(value1.y - value2.y) < tolerance);
        isNotValid |= !(Math.Abs(value1.z - value2.z) < tolerance);
        return !isNotValid;
    }
}
