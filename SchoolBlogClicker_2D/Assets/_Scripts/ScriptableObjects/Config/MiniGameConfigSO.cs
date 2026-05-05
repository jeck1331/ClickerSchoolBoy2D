using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Config/Mini Game Config")]
public class MiniGameConfigSO : ScriptableObject
{
    [Header("Round Timing")]
    [SerializeField] private float roundStartDelayMin = 6f;
    [SerializeField] private float roundStartDelayMax = 12f;
    [SerializeField] private int circleCountPerRound = 3;
    [SerializeField] private float delayBetweenCircles = 0.3f;

    [Header("Circle")]
    [SerializeField] private float circleLifetime = 2.2f;
    [SerializeField] private float warningAnimationStartNormalized = 0.65f;
    [SerializeField] private float circleRadius = 187f;

    [Header("Reward")]
    [SerializeField] private float minRewardMultiplier = 1.05f;
    [SerializeField] private float maxRewardMultiplier = 2.2f;

    public float RoundStartDelayMin => roundStartDelayMin;
    public float RoundStartDelayMax => roundStartDelayMax;
    public int CircleCountPerRound => Mathf.Max(1, circleCountPerRound);
    public float DelayBetweenCircles => Mathf.Max(0f, delayBetweenCircles);
    public float CircleLifetime => Mathf.Max(0.1f, circleLifetime);
    public float WarningAnimationStartNormalized => Mathf.Clamp01(warningAnimationStartNormalized);
    public float CircleRadius => Mathf.Max(1f, circleRadius);

    public ulong CalculateReward(uint clickPower, float reactionTime)
    {
        var normalized = Mathf.Clamp01(reactionTime / CircleLifetime);
        var multiplier = Mathf.Lerp(maxRewardMultiplier, minRewardMultiplier, normalized);
        var result = Math.Max(1f, clickPower * multiplier);
        return Convert.ToUInt64(result);
    }
}
