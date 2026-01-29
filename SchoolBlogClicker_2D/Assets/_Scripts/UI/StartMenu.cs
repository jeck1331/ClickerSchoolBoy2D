using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void RunGameScene()
    {
        SceneManager.LoadScene(1);
    }

    public void RunSettings()
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}