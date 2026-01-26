using _Scripts.Models;
using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LangPref", menuName = "Scriptable Objects/LangPref")]

    public class LangPref : ScriptableObject
    {
        [Header("Язык")] public Language Language;
        
        [Header("Словарь")] [SerializeField] public LangPrefModel Dictionary;
    }
}
