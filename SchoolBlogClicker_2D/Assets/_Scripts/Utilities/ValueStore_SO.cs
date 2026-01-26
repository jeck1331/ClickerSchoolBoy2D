using UnityEngine;

namespace _Scripts.Utilities
{
    public abstract class ValueStore<T>:  ScriptableObject
    {
        [Header("Значение")][SerializeField] private T _value;
        [SerializeField] private ScriptableObserver<T>[] _observers;
            
        public T Value
        {
            get { return _value; }
            set
            {
                if (Equals(_value, value)) return;
                
                _value = value;
                ChangeValue(value);
                
                return;
            }
        }

        /// <summary>
        /// Send signal for all observers about changes value
        /// </summary>
        /// <param name="value">New value</param>
        private void ChangeValue(T value)
        {
            foreach (var observer in _observers)
                observer.DoThing(value);
        }
    }
}