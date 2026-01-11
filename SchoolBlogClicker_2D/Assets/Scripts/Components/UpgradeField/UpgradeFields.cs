using System.Collections.Generic;
using UnityEngine;

public class UpgradeFields : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<UpgradeFieldModel> itemsData;

    [Header("References")]
    [SerializeField] private UpgradeFieldPrefab itemPrefab;

    private readonly List<UpgradeFieldPrefab> _spawnedItems = new();

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        Clear();

        foreach (var data in itemsData)
        {
            var item = Instantiate(itemPrefab, transform);
            item.Initialize(data);
            _spawnedItems.Add(item);
        }
    }

    private void Clear()
    {
        foreach (var item in _spawnedItems)
        {
            Destroy(item.gameObject);
        }

        _spawnedItems.Clear();
    }
}
