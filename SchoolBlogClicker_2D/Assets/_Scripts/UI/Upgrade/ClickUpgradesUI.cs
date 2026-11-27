using System.Collections.Generic;
using _Scripts.Prefabs;
using UnityEngine;

public class ClickUpgradesUI : MonoBehaviour
{
    [SerializeField] private UpgradeClickItemSO upgradeClickItem;
    [SerializeField] private UpgradeClickPrefab clickPrefab;
    [SerializeField] private UpgradeManager _upgradeManager;

    private readonly Stack<UpgradeClickPrefab> items = new();

    private void OnEnable()
    {
        _upgradeManager.UpgradesReset += Rebuild;
        Rebuild();
    }

    private void Rebuild()
    {
        Clear();
        foreach (var upgradeItem in upgradeClickItem.Upgrades)
        {
            var state = _upgradeManager.GetStateItem(upgradeItem.Id);
            if (state.IsBought) continue;

            UpgradeClickPrefab upgradeClickPrefab = Instantiate(clickPrefab, transform);
            upgradeClickPrefab.Initialize(
                upgradeItem,
                state,
                _upgradeManager.GetPreviousStateItem(upgradeItem.Id),
                _upgradeManager);
            items.Push(upgradeClickPrefab);
        }
    }

    private void OnDisable()
    {
        _upgradeManager.UpgradesReset -= Rebuild;
        Clear();
    }

    private void Clear()
    {
        while (items.Count > 0)
        {
            var item = items.Pop().gameObject;
            item.SetActive(false);
            Destroy(item);
        }
    }
}
