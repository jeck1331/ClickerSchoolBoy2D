using System.Collections;
using System.Linq;
using _Scripts.Core;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IInitialize
{
    private SaveGameSystem _saveGameSystem;
    private bool _isIncomeStared = false;
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

    void IInitialize.Initialize()
    {
        _saveGameSystem = new SaveGameSystem();
        RuntimeSaveGameData rgd = _saveGameSystem.Load();
        scoreValue.Value = rgd.Coins;
        if (rgd.UpgradeClickTree != null)
        {
            foreach (var utv in rgd.UpgradeClickTree)
            {
                var v = upgradeClickItem.Upgrades.First(x => x.Id.ToString() == utv.Key);
                if (utv.Value == "1") v.Buy();
                else v.Reset();
            }

            powerValue.Value = CalcCacheHelper.CalcPowerCache(upgradeClickItem.Upgrades.Where(x => x.IsBought).ToArray());
        }
        if (rgd.UpgradeIncomeTree != null)
        {
            foreach (var utv in rgd.UpgradeIncomeTree)
            {
                var v = upgradeIncomeItem.Upgrades.First(x => x.Id.ToString() == utv.Key);
                if (utv.Value == "1") v.Buy();
                else v.Reset();
                
            }
            incomeValue.Value = CalcCacheHelper.CalcIncomeCache(upgradeIncomeItem.Upgrades.Where(x => x.IsBought).ToArray());
        }
    }

    private SaveGameData CurrentGameData()
    {
        var sgd = new SaveGameData
        {
            Coins = scoreValue?.Value.ToString(),
            UpgradeClickTree = upgradeClickItem.Upgrades.Select(x => $"{x.Id},{(!x.IsBought ? 0 : 1)}").ToArray(),
            UpgradeIncomeTree = upgradeIncomeItem.Upgrades.Select(x => $"{x.Id},{(!x.IsBought ? 0 : 1)}").ToArray()
        };
        return sgd;
    }

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
        while (true)
        {
            yield return new WaitForSeconds(1);
        
            scoreValue.Value += incomeValue.Value;
        
            // Debug.Log("INCOME executed");
        }
    }

    private void OnEnable()
    {
        incomeObserver.OnValueChanged += IncomeUpdate;
        
        scoreObserver.Changing();
        powerObserver.Changing();

        IncomeUpdate();

        _autoSaveCoroutine = StartCoroutine(AutoSave_Coroutine());
    }

    private void IncomeUpdate()
    {
        if (!_isIncomeStared && incomeValue.Value > 0)
        {
            // Debug.Log("Income update START COROUTINE");
            _isIncomeStared = true;
            _incomeCoroutine = StartCoroutine(Income_Coroutine());
        }
    }

    // private void OnDestroy()
    // {
    //     StopCoroutine(AutoSave_Coroutine());
    //     _saveGameSystem.Save(CurrentGameData());
    // }

    private void OnDisable()
    {
        incomeObserver.OnValueChanged -= IncomeUpdate;
        
        if (_autoSaveCoroutine != null)
            StopCoroutine(_autoSaveCoroutine);
        if (_incomeCoroutine != null)
            StopCoroutine(_incomeCoroutine);
        // Debug.Log("Coroutines stopped");
        
        _saveGameSystem.Save(CurrentGameData());
    }
}