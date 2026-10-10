using _Scripts.Models.Upgrade;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/Income Item Upgrades")]
public class UpgradeIncomeItemSO: ScriptableObject
{
    [Header("Upgrades")][SerializeField] public UpgradeIncomeField[] Upgrades;
}