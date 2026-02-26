using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SaveGameData
{
    public bool IsFirstLaunch = true;
    public string Coins;
    public string Power;
    public string Income;

    public string[] UpgradeClickTree;
    public string[] UpgradeIncomeTree;


    public DateTime LastTimeExit;
}

public class RuntimeSaveGameData
{
    public ulong Coins;
    public uint Power;
    public uint Income;

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
            Power = 1;
            Income = 0;
            UpgradeClickTree = new Dictionary<string, string>();
            UpgradeIncomeTree = new Dictionary<string, string>();
        }
        else
        {
            Coins = ulong.Parse(String.IsNullOrEmpty(gd.Coins) ? "0" : gd.Coins);
            Power = uint.Parse(String.IsNullOrEmpty(gd.Power) ? "0" : gd.Power);
            Income = uint.Parse(String.IsNullOrEmpty(gd.Income) ? "0" : gd.Income);

            UpgradeClickTree = gd.UpgradeClickTree.ToDictionary(kvp => kvp.Split(",")[0], kvp => kvp.Split(",")[1]);
            UpgradeIncomeTree = gd.UpgradeIncomeTree.ToDictionary(kvp => kvp.Split(",")[0], kvp => kvp.Split(",")[1]);
        }
    }


    //public DateTime LastTimeExit;
}