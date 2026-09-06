using System.Collections.Generic;
using System.Linq;
using _Scripts.Core;
using _Scripts.Models.Save;
using _Scripts.Models.Upgrade;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private Dictionary<int, UpgradeStateItem> upgradeStates = new Dictionary<int, UpgradeStateItem>();
    [SerializeField] private UpgradeIncomeItemSO _shopData;

    public void FillDataFromSave(List<SavedUpgradeItem> data)
    {
        var savedById = data?.ToDictionary(x => x.Id) ?? new Dictionary<int, SavedUpgradeItem>();
        var previousBought = true;

        foreach (var item in _shopData.Upgrades.OrderBy(x => x.Id))
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

    public int GetIncomeValue()
    {
        return CalcCacheHelper.CalcIncomeCache(
            _shopData.Upgrades.Where(x => upgradeStates.ContainsKey(x.Id) && upgradeStates[x.Id].IsBought).ToArray()
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
