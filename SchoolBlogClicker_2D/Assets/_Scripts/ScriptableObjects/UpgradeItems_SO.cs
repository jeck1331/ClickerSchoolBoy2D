using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/Click Item Upgrades")]
public class UpgradeItems_SO: ScriptableObject
{
    [Header("Upgrades")][SerializeField] public UpgradeField[] Upgrades;
}