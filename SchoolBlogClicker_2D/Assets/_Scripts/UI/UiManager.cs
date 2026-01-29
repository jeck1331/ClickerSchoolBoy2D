using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour, ISubscribe
{
    private static readonly int Increment = Animator.StringToHash("increment");

    [SerializeField] private GameObject canvasUpgradeMenuUI;
    [SerializeField] private GameObject bottomButtonsUI;
    [SerializeField] private GameObject tapZoneGm;

    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private TMP_Text passiveIncomeText;

    [SerializeField] private Animator animator;

    [SerializeField] private ULongValue score;
    [SerializeField] private UIntValue clickPower;
    
    [SerializeField] private ObserverSo clickPowerObserver;
    [SerializeField] private ObserverSo scoreObserver;

    private void Awake()
    {
        (this as ISubscribe).Subscribes();
    }

    private void OnEnable()
    {
        (this as ISubscribe).Subscribes();
    }

    private void OnDisable()
    {
        (this as ISubscribe).Unsubscribes();
    }

    private void OnDestroy()
    {
        (this as ISubscribe).Unsubscribes();
    }

    private void UpdateTextCoinsText()
    {
        coinsText.text = score.Value / 1000 >= 1 ? $"{(score.Value / 1000f).ToString("0.0")}K" : score.Value.ToString();

        animator?.SetTrigger(Increment);
    }
    
    private void UpdatePowerText()
    {
        powerText.text = clickPower.Value.ToString();
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
        tapZoneGm.SetActive(false);
        canvasUpgradeMenuUI.SetActive(true);
        bottomButtonsUI.SetActive(false);
    }

    void ISubscribe.Subscribes()
    {
        scoreObserver.OnValueChanged += UpdateTextCoinsText;
        clickPowerObserver.OnValueChanged += UpdatePowerText;
    }

    void ISubscribe.Unsubscribes()
    {
        scoreObserver.OnValueChanged -= UpdateTextCoinsText;
        clickPowerObserver.OnValueChanged -= UpdatePowerText;
    }
}