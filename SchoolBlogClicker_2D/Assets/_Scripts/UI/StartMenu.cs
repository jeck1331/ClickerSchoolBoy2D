using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenuUI;

    public void RunGameScene()
    {
        SceneManager.LoadScene(1);
    }
    
    public void OpenSettings()
    {
        if (settingsMenuUI == null) return;
        var settingsPanel = settingsMenuUI.GetComponent<SettingsPanel>();
        if (settingsPanel != null)
            settingsPanel.OpenSettings();
        else
            settingsMenuUI.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}