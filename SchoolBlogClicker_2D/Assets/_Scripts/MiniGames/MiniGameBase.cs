using UnityEngine;
using System.Collections.Generic;
using TMPro;

public abstract class MiniGameBase : MonoBehaviour
{
    private List<MiniGameScorePopup> scorePopups;

    protected virtual void OnEnable()
    {
        GameplayPauseService.OnPauseChanged += HandlePause;
    }
    protected virtual void OnDisable()
    {
        GameplayPauseService.OnPauseChanged -= HandlePause;
        HideScorePopups();
    }

    protected virtual void HandlePause(bool paused) {}

    protected void ShowScorePopup(TMP_Text scoreText, Vector2 screenPosition)
    {
        if (scoreText == null || scoreText.canvas == null) return;
        scorePopups ??= new List<MiniGameScorePopup>();
        foreach (MiniGameScorePopup popup in scorePopups)
        {
            if (popup.IsVisible) continue;
            popup.Show(screenPosition);
            return;
        }

        var newPopup = new MiniGameScorePopup(scoreText);
        scorePopups.Add(newPopup);
        newPopup.Show(screenPosition);
    }

    protected void TickScorePopups(float deltaTime)
    {
        if (scorePopups == null) return;
        foreach (MiniGameScorePopup popup in scorePopups) popup.Tick(deltaTime);
    }

    protected void HideScorePopups()
    {
        if (scorePopups == null) return;
        foreach (MiniGameScorePopup popup in scorePopups) popup.Hide();
    }

    protected virtual void OnDestroy()
    {
        if (scorePopups == null) return;
        foreach (MiniGameScorePopup popup in scorePopups) popup.Destroy();
    }
}
