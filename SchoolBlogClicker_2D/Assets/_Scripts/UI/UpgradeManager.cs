using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Core;
using _Scripts.Models.Save;
using _Scripts.Models.Upgrade;
using UnityEngine;

public class UpgradeManager: MonoBehaviour
{
    private Dictionary<int, UpgradeStateItem> upgradeStates = new();
    [SerializeField] private UpgradeClickItemSO upgradeData;

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
