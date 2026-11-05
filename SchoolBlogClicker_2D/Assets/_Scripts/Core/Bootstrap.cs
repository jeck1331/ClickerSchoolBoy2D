using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> systems;

    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            Application.targetFrameRate = Mathf.CeilToInt((float)Screen.currentResolution.refreshRateRatio.value);
        }
        else
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
        }
        
        foreach (var system in systems)
        {
            if (system is IInitialize iSystem) iSystem.Initialize();
            Debug.Log($"init {system.name}");
        }
        Debug.Log($"End init");
    }
}
