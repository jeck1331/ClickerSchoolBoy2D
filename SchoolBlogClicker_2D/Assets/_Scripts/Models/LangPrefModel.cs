using System;
using UnityEngine;

[Serializable]
public class LangPrefModel
{
    [Header("Играть")] public string Play;
    [Header("Выход")] public string Exit;
    [Header("Настройки")] public string Settings;
    [Header("Аудио")] public string Audio;
    [Header("Громкость")] public string Volume;
    [Header("Музыка")] public string Music;
    [Header("Улучшение")] public string Upgrade;
    [Header("Монет в секундку")] public string CoinsPerSecond;
    [Header("Монеты")] public string Coins;
    [Header("Подписчики")] public string Subscribers;
}