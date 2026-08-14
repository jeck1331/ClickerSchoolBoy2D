using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ObserverGameObjSO",menuName = "Scriptable Objects/Observer Game Object")]
public class ObserverGameObjSO: ScriptableObject
{
    public event Action<GameObject> OnValueChanged;

    public void Changing(GameObject value)
    {
        Debug.Log($"{value.name} - МЕНЯ УДАРИЛИ");
        OnValueChanged?.Invoke(value);
    }
}