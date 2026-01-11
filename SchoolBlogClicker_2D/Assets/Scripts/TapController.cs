using Assets.Scripts;
using System;
using Unity.VisualScripting;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using UnityEngine.InputSystem;

public class TapController : MonoBehaviour
{
    InputAction clickAction;
    InputAction tapAction;

    public event Action<ulong> TapOnBox;

    private ulong gameCount = 0;
    private uint incrementValue = 1;

    private void Start()
    {
        UpdateState();

        clickAction = InputSystem.actions.FindAction("Click");
        tapAction = InputSystem.actions.FindAction("Tap");
    }

    private void UpdateState()
    {
        ulong.TryParse(PlayerPrefs.GetString(Constants.GAME_COUNT), out ulong playerGameCount);
        uint.TryParse(PlayerPrefs.GetString(Constants.INCREMENT_VALUE), out uint playerIncrementValue);

        gameCount = playerGameCount > 0 ? playerGameCount : 0;
        incrementValue = playerIncrementValue > 1 ? playerIncrementValue : 1;
    }

    private void Update()
    {
        if (Mouse.current != null && clickAction.WasReleasedThisFrame())
        {
            Vector2 screenPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            IncrementCounter(screenPosition);
        }

        if (Touchscreen.current != null && tapAction.WasReleasedThisFrame())
        {
            Vector2 screenPosition = Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            IncrementCounter(screenPosition);
        }

        void IncrementCounter(Vector2 screenPosition)
        {
            UpdateState();
            RaycastHit2D hitInformation = Physics2D.Raycast(screenPosition, Camera.main.transform.forward);

            if (hitInformation.collider != null && hitInformation.collider.name == gameObject.name)
            {
                GameObject touchedObject = hitInformation.transform.gameObject;

                Debug.Log(touchedObject.name);

                gameCount += incrementValue;
                TapOnBox.Invoke(gameCount);
            }
        }
    }


}
