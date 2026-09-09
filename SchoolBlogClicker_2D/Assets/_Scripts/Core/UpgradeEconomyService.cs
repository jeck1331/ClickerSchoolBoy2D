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
        UpgradeManager upgradeManager)
    {
        if (!CanBuy(data, state, scoreValue.Value)) return false;

        scoreValue.Value -= data.Price;
        state.Buy();
        powerValue.Value = upgradeManager.GetPowerValue();
        GameEvents.UpgradePurchased();
        return true;
    }

    public static bool TryBuyIncomeUpgrade(
        UpgradeIncomeField data,
        UpgradeStateItem state,
        ULongValue scoreValue,
        UIntValue incomeValue,
        ShopManager shopManager)
    {
        if (!CanBuy(data, state, scoreValue.Value)) return false;

        scoreValue.Value -= data.Price;
        state.Buy();
        incomeValue.Value = shopManager.GetIncomeValue();
        GameEvents.UpgradePurchased();
        return true;
    }
    
    public static void TryUnlockByPreviousId(UpgradeStateItem state, UpgradeStateItem previousState)
    {
        if (!state.IsSecret) return;
        if (previousState == null || previousState.IsBought)
            state.Unsecret();
    }
}
