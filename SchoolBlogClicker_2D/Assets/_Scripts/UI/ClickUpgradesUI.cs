using System.Collections.Generic;
using UnityEngine;

public class ClickUpgradesUI : MonoBehaviour
{
    [SerializeField] private UpgradeItems_SO upgradeItems;
    [SerializeField] private UpgradeItemPrefab itemPrefab;

    private Stack<UpgradeItemPrefab> items = new Stack<UpgradeItemPrefab>();
    private void OnEnable()
    {
        Debug.Log($"{name} Я ТУТ ");
        foreach (var upgradeItem in upgradeItems.Upgrades)
        {
            UpgradeItemPrefab upgradeItemPrefab = Instantiate(itemPrefab, transform);
            upgradeItemPrefab.Initialize(upgradeItem);
            items.Push(upgradeItemPrefab);
        }
    }

    private void OnDisable()
    {
        while (items.Count > 0) Destroy(items.Pop().gameObject);
    }
}
