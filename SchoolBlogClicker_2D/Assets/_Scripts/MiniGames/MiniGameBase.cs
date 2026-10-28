using UnityEngine;

public abstract class MiniGameBase : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        GameplayPauseService.OnPauseChanged += HandlePause;
    }
    protected virtual void OnDisable()
    {
        GameplayPauseService.OnPauseChanged -= HandlePause;
    }

    protected virtual void HandlePause(bool paused) {}
}
