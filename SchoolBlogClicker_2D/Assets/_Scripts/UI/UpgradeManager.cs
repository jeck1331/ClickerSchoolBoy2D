using System.Collections.Generic;
using System.Linq;
using _Scripts.Core;
using _Scripts.Models.Save;
using _Scripts.Models.Upgrade;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager: MonoBehaviour
{
    private Dictionary<int, UpgradeStateItem> upgradeStates = new();
    [SerializeField] private UpgradeClickItemSO upgradeData;
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

        foreach (var item in upgradeData.Upgrades.OrderBy(x => x.Id))
        {
            var isBought = savedById.TryGetValue(item.Id, out var saved) && saved.IsBought;
            //Not secret if it's the first upgrade, or the previous one is already bought
            var isSecret = !previousBought;

            var sui = new UpgradeStateItem(item.Id, isBought, isSecret);
            upgradeStates.TryAdd(item.Id, sui);

            previousBought = isBought;
        }
    }
    
    public UpgradeStateItem GetStateItem(int id) => upgradeStates[id];

    public UpgradeStateItem GetPreviousStateItem(int id) => upgradeStates.TryGetValue(id - 1, out var item) ? item : null;
    
    public SavedUpgradeItem[] GetDataToSave() => upgradeStates.Select(x => new SavedUpgradeItem
    {
        Id = x.Key,
        IsBought = x.Value.IsBought
    }).ToArray();
    
    public int GetPowerValue()
    {
        return CalcCacheHelper.CalcPowerCache(
            upgradeData.Upgrades.Where(x => upgradeStates.ContainsKey(x.Id) && upgradeStates[x.Id].IsBought).ToArray()
        );
    }

    public void Reset()
    {
        foreach (var upgradeStateItem in upgradeStates)
        {
            upgradeStateItem.Value.Reset();
        }
    }
}
