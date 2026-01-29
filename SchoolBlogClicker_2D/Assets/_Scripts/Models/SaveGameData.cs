using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SaveGameData
{
    public string Coins;
    public string ClickPower;
    public string PassiveIncome;

    public string[] UpgradeTreeValue;


    public DateTime LastTimeExit;
}

public class RuntimeSaveGameData
{
    public ulong Coins;
    public uint ClickPower;
    public uint PassiveIncome;

    public Dictionary<string, string> UpgradeTreeValue;

    public RuntimeSaveGameData()
    {
    }

    public RuntimeSaveGameData(SaveGameData gd)
    {
        if (gd == null)
        {
            Coins = 0;
            ClickPower = 1;
            PassiveIncome = 0;
            UpgradeTreeValue = new Dictionary<string, string>();
        }
        else
        {
            Coins = ulong.Parse(String.IsNullOrEmpty(gd.Coins) ? "0" : gd.Coins);
            ClickPower = uint.Parse(String.IsNullOrEmpty(gd.ClickPower) ? "0" : gd.ClickPower);
            PassiveIncome = uint.Parse(String.IsNullOrEmpty(gd.PassiveIncome) ? "0" : gd.PassiveIncome);

            UpgradeTreeValue = gd.UpgradeTreeValue.ToDictionary(kvp => kvp.Split(",")[0], kvp => kvp.Split(",")[1]);
        }
    }


    //public DateTime LastTimeExit;
}