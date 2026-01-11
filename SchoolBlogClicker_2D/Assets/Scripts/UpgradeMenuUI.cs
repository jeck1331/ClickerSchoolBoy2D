using Assets.Scripts;
using TMPro;
using UnityEngine;

public class UpgradeMenuUI : MonoBehaviour
{
    private ulong gameCount = 0;
    private uint subscribersCount = 0;
    private uint passiveIncome = 0;

    [SerializeField]
    private GameObject canvasGame;
    [SerializeField]
    private GameObject tapZone;
    [SerializeField]
    private GameState gameState;

    [SerializeField]
    private TMP_Text CountText;
    [SerializeField]
    private TMP_Text SubscribersText;
    [SerializeField]
    private TMP_Text CountPerSecText;


    private void Awake()
    {
        gameState.UpdateState += UpdateState;
    }

    private void Start()
    {
        ulong.TryParse(PlayerPrefs.GetString(Constants.GAME_COUNT), out ulong playerGameCount);
        uint.TryParse(PlayerPrefs.GetString(Constants.SUBS_COUNT), out uint playerSubsCount);
        uint.TryParse(PlayerPrefs.GetString(Constants.PASSIVE_INCOMVE_COUNT), out uint playerPassiveIncomeCount);

        gameCount = playerGameCount > 0 ? playerGameCount : 0;
        subscribersCount = playerSubsCount > 0 ? playerSubsCount : 0;
        passiveIncome = playerPassiveIncomeCount > 0 ? playerPassiveIncomeCount : 0;

        UpdateState(Constants.GAME_COUNT, gameCount.ToString());
        UpdateState(Constants.SUBS_COUNT, subscribersCount.ToString());
        UpdateState(Constants.PASSIVE_INCOMVE_COUNT, passiveIncome.ToString());
    }

    public void OnExitUpgradeMenu()
    {
        canvasGame.SetActive(true);
        tapZone.SetActive(true);
        gameObject.SetActive(false);
    }

    private void UpdateState(string name, string value)
    {
        switch(name)
        {
            case Constants.GAME_COUNT:
                gameCount = ulong.Parse(value);
                CountText.text = value;
                break;
            case Constants.PASSIVE_INCOMVE_COUNT:
                passiveIncome = uint.Parse(value);
                CountPerSecText.text = value;
                break;
            case Constants.SUBS_COUNT:
                subscribersCount = uint.Parse(value);
                SubscribersText.text = value;
                break;
            default:
                break;
        }
    }
}
