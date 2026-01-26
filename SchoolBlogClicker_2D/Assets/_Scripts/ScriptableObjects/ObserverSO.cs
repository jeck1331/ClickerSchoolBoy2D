using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Observers/Int Observer")]

    public class IntObserver : ScriptableObserver<int>
    {
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Observers/UInt Observer")]
    public class UIntObserver : ScriptableObserver<uint>
    {
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Observers/ULong Observer")]
    public class ULongObserver : ScriptableObserver<ulong>
    {
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Observers/Float Observer")]
    public class FloatObserver : ScriptableObserver<float>
    {
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Observers/String Observer")]
    public class StringObserver : ScriptableObserver<string>
    {
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Observers/Bool Observer")]
    public class BoolObserver : ScriptableObserver<bool>
    {
    }
}
