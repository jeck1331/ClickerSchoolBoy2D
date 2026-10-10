using _Scripts.Models.Upgrade;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/Click Item Upgrades")]
public class UpgradeClickItemSO: ScriptableObject
{
    [Header("Upgrades")][SerializeField] public UpgradeClickField[] Upgrades;
}