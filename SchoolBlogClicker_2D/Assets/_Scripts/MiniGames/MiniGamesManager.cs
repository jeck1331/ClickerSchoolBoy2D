using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGamesManager : MonoBehaviour
{
    [SerializeField] private GameObject miniGamesWindowUI;
    [SerializeField] private GameObject settingsWindowUI;

    public void OpenMiniGames()
    {
        if (miniGamesWindowUI == null) return;
        GameplayPauseService.SetPaused(this, true);
        miniGamesWindowUI.SetActive(true);
    }

    public void OpenSettings()
    {
        // SettingsPanel owns its pause through OnEnable and OnDisable.
        if (settingsWindowUI != null)
            settingsWindowUI.SetActive(true);
    }

    public void CloseMiniGames()
    {
        GameplayPauseService.SetPaused(this, false);
        if (miniGamesWindowUI != null)
            miniGamesWindowUI.SetActive(false);
    }
    
    public void CloseSettings()
    {
        if (settingsWindowUI != null)
            settingsWindowUI.SetActive(false);
    }

    private void OnDisable()
    {
        GameplayPauseService.SetPaused(this, false);
    }

    public void ExitMiniGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
