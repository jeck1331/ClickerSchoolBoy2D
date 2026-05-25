using System.Linq;
using _Scripts.Core;
using _Scripts.Models.Upgrade;
using Unity.VisualScripting;

public static class UpgradeEconomyService
{
    public static bool CanBuy(UpgradeBaseField data, UpgradeStateItem state, long score)
    {
        return data != null && !state.IsBought && !state.IsSecret && score >= data.Price;
    }
    
    public static bool TryBuyClickUpgrade(
        UpgradeClickField data,
        UpgradeStateItem state,
        ULongValue scoreValue,
        UIntValue powerValue,
        UpgradeClickItemSO upgrades)
    {
        if (!CanBuy(data, state, scoreValue.Value)) return false;
    
        scoreValue.Value -= data.Price;
        state.Buy();
        powerValue.Value = CalcCacheHelper.CalcPowerCache(upgrades.Upgrades.Where(x => x.Id == state.Id && state.IsBought).ToArray());
        GameEvents.UpgradePurchased();
        return true;
    }
    
    public static bool TryBuyIncomeUpgrade(
        UpgradeIncomeField data,
        UpgradeStateItem state,
        ULongValue scoreValue,
        UIntValue incomeValue,
        UpgradeIncomeItemSO upgrades)
    {
        if (!CanBuy(data, state, scoreValue.Value)) return false;
    
        scoreValue.Value -= data.Price;
        state.Buy();
        incomeValue.Value = CalcCacheHelper.CalcIncomeCache(upgrades.Upgrades.Where(x =>  x.Id == state.Id && state.IsBought).ToArray());
        GameEvents.UpgradePurchased();
        return true;
    }
    
    public static void TryUnlockByPreviousId(UpgradeBaseField data, UpgradeStateItem state, UpgradeStateItem[] states)
    {
        if (data == null || states == null || !state.IsSecret) return;
        var prev = states.SingleOrDefault(u => u.Id == state.Id - 1);
        if (prev != null && prev.IsBought)
            state.Unsecret();
    }
}
