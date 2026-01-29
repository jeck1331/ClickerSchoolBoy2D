using UnityEngine;

public abstract class ValueStoreSO<T> : ScriptableObject
{
    [Header("Значение")] [SerializeField] private T value;
    [SerializeField] private ObserverSo[] observers;

    public T Value
    {
        get { return value; }
        set
        {
            if (Equals(this.value, value)) return;

            this.value = value;
            ChangeValue();

            return;
        }
    }

    /// <summary>
    /// Send signal for all observers about changes value
    /// </summary>
    /// <param name="value">New value</param>
    private void ChangeValue()
    {
        foreach (var observer in observers)
            observer.Changing();
    }
}