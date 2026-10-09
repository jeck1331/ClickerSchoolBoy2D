using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ObserverGameObjSO",menuName = "Scriptable Objects/Observer Game Object")]
public class ObserverGameObjSO: ScriptableObject
{
    public event Action<GameObject> OnValueChanged;
    public Vector2 LastScreenPosition { get; private set; }

    public void ChangingAtScreenPosition(GameObject value, Vector2 screenPosition)
    {
        LastScreenPosition = screenPosition;
        Changing(value);
    }

    public void Changing(GameObject value)
    {
        Debug.Log($"{value.name} - МЕНЯ УДАРИЛИ");
        OnValueChanged?.Invoke(value);
    }
}
