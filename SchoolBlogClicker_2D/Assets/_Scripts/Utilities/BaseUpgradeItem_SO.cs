using UnityEngine;

namespace _Scripts.Utilities
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/Click Item Upgrade")]
    public class BaseUpgradeItem_SO : ScriptableObject
    {
        [Header("Name")] [SerializeField] private string title;
        [Header("Description")] [SerializeField] private string description;
        [Header("Price")] [SerializeField] private uint price;
        [Header("Icon")] [SerializeField] private Sprite sprite;
        [Header("Max level")] [Range(1, 20)] [SerializeField] private sbyte maxLevel;
        [Header("Current level")] [SerializeField] private sbyte currentLevel = 1;

        public string Title => title;
        public string Description => description;
        public uint Price => price;
        public Sprite SpriteAsset => sprite;
        public sbyte MaxLevel => maxLevel;
        public sbyte CurrentLevel => currentLevel;
    }
}