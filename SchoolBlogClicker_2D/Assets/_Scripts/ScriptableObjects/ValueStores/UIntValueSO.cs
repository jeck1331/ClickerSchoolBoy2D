using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ValueStore/UInt Value")]
public class UIntValue : ValueStoreSO<int>
{
    public const int UnitsPerCoin = 100;
    public decimal CoinValue => ToCoins(Value);

    public static decimal ToCoins(int units) => (decimal)units / UnitsPerCoin;
}
