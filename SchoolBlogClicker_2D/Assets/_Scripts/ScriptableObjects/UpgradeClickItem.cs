using System.Collections.Generic;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/Click Item Upgrade")]
    public class UpgradeClickItem : BaseUpgradeItem_SO
    {
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/StaticData/Click Items Upgrade")]
    public class StateUpgradeClickItems : ScriptableObject
    {
        [Header("Улучшения клика\\тапа")] [SerializeField]
        private List<BaseUpgradeItem_SO> items;
    }
}