using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class OnEnableRebuildLayout : MonoBehaviour 
{
	private void OnValidate()
	{
		RebuildLayout();
	}

	private void OnEnable()
	{
		RebuildLayout();
	}

	public void RebuildLayout()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
	}

	public void DelayRebuildLayout()
	{
		StartCoroutine(DelayRebuild());
	}

	IEnumerator DelayRebuild()
	{
		yield return new WaitForEndOfFrame();
		RebuildLayout();
	}
}
