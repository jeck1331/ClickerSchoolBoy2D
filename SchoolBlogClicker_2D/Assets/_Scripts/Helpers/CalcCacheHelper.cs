using System.Linq;
using _Scripts.Models.Enums;
using _Scripts.Models.Upgrade;

namespace _Scripts.Core
{
    public static class CalcCacheHelper
    {
        public const int StartingPower = UIntValue.UnitsPerCoin;

        public static int CalcPowerCache(UpgradeClickField[] powerUpgrades)
        {
            int power = StartingPower;
            foreach (var upgrade in powerUpgrades.OrderBy(x => x.Id))
            {
                switch (upgrade.CalcType)
                {
                    case ClickCalcType.Add:
                        power += upgrade.Power;
                        break;
                    case ClickCalcType.Multiply:
                        power *= upgrade.Power;
                        break;
                    case ClickCalcType.SetBonus:
                        power = StartingPower + upgrade.Power;
                        break;
                }
            }

            return power;
        }
    }
}
