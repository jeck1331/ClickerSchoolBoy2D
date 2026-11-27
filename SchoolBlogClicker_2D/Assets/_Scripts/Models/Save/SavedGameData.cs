using System;
using _Scripts.Models.Save;
using JetBrains.Annotations;

[Serializable]
public struct SavedGameData
{
    public bool IsFirstLaunch;
    public long Coins;
    public int CoinRemainder;
    public int CoinRemainderScale;

    [CanBeNull] public SavedUpgradeItem[] UpgradeClickTree;
    [CanBeNull] public SavedUpgradeItem[] UpgradeIncomeTree;


    public DateTime LastTimeExit;
}