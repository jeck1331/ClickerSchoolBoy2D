using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SaveGameData
{
    public bool IsFirstLaunch = true;
    public string Coins;

    public string[] UpgradeClickTree;
    public string[] UpgradeIncomeTree;


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

            UpgradeClickTree = gd.UpgradeClickTree.ToDictionary(kvp => kvp.Split(",")[0], kvp => kvp.Split(",")[1]);
            UpgradeIncomeTree = gd.UpgradeIncomeTree.ToDictionary(kvp => kvp.Split(",")[0], kvp => kvp.Split(",")[1]);
        }
    }


    //public DateTime LastTimeExit;
}