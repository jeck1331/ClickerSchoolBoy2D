using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickController : MonoBehaviour
{
    private InputAction _clickAction;
    private InputAction _tapAction;
    private Vector2 _mousePosition;

    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue clickPowerValue;
    [SerializeField] private ParticleSystem vfxStarParticle;

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
            Vector2 screenPosition =
                Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            IncrementCounter(screenPosition);
        }

        void IncrementCounter(Vector2 screenPosition)
        {
            RaycastHit2D hitInformation = Physics2D.Raycast(screenPosition, Camera.main.transform.forward);

            if (hitInformation.collider != null){
                if (hitInformation.collider.CompareTag(gameObject.name))
                {
                    GameObject touchedObject = hitInformation.transform.gameObject;
                    vfxStarParticle.transform.position = screenPosition;
                    vfxStarParticle.Play();

                    Debug.Log(touchedObject.name);

                    scoreValue!.Value += clickPowerValue!.Value;
                } else if (hitInformation.collider.CompareTag("ClickItemMG"))
                {
                    GameObject touchedObject = hitInformation.transform.gameObject;
                    vfxStarParticle.transform.position = screenPosition;
                    vfxStarParticle.Play();

                    Debug.Log(touchedObject.name);
                    Debug.Log($"Крит удар: {clickPowerValue!.Value * 1.25}");

                    scoreValue!.Value += Convert.ToUInt64(Math.Abs(clickPowerValue!.Value * 1.25));
                }
            }
        }
    }
}