using System.Globalization;
using _Scripts.Models.Upgrade;
using UnityEngine;

public static class GameText
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en");
    private static readonly CultureInfo Russian = CultureInfo.GetCultureInfo("ru");
    private static LangPref russianCatalog;

    public static bool IsRussian => SettingsService.Language == GameLanguage.Russian;
    public static string Choose(string english, string russian) => IsRussian ? russian : english;
    public static string Number(decimal value, string format = "0.##") =>
        value.ToString(format, IsRussian ? Russian : English);

    public static string Buy(string price) =>
        IsRussian ? $"Купить{(price.Length > 4 ? "\n" : " ")}{price}" : $"Buy {price}";

    public static string Results(long score, long reward) =>
        Choose($"Score: {score}\nReward: {reward}", $"Счёт: {score}\nНаграда: {reward}");

    public static UpgradeTexts Upgrade(int id, bool isIncome)
    {
        if (!IsRussian) return null;
        if (russianCatalog == null)
            russianCatalog = Resources.Load<LangPref>("StaticData/RuLangPref");

        var dictionary = russianCatalog?.Dictionary;
        var items = isIncome ? dictionary?.UpgradeIncomeTexts : dictionary?.UpgradeClickTexts;
        return items != null && id >= 0 && id < items.Length ? items[id] : null;
    }
}
