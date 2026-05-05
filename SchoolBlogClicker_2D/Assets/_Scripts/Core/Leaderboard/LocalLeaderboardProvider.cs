using UnityEngine;

public class LocalLeaderboardProvider : ILeaderboardProvider
{
    private const string BestScoreKey = "leaderboard.local.bestScore";

    public void SubmitScore(ulong score)
    {
        var best = GetBestScore();
        if (score <= best) return;
        PlayerPrefs.SetString(BestScoreKey, score.ToString());
        PlayerPrefs.Save();
    }

    public ulong GetBestScore()
    {
        return ulong.TryParse(PlayerPrefs.GetString(BestScoreKey, "0"), out var value) ? value : 0;
    }
}
