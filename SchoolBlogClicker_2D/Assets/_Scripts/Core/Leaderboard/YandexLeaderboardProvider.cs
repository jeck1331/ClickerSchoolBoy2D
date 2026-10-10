using UnityEngine;

public class YandexLeaderboardProvider : ILeaderboardProvider
{
    private readonly LocalLeaderboardProvider _fallback = new();

    public void SubmitScore(long score)
    {
        Debug.Log("Yandex leaderboard provider is not wired yet. Using local fallback.");
        _fallback.SubmitScore(score);
    }

    public long GetBestScore()
    {
        return _fallback.GetBestScore();
    }
}
