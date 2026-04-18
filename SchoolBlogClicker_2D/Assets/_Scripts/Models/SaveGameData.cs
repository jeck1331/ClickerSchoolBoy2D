using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

[Serializable]
public class SaveGameData
{
    public bool IsFirstLaunch = true;
    public string Coins;

    [CanBeNull] public string[] UpgradeClickTree;
    [CanBeNull] public string[] UpgradeIncomeTree;


    public DateTime LastTimeExit;
}

public class RuntimeSaveGameData
{
    public ulong Coins;

    public Dictionary<string, string> UpgradeClickTree;
    public Dictionary<string, string> UpgradeIncomeTree;

    public RuntimeSaveGameData()
    {
    }

    public RuntimeSaveGameData(SaveGameData gd)
    {
        if (gd == null)
        {
            Coins = 0;
            UpgradeClickTree = new Dictionary<string, string>();
            UpgradeIncomeTree = new Dictionary<string, string>();
        }
        else
        {
            Coins = ulong.Parse(String.IsNullOrEmpty(gd.Coins) ? "0" : gd.Coins);

            UpgradeClickTree = gd.UpgradeClickTree != null ?
                gd.UpgradeClickTree.ToDictionary(kvp => kvp.Split(",")[0], kvp => kvp.Split(",")[1]) :
                new();
            UpgradeIncomeTree = gd.UpgradeIncomeTree != null ? 
                gd.UpgradeIncomeTree.ToDictionary(kvp => kvp.Split(",")[0], kvp => kvp.Split(",")[1]) :
                new();
        }
    }


    //public DateTime LastTimeExit;
}