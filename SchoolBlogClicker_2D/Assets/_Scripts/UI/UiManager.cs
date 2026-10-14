using _Scripts.Helpers;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour, ISubscribe, IInitialize
{
    private static readonly int Increment = Animator.StringToHash("increment");

    [SerializeField] private GameObject shopMenuUI;
    [SerializeField] private GameObject upgradeMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject miniGamesMenuUI;
    
    [SerializeField] private GameObject tapZoneGm;

    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private TMP_Text passiveIncomeText;

    [SerializeField] private Animator animator;

    [SerializeField] private ULongValue score;
    [SerializeField] private UIntValue clickPower;
    [SerializeField] private UIntValue incomePower;
    
    [SerializeField] private ObserverSO clickPowerObserver;
    [SerializeField] private ObserverSO incomePowerObserver;
    [SerializeField] private ObserverSO scoreObserver;
    
    public void Initialize()
    {
        UpdateTextCoinsText();
        UpdatePowerText();
        UpdateIncomeText();
        
        Debug.Log("Initialize UiManager");
    }

    private void OnEnable()
    {
        (this as ISubscribe).Subscribes();
    }

    private void OnDisable()
    {
        (this as ISubscribe).Unsubscribes();
        // Release this menu's pause when its scene is unloaded or the UI is disabled.
        GameplayPauseService.SetPaused(this, false);
    }

    private void UpdateTextCoinsText()
    {
        coinsText.text = score.Value.ScoreViewFromNumber();;

        animator?.SetTrigger(Increment);
    }
    
    private void UpdatePowerText()
    {
        powerText.text = clickPower.Value.ToString();
    }
    
    private void UpdateIncomeText()
    {
        passiveIncomeText.text = incomePower.Value.ToString();
    }

    public void ResetCount()
    {
        score.Value = 0;
    }

    public void Upgrade()
    {
        clickPower.Value *= 2;
    }

    public void Downgrade()
    {
        clickPower.Value = clickPower.Value > 1 ? clickPower.Value / 2 : clickPower.Value;
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
    
    public void OpenShopMenu() => ClickToMenu(shopMenuUI, new [] { upgradeMenuUI });

    public void OpenUpgradeMenu() => ClickToMenu(upgradeMenuUI, new [] { shopMenuUI });

    private void ClickToMenu(GameObject menu, [CanBeNull] GameObject[] otherMenus)
    {
        bool pauseNewState = !menu.activeSelf;
        
        if (otherMenus is { Length: > 0 })
            foreach (var otherMenu in otherMenus)
                otherMenu.SetActive(false);
        
        menu.SetActive(pauseNewState);

        if (tapZoneGm != null)
            tapZoneGm.SetActive(!pauseNewState);

        GameplayPauseService.SetPaused(this, pauseNewState);
    }

    public void OpenMiniGames()
    {
        if (miniGamesMenuUI == null) return;
        miniGamesMenuUI.SetActive(true);
        GameplayPauseService.SetPaused(this, true);
    }
    
    public void CloseMiniGames()
    {
        if (miniGamesMenuUI == null) return;
        miniGamesMenuUI.SetActive(false);
        CloseAnyMenu();
    }

    public void CloseAnyMenu()
    {
        tapZoneGm.SetActive(true);
        
        if (GameplayPauseService.IsGameplayInputPaused)
            GameplayPauseService.SetPaused(this, false);
    }

    void ISubscribe.Subscribes()
    {
        scoreObserver.OnValueChanged += UpdateTextCoinsText;
        clickPowerObserver.OnValueChanged += UpdatePowerText;
        incomePowerObserver.OnValueChanged += UpdateIncomeText;
    }

    void ISubscribe.Unsubscribes()
    {
        scoreObserver.OnValueChanged -= UpdateTextCoinsText;
        clickPowerObserver.OnValueChanged -= UpdatePowerText;
        incomePowerObserver.OnValueChanged -= UpdateIncomeText;
    }
}
