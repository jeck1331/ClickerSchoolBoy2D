using System;
using UnityEngine;

[Serializable]
public class UpgradeField
{
    [Header("ID")][SerializeField]private uint id;
    [Header("Base Description")] [SerializeField]
    private string description;

    [Header("Icon")] [SerializeField] private Sprite sprite;

    [Header("Max level")] [Range(1, 20)] [SerializeField]
    private sbyte maxLevel;
    
    [Header("Current level")] [SerializeField]
    private sbyte currentLevel = 1;

    [Header("Price Tree")][SerializeField] private uint[] priceTree;
    [Header("Upgrade Tree")][SerializeField] private uint[] upgradeTree;
    [Header("Titles Tree")][SerializeField] private string[] titlesTree;

    public uint Id => id;
    public string Title => titlesTree[CurrentLevel-1];
    public string Description => description;
    public uint Power => upgradeTree[CurrentLevel - 1];
    public string PriceTitle => priceTree[CurrentLevel-1]/1000 >= 1 ? $"{(priceTree[CurrentLevel-1] / 1000f).ToString("0.0")}K" : priceTree[CurrentLevel-1].ToString();
    public uint Price => priceTree[CurrentLevel-1];
    public Sprite SpriteAsset => sprite;
    public sbyte MaxLevel => maxLevel;
    public sbyte CurrentLevel
    {
        get => currentLevel;
        set
        {
            if (value >= maxLevel) currentLevel = maxLevel;
            
            currentLevel = value;
        }
    }
}