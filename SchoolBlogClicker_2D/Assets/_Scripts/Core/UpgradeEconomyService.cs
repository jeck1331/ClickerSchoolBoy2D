using System.Linq;
using _Scripts.Core;
using _Scripts.Models.Upgrade;

public static class UpgradeEconomyService
{
    public static bool CanBuy(UpgradeBaseField data, ulong score)
    {
        return data != null && !data.IsBought && !data.IsSecret && score >= data.Price;
    }

    public static bool TryBuyClickUpgrade(
        UpgradeClickField data,
        ULongValue scoreValue,
        UIntValue powerValue,
        UpgradeClickItemSO upgrades)
    {
        if (!CanBuy(data, scoreValue.Value)) return false;

        scoreValue.Value -= data.Price;
        data.Buy();
        powerValue.Value = CalcCacheHelper.CalcPowerCache(upgrades.Upgrades.Where(x => x.IsBought).ToArray());
        GameEvents.UpgradePurchased();
        return true;
    }

    public static bool TryBuyIncomeUpgrade(
        UpgradeIncomeField data,
        ULongValue scoreValue,
        UIntValue incomeValue,
        UpgradeIncomeItemSO upgrades)
    {
        if (!CanBuy(data, scoreValue.Value)) return false;

        scoreValue.Value -= data.Price;
        data.Buy();
        incomeValue.Value = CalcCacheHelper.CalcIncomeCache(upgrades.Upgrades.Where(x => x.IsBought).ToArray());
        GameEvents.UpgradePurchased();
        return true;
    }

    public static void TryUnlockByPreviousId(UpgradeBaseField data, UpgradeBaseField[] upgrades)
    {
        if (data == null || upgrades == null || !data.IsSecret) return;
        var prev = upgrades.SingleOrDefault(u => u.Id == data.Id - 1);
        if (prev != null && prev.IsBought)
            data.Unsecret();
    }
}
