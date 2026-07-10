using _Scripts.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour, ISubscribe, IInitialize
{
    private static readonly int Increment = Animator.StringToHash("increment");

    [SerializeField] private GameObject canvasUpgradeMenuUI;
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

    // private void Awake()
    // {
    //     (this as ISubscribe).Subscribes();
    // }
    
    public void Initialize()
    {
        UpdatePowerText();
        UpdateIncomeText();
    }

    private void OnEnable()
    {
        (this as ISubscribe).Subscribes();
    }

    private void OnDisable()
    {
        (this as ISubscribe).Unsubscribes();
    }

    // private void OnDestroy()
    // {
    //     (this as ISubscribe).Unsubscribes();
    // }

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

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenUpgradeMenu()
    {
        var isOpen = !canvasUpgradeMenuUI.activeSelf;
        canvasUpgradeMenuUI.SetActive(isOpen);

        if (tapZoneGm != null)
            tapZoneGm.SetActive(true);

        GameplayPauseService.SetPaused(this, isOpen);
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
