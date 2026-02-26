using System.Collections.Generic;
using _Scripts.Prefabs;
using UnityEngine;

public class IncomeUpgradesUI : MonoBehaviour
{
    [SerializeField] private UpgradeIncomeItemSO upgradeIncomeItem;
    [SerializeField] private UpgradeIncomePrefab incomePrefab;

    private Stack<UpgradeIncomePrefab> items = new Stack<UpgradeIncomePrefab>();
    private void OnEnable()
    {
        foreach (var upgradeItem in upgradeIncomeItem.Upgrades)
        {
            UpgradeIncomePrefab upgradeIncomePrefab = Instantiate(incomePrefab, transform);
            upgradeIncomePrefab.Initialize(upgradeItem);
            items.Push(upgradeIncomePrefab);
        }
    }

    private void OnDisable()
    {
        while (items.Count > 0) Destroy(items.Pop().gameObject);
    }
}
