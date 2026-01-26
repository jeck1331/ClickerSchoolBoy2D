using _Scripts.Core;
using _Scripts.Utilities;
using _Scripts.Interfaces;
using _Scripts.Models;
using System.Collections;
using _Scripts.Events;
using _Scripts.ScriptableObjects;
using UnityEngine;

public class GameManager : Singleton<MonoBehaviour>, IInitialize
{
    private SaveGameSystem _saveGameSystem;
    [SerializeField] private ulong coins;
    [SerializeField] private uint clickPower;
    [SerializeField] private uint passiveIncome;
    public ulong Coins => coins;
    public uint ClickPower => clickPower;
    public uint PassiveIncome => passiveIncome;

    [SerializeField] private ULongObserver scoreObserver;
    [SerializeField] private UIntObserver clickPowerObserver;
    [SerializeField] private UIntObserver passiveIncomeObserver;

    void IInitialize.Initialize()
    {
        _saveGameSystem = new SaveGameSystem();
        RuntimeSaveGameData rgd = _saveGameSystem.Load();
        coins = rgd.Coins;
        clickPower = rgd.ClickPower;
        passiveIncome = rgd.PassiveIncome;

        GameEvents.InvokeGameInitialized();
    }

    private SaveGameData CurrentGameData()
    {
        return new SaveGameData
        {
            Coins = coins.ToString(),
            ClickPower = clickPower.ToString(),
            PassiveIncome = passiveIncome.ToString()
        };
    }

    private IEnumerator AutoSave_Coroutine()
    {
        yield return new WaitForSeconds(30);

        _saveGameSystem.Save(CurrentGameData());

        Debug.Log("AutoSave executed");
    }

    private void OnEnable()
    {
        CoinsEvents.InvokeCoinsChanged(coins);
        ClickerEvents.OnClickOrTap += onGameStateCountChange;
        StartCoroutine(AutoSave_Coroutine());
    }

    public void onGameStateCountChange()
    {
        coins += clickPower;
        CoinsEvents.InvokeCoinsChanged(coins, true);
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
