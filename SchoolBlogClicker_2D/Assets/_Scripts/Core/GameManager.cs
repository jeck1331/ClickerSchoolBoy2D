using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<MonoBehaviour>, IInitialize
{
    private SaveGameSystem _saveGameSystem;

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
        powerValue.Value = rgd.Power;
        incomeValue.Value = rgd.Income;
        if (rgd.UpgradeClickTree != null)
        {
            foreach (var utv in rgd.UpgradeClickTree)
            {
                if (utv.Value == "1")
                    upgradeClickItem.Upgrades.First(x => x.Id.ToString() == utv.Key).Buy();
            }
        }
    }

    private SaveGameData CurrentGameData()
    {
        var sgd = new SaveGameData
        {
            Coins = scoreValue?.Value.ToString(),
            Power = powerValue?.Value.ToString(),
            Income = incomeValue?.Value.ToString(),
            UpgradeClickTree = upgradeClickItem.Upgrades.Select(x => $"{x.Id},{(x.IsBought == false ? 0 : 1)}").ToArray(),
            UpgradeIncomeTree = upgradeIncomeItem.Upgrades.Select(x => $"{x.Id},{(x.IsBought == false ? 0 : 1)}").ToArray()
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