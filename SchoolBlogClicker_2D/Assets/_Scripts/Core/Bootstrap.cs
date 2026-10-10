using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> systems;

    private void Awake()
    {
        // #FOR ANDROID
        // Научится вычислять FrameRate телефона
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
        
        foreach (var system in systems)
        {
            if (system is IInitialize iSystem) iSystem.Initialize();
            Debug.Log($"init {system.name}");
        }
        Debug.Log($"End init");
    }
}
