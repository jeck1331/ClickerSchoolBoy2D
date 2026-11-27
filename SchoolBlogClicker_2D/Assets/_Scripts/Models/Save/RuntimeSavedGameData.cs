using System.Collections.Generic;
using System.Linq;

namespace _Scripts.Models.Save
{
    public class RuntimeSavedGameData
    {
        public long Coins;
        public int CoinRemainder;

        public List<SavedUpgradeItem> UpgradeClickTree;
        public List<SavedUpgradeItem> UpgradeIncomeTree;

        public RuntimeSavedGameData()
        {
            Coins = 0;
            UpgradeClickTree = new();
            UpgradeIncomeTree = new();
        }

        public RuntimeSavedGameData(SavedGameData? gd)
        {
            if (gd == null)
            {
                Coins = 0;
                UpgradeClickTree = new();
                UpgradeIncomeTree = new();
            }
            else
            {
                Coins = gd.Value.Coins;
                const int legacyScale = 10;
                int scale = gd.Value.CoinRemainderScale > 0 ? gd.Value.CoinRemainderScale : legacyScale;
                long remainder = System.Math.Clamp(gd.Value.CoinRemainder, 0, scale - 1);
                CoinRemainder = (int)(remainder * UIntValue.UnitsPerCoin / scale);

                if (gd.Value.UpgradeClickTree != null) UpgradeClickTree = gd.Value.UpgradeClickTree.ToList();
                if (gd.Value.UpgradeIncomeTree != null) UpgradeIncomeTree = gd.Value.UpgradeIncomeTree.ToList();
            }
        }

        // private static List<SavedUpgradeItem> ParseUpgradeTree([CanBeNull] IEnumerable<SavedUpgradeItem> source)
        // {
        //     var result = new List<SavedUpgradeItem>();
        //     if (source == null) return result;
        //
        //     foreach (var rawEntry in source)
        //     {
        //         result.Add(rawEntry);
        //     }
        //
        //     return result;
        // }


        //public DateTime LastTimeExit;
    }
}