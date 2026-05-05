using UnityEngine;

public enum LeaderboardPlatform
{
    Local,
    Yandex,
    Itch
}

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private ObserverSO scoreObserver;
    [SerializeField] private LeaderboardPlatform platform = LeaderboardPlatform.Local;

    private ILeaderboardProvider _provider;

    private void Awake()
    {
        _provider = platform switch
        {
            LeaderboardPlatform.Yandex => new YandexLeaderboardProvider(),
            LeaderboardPlatform.Itch => new ItchLeaderboardProvider(),
            _ => new LocalLeaderboardProvider()
        };
    }

    private void OnEnable()
    {
        scoreObserver.OnValueChanged += SubmitCurrentScore;
    }

    private void OnDisable()
    {
        scoreObserver.OnValueChanged -= SubmitCurrentScore;
    }

    private void SubmitCurrentScore()
    {
        _provider?.SubmitScore(scoreValue.Value);
    }

    public ulong GetBestScore()
    {
        return _provider?.GetBestScore() ?? 0;
    }
}
