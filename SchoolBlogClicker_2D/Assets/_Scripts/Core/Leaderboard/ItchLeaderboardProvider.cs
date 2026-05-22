using UnityEngine;

public class ItchLeaderboardProvider : ILeaderboardProvider
{
    private readonly LocalLeaderboardProvider _fallback = new();

    public void SubmitScore(long score)
    {
        Debug.Log("Itch leaderboard provider is not wired yet. Using local fallback.");
        _fallback.SubmitScore(score);
    }

    public long GetBestScore()
    {
        return _fallback.GetBestScore();
    }
}
