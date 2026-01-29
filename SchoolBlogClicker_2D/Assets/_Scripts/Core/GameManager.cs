using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<MonoBehaviour>, IInitialize
{
    private SaveGameSystem _saveGameSystem;

    [SerializeField] private ObserverSo scoreObserver;
    [SerializeField] private ObserverSo powerObserver;
    
    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue clickPowerValue;
    [SerializeField] private UIntValue passiveIncomeValue;
    [SerializeField] private UpgradeItems_SO upgradeItems;

    void IInitialize.Initialize()
    {
        _saveGameSystem = new SaveGameSystem();
        RuntimeSaveGameData rgd = _saveGameSystem.Load();
        scoreValue.Value = rgd.Coins;
        clickPowerValue.Value = rgd.ClickPower;
        passiveIncomeValue.Value = rgd.PassiveIncome;
        if (rgd.UpgradeTreeValue != null)
        {
            foreach (var utv in  rgd.UpgradeTreeValue)
            {
                upgradeItems.Upgrades.First(x => x.Id.ToString() == utv.Key).CurrentLevel = sbyte.Parse(utv.Value);
            }
        }
        else
        {
            upgradeItems.Upgrades = upgradeItems.Upgrades.Select(x =>
            {
                x.CurrentLevel = 1;
                return x;
            }).ToArray();
        }
    }

    private SaveGameData CurrentGameData()
    {
        var sgd = new SaveGameData
        {
            Coins = scoreValue?.Value.ToString(),
            ClickPower = clickPowerValue?.Value.ToString(),
            PassiveIncome = passiveIncomeValue?.Value.ToString(),
            UpgradeTreeValue = upgradeItems.Upgrades.Select(x => $"{x.Id},{x.CurrentLevel}").ToArray()
        };
        return sgd;
    }

    private IEnumerator AutoSave_Coroutine()
    {
        yield return new WaitForSeconds(30);

        _saveGameSystem.Save(CurrentGameData());

        Debug.Log("AutoSave executed");
    }

    private void OnEnable()
    {
        scoreObserver.Changing();
        powerObserver.Changing();
        
        StartCoroutine(AutoSave_Coroutine());
    }

    //public void onGameStateIncrementValueChange(uint newIncrementValue)
    //{
    //    _incrementValue = newIncrementValue;
    //    Debug.Log(_incrementValue.ToString());
    //    SaveState(STATE_FIELD_NAME.INCREMENT_VALUE, _incrementValue.ToString());
    //}

    //public void onGameStateSubsChange(uint subsCount)
    //{
    //    _subscribersCount = subsCount;
    //    _subscribersText.text = _subscribersCount.ToString();
    //    SaveState(STATE_FIELD_NAME.SUBS_COUNT, _subscribersCount.ToString());
    //}

    //public void onGameStatePassiveIncomeChange(uint count)
    //{
    //    _passiveIncome += count;
    //    SaveState(STATE_FIELD_NAME.PASSIVE_INCOMVE_COUNT, _passiveIncome.ToString());
    //}

    //private void onBuyUpgrade(ulong priceValue)
    //{
    //    _gameCount -= priceValue;
    //    _countText.text = _gameCount / 1000 >= 1 ? $"{Mathf.Round(_gameCount / 1000)}K" : _gameCount.ToString();
    //    SaveState(STATE_FIELD_NAME.GAME_COUNT, _gameCount.ToString());
    //}


    private void OnDestroy()
    {
        StopCoroutine(AutoSave_Coroutine());
        _saveGameSystem.Save(CurrentGameData());
    }

    private void OnDisable()
    {
        StopCoroutine(AutoSave_Coroutine());
        _saveGameSystem.Save(CurrentGameData());
    }
}
