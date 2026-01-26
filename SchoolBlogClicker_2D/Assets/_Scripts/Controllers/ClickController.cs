using _Scripts.Events;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickController : MonoBehaviour
{
    private InputAction _clickAction;
    private InputAction _tapAction;

    private void Start()
    {
        _clickAction = InputSystem.actions.FindAction("Click");
        _tapAction = InputSystem.actions.FindAction("Tap");
    }

    private void Update()
    {
        if (Mouse.current != null && _clickAction.WasReleasedThisFrame())
        {
            Vector2 screenPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            IncrementCounter(screenPosition);
        }

        if (Touchscreen.current != null && _tapAction.WasReleasedThisFrame())
        {
            Vector2 screenPosition = Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            IncrementCounter(screenPosition);
        }

        void IncrementCounter(Vector2 screenPosition)
        {
            
            RaycastHit2D hitInformation = Physics2D.Raycast(screenPosition, Camera.main.transform.forward);

            if (hitInformation.collider != null && hitInformation.collider.name == gameObject.name)
            {
                GameObject touchedObject = hitInformation.transform.gameObject;

                Debug.Log(touchedObject.name);

                ClickerEvents.InvokeClickOrTap();
            }
        }
    }


}
