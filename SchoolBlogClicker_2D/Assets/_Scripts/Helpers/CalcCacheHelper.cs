using System.Linq;
using _Scripts.Models.Enums;
using _Scripts.Models.Upgrade;

namespace _Scripts.Core
{
    public static class CalcCacheHelper
    {
        public static int CalcIncomeCache (UpgradeIncomeField[] incomeUpgrades)
        {
            int res = 0;
            foreach (var upgrade in incomeUpgrades)
            {
                res += upgrade.Income;
            }

            // Debug.Log($"{res} РАССЧИТАН INCOME CACHE");
            return res;
        }
        
        public static int CalcPowerCache (UpgradeClickField[] powerUpgrades)
        {
            int res = 1;
            foreach (var upgrade in powerUpgrades.OrderBy(x => x.Id))
            {
                switch (upgrade.CalcType)
                {
                    case ClickCalcType.Add:
                        res += upgrade.Power;
                        break;
                    case ClickCalcType.Multiply:
                        res *= upgrade.Power;
                        break;
                }
            }
            // Debug.Log($"{res} РАССЧИТАН POWER CACHE");
            return res;
        }
    }
}