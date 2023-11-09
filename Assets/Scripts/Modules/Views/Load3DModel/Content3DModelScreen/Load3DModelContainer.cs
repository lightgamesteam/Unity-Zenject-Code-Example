using UnityEngine;
using UnityEngine.SceneManagement;

public class Load3DModelContainer : ViewBase
{
	public void CloseModule()
	{
		SceneManager.UnloadSceneAsync(gameObject.scene.name);
	}
	
	public void UnloadModule()
	{
		if(gameObject.scene.isLoaded)
			SceneManager.UnloadScene(gameObject.scene);
	}

	private void OnDisable()
	{
		foreach (Transform tr in transform)
		{
			Destroy(tr.gameObject);
		}
	}
}