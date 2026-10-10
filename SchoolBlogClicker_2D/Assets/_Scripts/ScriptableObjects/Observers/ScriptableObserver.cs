using System;
using UnityEngine;

public class ScriptableObserverBase : ScriptableObject
{
    public event Action OnValueChanged;

    public void Changing()
    {
        OnValueChanged?.Invoke();
    }
}