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
            if (!ulong.TryParse(string.IsNullOrEmpty(gd.Coins) ? "0" : gd.Coins, out var parsedCoins))
                parsedCoins = 0;
            Coins = parsedCoins;

            UpgradeClickTree = ParseUpgradeTree(gd.UpgradeClickTree);
            UpgradeIncomeTree = ParseUpgradeTree(gd.UpgradeIncomeTree);
        }
    }

    private static Dictionary<string, string> ParseUpgradeTree([CanBeNull] IEnumerable<string> source)
    {
        var result = new Dictionary<string, string>();
        if (source == null) return result;

        foreach (var rawEntry in source)
        {
            if (string.IsNullOrWhiteSpace(rawEntry)) continue;
            var parts = rawEntry.Split(',');
            if (parts.Length < 2) continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim();
            if (string.IsNullOrEmpty(key)) continue;

            result[key] = value;
        }

        return result;
    }


    //public DateTime LastTimeExit;
}