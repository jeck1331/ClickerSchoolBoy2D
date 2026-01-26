using UnityEngine;

namespace _Scripts.Utilities
{
    public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }
        protected virtual void Awake() => Instance = this as T;

        protected virtual void OnApplciationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// "Синглтон" - на самом деле нет, просто хранит статичный инстанс на объект
    /// </summary>
    public abstract class Singleton<T>: StaticInstance<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            base.Awake();
        }
    }

    /// <summary>
    /// Не уничтожаемый синглтон при смене сцен
    /// </summary>
    public abstract class SingletonPersistent<T>: Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}
