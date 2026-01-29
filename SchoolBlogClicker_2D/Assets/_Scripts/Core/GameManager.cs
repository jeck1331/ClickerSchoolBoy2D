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
            foreach (var utv in rgd.UpgradeTreeValue)
            {
                if (utv.Value == "1")
                    upgradeItems.Upgrades.First(x => x.Id.ToString() == utv.Key).Buy();
            }
        }
    }

    private SaveGameData CurrentGameData()
    {
        var sgd = new SaveGameData
        {
            Coins = scoreValue?.Value.ToString(),
            ClickPower = clickPowerValue?.Value.ToString(),
            PassiveIncome = passiveIncomeValue?.Value.ToString(),
            UpgradeTreeValue = upgradeItems.Upgrades.Select(x => $"{x.Id},{(x.IsBought == false ? 0 : 1)}").ToArray()
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