using Assets.Scripts;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public event Action<uint> TapOnUpgrade;
    public event Action<ulong> TapOnReset;

    [SerializeField]
    private GameObject canvasUpgradeMenu;
    [SerializeField]
    private GameObject tapZone;

    private uint incrementValue = 1;

    private void Awake()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        uint.TryParse(PlayerPrefs.GetString(Constants.INCREMENT_VALUE), out uint playerIncrementValue);

        incrementValue = playerIncrementValue > 1 ? playerIncrementValue : 1;
    }

    public void ResetCount()
    {
        TapOnReset.Invoke(0);
    }

    public void Upgrade()
    {
        incrementValue *= 2;
        TapOnUpgrade.Invoke(incrementValue);
    }

    public void Downgrade()
    {
        incrementValue /= 2;
        TapOnUpgrade.Invoke(incrementValue);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenUpgradeMenu()
    {
        canvasUpgradeMenu.SetActive(true);
        gameObject.transform.parent.gameObject.SetActive(false);
        tapZone.SetActive(false);
    }
}
