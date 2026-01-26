using System;
using UnityEngine;

namespace _Scripts.Utilities
{
    public abstract class ScriptableObserver<T>: ScriptableObject
    {
        public event Action<T> OnValueChanged;

        public void DoThing(T val)
        {
            OnValueChanged?.Invoke(val);
        }
    }
}
