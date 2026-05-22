using System;

public static class GameEvents
{
    public static event Action<bool, long> OnClickResolved;
    public static event Action OnUpgradePurchased;
    public static event Action<MiniGameRoundResult, int> OnMiniGameRoundCompleted;

    public static void ClickResolved(bool isCrit, long addedCoins) => OnClickResolved?.Invoke(isCrit, addedCoins);
    public static void UpgradePurchased() => OnUpgradePurchased?.Invoke();
    public static void MiniGameRoundCompleted(MiniGameRoundResult result, int hits) =>
        OnMiniGameRoundCompleted?.Invoke(result, hits);
}
