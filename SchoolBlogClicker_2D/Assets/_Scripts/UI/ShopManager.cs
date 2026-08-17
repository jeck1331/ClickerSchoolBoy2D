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
        if (data != null)
        {
            foreach (var item in _shopData.Upgrades)
            {
                //sui - SavedUpgradeItem
                var currentSuiFromSaved = data.SingleOrDefault(x => x.Id == item.Id);
                //If prevBought, but current not bought, then isSecret = true, else false
                var prevBought = !currentSuiFromSaved.IsBought && upgradeStates.Count > 0 && upgradeStates.Last().Value.IsBought;
                
                var sui = new UpgradeStateItem(item.Id, currentSuiFromSaved.IsBought, prevBought);
                upgradeStates.TryAdd(item.Id, sui);
            }
        }
    }

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
