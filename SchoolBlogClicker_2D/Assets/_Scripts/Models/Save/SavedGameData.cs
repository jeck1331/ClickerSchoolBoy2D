using System;
using _Scripts.Models.Save;
using JetBrains.Annotations;

[Serializable]
public struct SavedGameData
{
    public bool IsFirstLaunch;
    public long Coins;

    [CanBeNull] public SavedUpgradeItem[] UpgradeClickTree;
    [CanBeNull] public SavedUpgradeItem[] UpgradeIncomeTree;


    public DateTime LastTimeExit;
}