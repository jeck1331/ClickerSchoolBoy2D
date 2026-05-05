using UnityEngine;

public class ItchLeaderboardProvider : ILeaderboardProvider
{
    private readonly LocalLeaderboardProvider _fallback = new();

    public void SubmitScore(ulong score)
    {
        Debug.Log("Itch leaderboard provider is not wired yet. Using local fallback.");
        _fallback.SubmitScore(score);
    }

    public ulong GetBestScore()
    {
        return _fallback.GetBestScore();
    }
}
