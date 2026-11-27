using _Scripts.Models.Upgrade;

public static class UpgradeEconomyService
{
    public static bool CanBuyClickUpgrade(UpgradeClickField data, UpgradeStateItem state, long score)
    {
        return !state.IsBought && !state.IsSecret && score >= data.Price;
    }

    public static bool CanBuyIncomeUpgrade(UpgradeIncomeField data, UpgradeStateItem state, long score)
    {
        var price = data.GetPrice(state.Count);
        return !state.IsSecret && price.HasValue && score >= price.Value;
    }

    public static bool TryBuyClickUpgrade(
        UpgradeClickField data,
        UpgradeStateItem state,
        ULongValue scoreValue,
        UIntValue powerValue,
        UpgradeManager upgradeManager)
    {
        if (!CanBuyClickUpgrade(data, state, scoreValue.Value)) return false;

        state.Buy();
        scoreValue.Value -= data.Price;
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
        var price = data.GetPrice(state.Count);
        if (state.IsSecret || !price.HasValue || scoreValue.Value < price.Value) return false;

        state.Buy();
        scoreValue.Value -= price.Value;
        incomeValue.Value = shopManager.GetIncomeValue();
        GameEvents.UpgradePurchased();
        return true;
    }

    public static void TryUnlockByPreviousId(UpgradeStateItem state, UpgradeStateItem previousState)
    {
        if (state.IsSecret && (previousState == null || previousState.IsBought))
            state.Unsecret();
    }
}
