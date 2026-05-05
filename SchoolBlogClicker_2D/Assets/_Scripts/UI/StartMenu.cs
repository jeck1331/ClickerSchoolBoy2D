using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private SettingsMenuUI settingsMenu;

    public void RunGameScene()
    {
        SceneManager.LoadScene(1);
    }

    public void RunSettings()
    {
        if (settingsMenu != null)
            settingsMenu.Open();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}