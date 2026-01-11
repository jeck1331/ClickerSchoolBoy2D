using Assets.Scripts;
using System;
using TMPro;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private ulong gameCount = 0;
    private uint subscribersCount = 0;

    [SerializeField]
    private Animator animCountText;

    private uint passiveIncome = 0;

    [SerializeField]
    private GameObject GM_CountText;
    [SerializeField]
    private GameObject GM_SubscribersText;

    private TMP_Text CountText;
    private TMP_Text SubscribersText;

    private TapController tapController;

    [SerializeField]
    private GameUI gameUI;

    public event Action<string, string> UpdateState;

    void Awake()
    {
        tapController = GameObject.Find("TapZone")?.GetComponent<TapController>() ?? new TapController();
        tapController.TapOnBox += onGameStateCountChange;
        gameUI.TapOnUpgrade += onGameStateIncrementValueChange;
        gameUI.TapOnReset += onGameStateCountChange;

        CountText = GM_CountText.GetComponent<TMP_Text>();
        SubscribersText = GM_SubscribersText.GetComponent<TMP_Text>();


        // START [Set player data from saves]
        ulong.TryParse(PlayerPrefs.GetString(Constants.GAME_COUNT), out ulong playerGameCount);
        uint.TryParse(PlayerPrefs.GetString(Constants.SUBS_COUNT), out uint playerSubsCount);
        uint.TryParse(PlayerPrefs.GetString(Constants.PASSIVE_INCOMVE_COUNT), out uint playerPassiveIncomeCount);

        gameCount = playerGameCount > 0 ? playerGameCount : 0;
        subscribersCount = playerSubsCount > 0 ? playerSubsCount : 0;
        passiveIncome = playerPassiveIncomeCount > 0 ? playerPassiveIncomeCount : 0;

        UpdateState?.Invoke(Constants.GAME_COUNT, gameCount.ToString());
        UpdateState?.Invoke(Constants.SUBS_COUNT, subscribersCount.ToString());
        UpdateState?.Invoke(Constants.PASSIVE_INCOMVE_COUNT, passiveIncome.ToString());

        CountText.text = gameCount / 1000 >= 1 ? $"{Mathf.Round(gameCount / 1000)}K" : gameCount.ToString();
        SubscribersText.text = subscribersCount.ToString();
        // END [Set player data from saves]
    }

    void Update()
    {

    }

    public void onGameStateCountChange(ulong count)
    {
        animCountText.SetTrigger("increment");
        CountText.text = count / 1000 >= 1 ? $"{Mathf.Round(count / 1000)}K" : count.ToString();
        SaveState(Constants.GAME_COUNT, count.ToString());
    }

    public void onGameStateIncrementValueChange(uint newIncrementValue)
    {
        Debug.Log(newIncrementValue.ToString());
        SaveState(Constants.INCREMENT_VALUE, newIncrementValue.ToString());
    }

    public void onGameStateSubsChange(uint subsCount)
    {
        SubscribersText.text = subsCount.ToString();
        SaveState(Constants.SUBS_COUNT, subsCount.ToString());
    }

    public void onGameStatePassiveIncomeChange(uint count)
    {
        passiveIncome += count;
        SaveState(Constants.PASSIVE_INCOMVE_COUNT, passiveIncome.ToString());
    }

    private void SaveState(string name, string value)
    {
        PlayerPrefs.SetString(name, value);
        UpdateState?.Invoke(name, value);
    }
}
