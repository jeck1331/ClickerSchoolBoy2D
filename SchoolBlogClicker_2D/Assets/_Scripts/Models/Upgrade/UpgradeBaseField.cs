using System;
using UnityEngine;

namespace _Scripts.Models.Upgrade
{
    [Serializable]
    public abstract class UpgradeBaseField
    {
        [Header("ID")] [SerializeField] private int id;
        [Header("Title")] [SerializeField] private string title;
        [Header("Base Description")] [SerializeField] private string description;
        [Header("Price")] [SerializeField] private int price;
        [Header("Icon")] [SerializeField] private Sprite sprite;

        public int Id => id;
        public string Title => title;
        public string Description => description;
        public int Price => price;
        public Sprite SpriteAsset => sprite;
    }
}