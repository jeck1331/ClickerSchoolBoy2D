using System.Collections.Generic;
using _Scripts.Prefabs;
using UnityEngine;

public class ClickUpgradesUI : MonoBehaviour
{
    [SerializeField] private UpgradeClickItemSO upgradeClickItem;
    [SerializeField] private UpgradeClickPrefab clickPrefab;

    private Stack<UpgradeClickPrefab> items = new Stack<UpgradeClickPrefab>();
    private void OnEnable()
    {
        foreach (var upgradeItem in upgradeClickItem.Upgrades)
        {
            UpgradeClickPrefab upgradeClickPrefab = Instantiate(clickPrefab, transform);
            upgradeClickPrefab.Initialize(upgradeItem);
            items.Push(upgradeClickPrefab);
        }
    }

    private void OnDisable()
    {
        while (items.Count > 0) Destroy(items.Pop().gameObject);
    }
}
