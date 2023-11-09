using UnityEngine;
using UnityEngine.Events;

public class ExecutionEvents : MonoBehaviour {

	public UnityEvent awake;
	public UnityEvent onEnable;
	public UnityEvent start;
	public UnityEvent onDisable;
	public UnityEvent onDestroy;
	public UnityEvent onRectTransformDimensionsChange;

	private void Awake()
	{
		awake.Invoke();
	}

	private void OnEnable()
	{
		onEnable.Invoke();
	}

	private void Start ()
	{
		start.Invoke();
	}

	private void OnDisable()
	{
		onDisable.Invoke();
	}

	private void OnDestroy()
	{
		onDestroy.Invoke();
	}

	private void OnRectTransformDimensionsChange()
	{
		onRectTransformDimensionsChange.Invoke();
	}
}
