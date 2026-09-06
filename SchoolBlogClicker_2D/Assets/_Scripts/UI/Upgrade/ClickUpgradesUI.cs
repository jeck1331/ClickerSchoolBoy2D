using System.Collections.Generic;
using _Scripts.Models.Upgrade;
using _Scripts.Prefabs;
using UnityEngine;

public class ClickUpgradesUI : MonoBehaviour
{
    [SerializeField] private UpgradeClickItemSO upgradeClickItem;
    [SerializeField] private UpgradeClickPrefab clickPrefab;
    [SerializeField] private UpgradeManager _upgradeManager;

    private Stack<UpgradeClickPrefab> items = new Stack<UpgradeClickPrefab>();
    private void OnEnable()
    {
        foreach (var upgradeItem in upgradeClickItem.Upgrades)
        {
            UpgradeClickPrefab upgradeClickPrefab = Instantiate(clickPrefab, transform);
            upgradeClickPrefab.Initialize(upgradeItem, _upgradeManager.GetStateItem(upgradeItem.Id));
            items.Push(upgradeClickPrefab);
            Debug.Log($"Instantiate: {upgradeClickPrefab.name}");
        }
    }

    private void OnDisable()
    {
        while (items.Count != 0) Destroy(items.Pop().gameObject);
    }
}
