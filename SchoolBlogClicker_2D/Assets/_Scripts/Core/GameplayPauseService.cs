using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayPauseService
{
    private const string InMenuValueResourcePath = "Data/InMenuValue";
    private const string InMenuObserverResourcePath = "Observers/InMenuObserver";

    private static readonly HashSet<object> PauseOwners = new();
    private static BoolValue _inMenuValue;
    private static ObserverSO _inMenuObserver;
    private static bool _isGameplayInputPaused;

    public static event Action<bool> OnPauseChanged;

    public static bool IsGameplayInputPaused => _isGameplayInputPaused;

    public static void SetPaused(object owner, bool paused)
    {
        if (owner == null) return;

        if (paused)
            PauseOwners.Add(owner);
        else
            PauseOwners.Remove(owner);

        ApplyPauseState(PauseOwners.Count > 0);
    }

    public static void ForceSetPaused(bool paused)
    {
        PauseOwners.Clear();
        ApplyPauseState(paused);
    }

    private static void ApplyPauseState(bool paused)
    {
        if (_isGameplayInputPaused == paused) return;

        _isGameplayInputPaused = paused;
        LoadResourcesIfNeeded();

        if (_inMenuValue != null)
            _inMenuValue.Value = paused;

        _inMenuObserver?.Changing();
        OnPauseChanged?.Invoke(paused);
    }

    private static void LoadResourcesIfNeeded()
    {
        if (_inMenuValue == null)
            _inMenuValue = Resources.Load<BoolValue>(InMenuValueResourcePath);

        if (_inMenuObserver == null)
            _inMenuObserver = Resources.Load<ObserverSO>(InMenuObserverResourcePath);
    }
}
