using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Config/Mini Game Config")]
public class MiniGameConfigSO : ScriptableObject
{
    [Header("Round Timing")]
    [SerializeField] private float roundStartDelayMin = 6f;
    [SerializeField] private float roundStartDelayMax = 12f;
    [SerializeField] private int circleCountPerRound = 3;
    [SerializeField] private Vector2 delayBetweenCircles = new Vector2(0.25f, 0.5f);

    [Header("Circle")]
    [SerializeField] private Vector2 circleLifetime = new Vector2(1.5f, 2f);
    [SerializeField] private float warningAnimationStartNormalized = 0.4f;

    [Header("Reward")]
    [SerializeField] private float minRewardMultiplier = 1.05f;
    [SerializeField] private float maxRewardMultiplier = 2.2f;

    public float RoundStartDelayMin => roundStartDelayMin;
    public float RoundStartDelayMax => roundStartDelayMax;
    public int CircleCountPerRound => Mathf.Max(1, circleCountPerRound);
    public float DelayBetweenCircles => UnityEngine.Random.Range(
        Mathf.Max(0f, delayBetweenCircles.x), Mathf.Max(delayBetweenCircles.x, delayBetweenCircles.y));
    public float CircleLifetime => Mathf.Max(0.1f, circleLifetime.y);

    public float GetFirstCircleLifetime() => UnityEngine.Random.Range(
        Mathf.Max(0.1f, circleLifetime.x), Mathf.Max(0.1f, Mathf.Max(circleLifetime.x, circleLifetime.y)));
    public float WarningAnimationStartNormalized => Mathf.Clamp01(warningAnimationStartNormalized);

    public long CalculateReward(int clickPower, float reactionTime)
    {
        var normalized = Mathf.Clamp01(reactionTime / CircleLifetime);
        var multiplier = Mathf.Lerp(maxRewardMultiplier, minRewardMultiplier, normalized);
        var result = Math.Max(1f, clickPower * multiplier);
        return Convert.ToInt64(result);
    }
}
