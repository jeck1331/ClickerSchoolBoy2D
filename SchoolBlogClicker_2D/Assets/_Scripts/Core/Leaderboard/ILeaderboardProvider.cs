public interface ILeaderboardProvider
{
    void SubmitScore(ulong score);
    ulong GetBestScore();
}
