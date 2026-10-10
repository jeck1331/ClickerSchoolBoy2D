using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private ObserverSO scoreObserver;

    private readonly HashSet<string> _unlocked = new();
    private int _critStreak;
    private int _miniSuccessRounds;
    private int _upgradesPurchased;

    private void OnEnable()
    {
        scoreObserver.OnValueChanged += CheckScoreAchievements;
        GameEvents.OnClickResolved += HandleClickResolved;
        GameEvents.OnMiniGameRoundCompleted += HandleMiniRoundCompleted;
        GameEvents.OnUpgradePurchased += HandleUpgradePurchased;
        LoadState();
    }

    private void OnDisable()
    {
        scoreObserver.OnValueChanged -= CheckScoreAchievements;
        GameEvents.OnClickResolved -= HandleClickResolved;
        GameEvents.OnMiniGameRoundCompleted -= HandleMiniRoundCompleted;
        GameEvents.OnUpgradePurchased -= HandleUpgradePurchased;
        SaveState();
    }

    private void HandleClickResolved(bool isCrit, long _)
    {
        _critStreak = isCrit ? _critStreak + 1 : 0;
        if (_critStreak >= 5) Unlock("crit_streak_5");
    }

    private void HandleMiniRoundCompleted(MiniGameRoundResult result, int _)
    {
        if (result != MiniGameRoundResult.Success) return;
        _miniSuccessRounds++;
        if (_miniSuccessRounds >= 10) Unlock("mini_round_10_success");
    }

    private void HandleUpgradePurchased()
    {
        _upgradesPurchased++;
        if (_upgradesPurchased >= 10) Unlock("upgrade_buy_10");
    }

    private void CheckScoreAchievements()
    {
        if (scoreValue.Value >= 1000) Unlock("score_1k");
        if (scoreValue.Value >= 100000) Unlock("score_100k");
        if (scoreValue.Value >= 1000000) Unlock("score_1m");
    }

    private void Unlock(string achievementId)
    {
        if (!_unlocked.Add(achievementId)) return;
        Debug.Log($"Achievement unlocked: {achievementId}");
        PlayerPrefs.SetInt($"achievement.{achievementId}", 1);
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        _critStreak = PlayerPrefs.GetInt("achievement.progress.critStreak", 0);
        _miniSuccessRounds = PlayerPrefs.GetInt("achievement.progress.miniSuccessRounds", 0);
        _upgradesPurchased = PlayerPrefs.GetInt("achievement.progress.upgradesPurchased", 0);
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt("achievement.progress.critStreak", _critStreak);
        PlayerPrefs.SetInt("achievement.progress.miniSuccessRounds", _miniSuccessRounds);
        PlayerPrefs.SetInt("achievement.progress.upgradesPurchased", _upgradesPurchased);
        PlayerPrefs.Save();
    }
}
