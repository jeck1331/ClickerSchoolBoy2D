using System;
using System.Collections.Generic;

namespace _Scripts.Models
{
    [Serializable]
    public class SaveGameData
    {
        public string Coins;
        public string ClickPower;
        public string PassiveIncome;

        public Dictionary<string, int> UpgradeTreeValue;


        public DateTime LastTimeExit;
    }
    
    public class RuntimeSaveGameData
    {
        public ulong Coins;
        public uint ClickPower;
        public uint PassiveIncome;

        public Dictionary<string, int> UpgradeTreeValue;

        public RuntimeSaveGameData() { }

        public RuntimeSaveGameData(SaveGameData? gd)
        {
            if (gd == null)
            {
                Coins = 0;
                ClickPower = 1;
                PassiveIncome = 0;
            } else
            {
                Coins = ulong.Parse(gd.Coins);
                ClickPower = uint.Parse(gd.ClickPower);
                PassiveIncome = uint.Parse(gd.PassiveIncome);

                UpgradeTreeValue = gd.UpgradeTreeValue;
            }
        }


        //public DateTime LastTimeExit;
    }
}
