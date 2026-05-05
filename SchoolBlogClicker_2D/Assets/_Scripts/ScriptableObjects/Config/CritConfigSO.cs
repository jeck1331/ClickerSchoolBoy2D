using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Config/Crit Config")]
public class CritConfigSO : ScriptableObject
{
    [Header("Chance")]
    [SerializeField] private float baseChance = 0.05f;
    [SerializeField] private float chancePerScoreUnit = 0.00000005f;
    [SerializeField] private float maxChance = 0.45f;

    [Header("Multiplier")]
    [SerializeField] private float minMultiplierAtStart = 1.5f;
    [SerializeField] private float maxMultiplierAtStart = 2.1f;
    [SerializeField] private float minMultiplierAtHighScore = 2.0f;
    [SerializeField] private float maxMultiplierAtHighScore = 3.1f;
    [SerializeField] private ulong scoreForMaxMultiplierRange = 1000000;

    public float CalculateChance(ulong score)
    {
        var chance = baseChance + score * chancePerScoreUnit;
        return Mathf.Clamp(chance, 0f, maxChance);
    }

    public Vector2 CalculateMultiplierRange(ulong score)
    {
        var t = scoreForMaxMultiplierRange == 0 ? 1f : Mathf.Clamp01(score / (float)scoreForMaxMultiplierRange);
        var min = Mathf.Lerp(minMultiplierAtStart, minMultiplierAtHighScore, t);
        var max = Mathf.Lerp(maxMultiplierAtStart, maxMultiplierAtHighScore, t);
        return new Vector2(min, Mathf.Max(min, max));
    }
}
