using System.Collections.Generic;
using System.Linq;
using _Scripts.Models.Save;
using _Scripts.Models.Upgrade;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private Dictionary<int, UpgradeStateItem> upgradeStates = new Dictionary<int, UpgradeStateItem>();
    [SerializeField] private UpgradeIncomeItemSO _shopData;
    [SerializeField] private Scrollbar _scrollbar;

    float velocity = 0f;
    float smoothTime = 0.1f;
    private bool isScrollToTop = false;

    private void OnEnable()
    {
        isScrollToTop = true;
    }
    
    private void Update()
    {
        if (!isScrollToTop) return;
        
        _scrollbar.value = Mathf.SmoothDamp(_scrollbar.value, 1f, ref velocity, smoothTime);

        if (Mathf.Abs(_scrollbar.value) > 0.999f)
        {
            _scrollbar.value = 1f;
            isScrollToTop = false;
        }
    }

    private void OnDisable()
    {
        isScrollToTop = false;
    }

    public void FillDataFromSave(List<SavedUpgradeItem> data)
    {
        var savedById = data?.ToDictionary(x => x.Id) ?? new Dictionary<int, SavedUpgradeItem>();
        var previousBought = true;

        foreach (var item in _shopData.Upgrades.OrderBy(x => x.Id))
        {
            int owned = savedById.TryGetValue(item.Id, out var saved)
                ? System.Math.Max(saved.Count, saved.IsBought ? 1 : 0)
                : 0;
            var state = new UpgradeStateItem(item.Id, owned, !previousBought && owned == 0);
            upgradeStates.TryAdd(item.Id, state);
            previousBought = state.IsBought;
        }
    }
    
    public UpgradeStateItem GetStateItem(int id) => upgradeStates[id];

    public UpgradeStateItem GetPreviousStateItem(int id) => upgradeStates.TryGetValue(id - 1, out var item) ? item : null;

    public SavedUpgradeItem[] GetDataToSave() => upgradeStates.Select(x => new SavedUpgradeItem
    {
        Id = x.Key,
        IsBought = x.Value.IsBought,
        Count = x.Value.Count
    }).ToArray();

    public int GetIncomeValue()
    {
        int total = 0;
        foreach (var upgrade in _shopData.Upgrades)
        {
            if (upgradeStates.TryGetValue(upgrade.Id, out var state))
                total += upgrade.Income * state.Count;
        }

        return total;
    }
    
    public void Reset()
    {
        foreach (var upgradeStateItem in upgradeStates)
        {
            upgradeStateItem.Value.Reset();
        }
    }
}
