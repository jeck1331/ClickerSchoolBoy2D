using _Scripts.Interfaces;
using System.Collections.Generic;
using _Scripts.Events;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> systems;

    private void Awake()
    {
        foreach (var system in systems)
        {
            if (system is IInitialize iSystem) iSystem.Initialize();
            Debug.Log($"init {system.name}");
        }

        GameEvents.InvokeGameInitialized();
        Debug.Log($"End init");
    }
}
