using UnityEngine;

namespace XDPaint.Tools
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Instance { get; private set; }

		protected void Awake()
		{
			CacheInstance();
		}

		protected void OnEnable()
		{
			CacheInstance();
		}

		private void CacheInstance()
		{
			if (Instance == null)
			{
				Instance = GetComponent<T>();
			}
		}
	}
}