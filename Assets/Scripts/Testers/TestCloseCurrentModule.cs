using UnityEngine;
using UnityEngine.SceneManagement;

public class TestCloseCurrentModule : MonoBehaviour
{
    public void CloseModule()
    {
        SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
}