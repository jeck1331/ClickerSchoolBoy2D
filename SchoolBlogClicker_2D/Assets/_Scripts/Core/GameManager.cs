using System.Collections;
using _Scripts.Models.Save;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IInitialize
{
    private SaveGameSystem _saveGameSystem;
    private int coinRemainder;
    private int incomeRate;
    private double lastIncomeTime;
    private double pendingIncomeUnits;
    [CanBeNull] private Coroutine _autoSaveCoroutine;
    [CanBeNull] private Coroutine _incomeCoroutine;

    [SerializeField] private ObserverSO scoreObserver;
    [SerializeField] private ObserverSO powerObserver;
    [SerializeField] private ObserverSO incomeObserver;

    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue powerValue;
    [SerializeField] private UIntValue incomeValue;
    
    [SerializeField] private UpgradeClickItemSO upgradeClickItem;
    [SerializeField] private UpgradeIncomeItemSO upgradeIncomeItem;
    
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private UpgradeManager upgradeManager;

    void IInitialize.Initialize()
    {
        _saveGameSystem = new SaveGameSystem();
        RuntimeSavedGameData rgd = _saveGameSystem.Load();
        
        shopManager.FillDataFromSave(rgd.UpgradeIncomeTree);
        upgradeManager.FillDataFromSave(rgd.UpgradeClickTree);
        
        coinRemainder = System.Math.Clamp(rgd.CoinRemainder, 0, UIntValue.UnitsPerCoin - 1);
        scoreValue.Value = rgd.Coins;
        powerValue.Value = upgradeManager.GetPowerValue();
        incomeValue.Value = shopManager.GetIncomeValue();
    }

    private SavedGameData CurrentGameData() => new SavedGameData
    {
        Coins = scoreValue.Value,
        CoinRemainder = coinRemainder,
        CoinRemainderScale = UIntValue.UnitsPerCoin,
        UpgradeClickTree = upgradeManager.GetDataToSave(),
        UpgradeIncomeTree = shopManager.GetDataToSave()
    };

    private IEnumerator AutoSave_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(30);

            _saveGameSystem.Save(CurrentGameData());

            Debug.Log("AutoSave executed");
        }
    }
    
    private IEnumerator Income_Coroutine()
    {
        var interval = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return interval;
            CreditIncomeUntil(Time.timeAsDouble);
        }
    }

    private void OnEnable()
    {
        incomeObserver.OnValueChanged += IncomeUpdate;
        
        Debug.Log($"Start income {scoreValue.Value}");
        scoreObserver.Changing();
        powerObserver.Changing();

        IncomeUpdate();

        _autoSaveCoroutine = StartCoroutine(AutoSave_Coroutine());
    }

    private void IncomeUpdate()
    {
        CreditIncomeUntil(Time.timeAsDouble);
        incomeRate = incomeValue.Value;
        if (incomeRate > 0 && _incomeCoroutine == null)
            _incomeCoroutine = StartCoroutine(Income_Coroutine());
        else if (incomeRate == 0 && _incomeCoroutine != null)
        {
            StopCoroutine(_incomeCoroutine);
            _incomeCoroutine = null;
        }
    }

    private void CreditIncomeUntil(double now)
    {
        if (incomeRate > 0)
            pendingIncomeUnits += incomeRate * (now - lastIncomeTime);

        lastIncomeTime = now;
        long earnedUnits = (long)pendingIncomeUnits;
        if (earnedUnits == 0) return;

        pendingIncomeUnits -= earnedUnits;
        EarnCoinUnits(earnedUnits);
    }
    
    private void OnDisable()
    {
        incomeObserver.OnValueChanged -= IncomeUpdate;
        
        if (_autoSaveCoroutine != null)
            StopCoroutine(_autoSaveCoroutine);
        if (_incomeCoroutine != null)
        {
            StopCoroutine(_incomeCoroutine);
            _incomeCoroutine = null;
        }

        CreditIncomeUntil(Time.timeAsDouble);
        incomeRate = 0;
        
        _saveGameSystem.Save(CurrentGameData());
    }

    public long EarnCoinUnits(long units)
    {
        int remainder = coinRemainder + (int)(units % UIntValue.UnitsPerCoin);
        long coins = units / UIntValue.UnitsPerCoin + remainder / UIntValue.UnitsPerCoin;
        coinRemainder = remainder % UIntValue.UnitsPerCoin;
        long credited = System.Math.Min(coins, long.MaxValue - System.Math.Max(0, scoreValue.Value));
        scoreValue.Value += credited;
        return credited;
    }

    public void ResetData()
    {
        incomeRate = 0;
        pendingIncomeUnits = 0;
        coinRemainder = 0;
        scoreValue.Value = 0;
        shopManager.Reset();
        upgradeManager.Reset();
        powerValue.Value = upgradeManager.GetPowerValue();
        incomeValue.Value = shopManager.GetIncomeValue();
        
        _saveGameSystem.Save(CurrentGameData());
    }

    public void AddCoinsAndSave(long amount)
    {
        if (amount < 0) throw new System.ArgumentOutOfRangeException(nameof(amount));
        SavedGameData data = CurrentGameData();
        data.Coins += System.Math.Min(amount, long.MaxValue - System.Math.Max(0L, data.Coins));
        _saveGameSystem.Save(data);
        scoreValue.Value = data.Coins;
    }
}
