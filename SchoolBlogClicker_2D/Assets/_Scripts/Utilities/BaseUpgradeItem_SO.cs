// using UnityEngine;
//
// [CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/Click Item Upgrade")]
// public class BaseUpgradeItem_SO : ScriptableObject
// {
//     [Header("Name")] [SerializeField] private string title;
//
//     [Header("Description")] [SerializeField]
//     private string description;
//
//     [Header("Icon")] [SerializeField] private Sprite sprite;
//
//     [Header("Max level")] [Range(1, 20)] [SerializeField]
//     private sbyte maxLevel;
//     
//     [Header("Current level")] [SerializeField]
//     private sbyte currentLevel = 1;
//
//     [Header("Upgrade Tree")][SerializeField] private uint[] upgradeTree;
//
//     public string Title => title;
//     public string Description => description;
//     public uint Price => upgradeTree[currentLevel-1];
//     public Sprite SpriteAsset => sprite;
//     public sbyte MaxLevel => maxLevel;
//     public sbyte CurrentLevel => currentLevel;
// }