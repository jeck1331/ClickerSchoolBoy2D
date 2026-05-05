using UnityEngine;

public class YandexLeaderboardProvider : ILeaderboardProvider
{
    private readonly LocalLeaderboardProvider _fallback = new();

    public void SubmitScore(ulong score)
    {
        Debug.Log("Yandex leaderboard provider is not wired yet. Using local fallback.");
        _fallback.SubmitScore(score);
    }

    public ulong GetBestScore()
    {
        return _fallback.GetBestScore();
    }
}
