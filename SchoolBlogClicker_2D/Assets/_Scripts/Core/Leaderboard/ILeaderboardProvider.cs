public interface ILeaderboardProvider
{
    void SubmitScore(long score);
    long GetBestScore();
}
