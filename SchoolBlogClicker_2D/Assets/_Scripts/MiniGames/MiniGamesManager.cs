using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGamesManager : MonoBehaviour
{
    [SerializeField] private GameObject miniGamesWindowUI;
    [SerializeField] private GameObject settingsWindowUI;

    public void OpenMiniGames()
    {
        GameplayPauseService.SetPaused(this, true);
        if (miniGamesWindowUI != null)
            miniGamesWindowUI.SetActive(true);
    }

    public void OpenSettings()
    {
        GameplayPauseService.SetPaused(this, true);
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
        GameplayPauseService.SetPaused(this, false);
        if (settingsWindowUI != null)
            settingsWindowUI.SetActive(false);
    }

    public void ExitMiniGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}