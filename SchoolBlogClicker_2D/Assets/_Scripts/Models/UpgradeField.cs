using System;
using UnityEngine;

[Serializable]
public class UpgradeField
{
    [Header("ID")] [SerializeField] private uint id;
    [Header("Title")] [SerializeField] private string title;

    [Header("Base Description")] [SerializeField]
    private string description;

    [Header("Power")] [SerializeField] private uint power;
    [Header("Price")] [SerializeField] private uint price;

    [Header("Icon")] [SerializeField] private Sprite sprite;

    [Header("Bought?")] [SerializeField] private bool isBought = false;
    [Header("Secret?")] [SerializeField] private bool isSecret = true; 

    public uint Id => id;
    public string Title => title;
    public string Description => description;
    public uint Power => power;

    public uint Price => price;

    public Sprite SpriteAsset => sprite;

    public bool IsSecret => isSecret;

    public bool IsBought => isBought;

    public void Buy() => isBought = true;
    public void Unsecret() => isSecret = false;
}