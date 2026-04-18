using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ClickController : MonoBehaviour
{
    private InputAction _clickAction;
    private InputAction _tapAction;
    private Vector2 _mousePosition;

    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue clickPowerValue;
    [SerializeField] private ParticleSystem vfxStarParticle;
    [SerializeField] private ParticleSystem vfxCritStarParticle;
    [SerializeField] private ObserverGameObjSO circleObserver;

    private void Start()
    {
        _clickAction = InputSystem.actions.FindAction("Click");
        _tapAction = InputSystem.actions.FindAction("Tap");
    }

    private void Update()
    {
        if (!TryGetPointerPosition(out Vector3 pointerPosition))
            return;
        Vector2 screenPosition = Camera.main.ScreenToWorldPoint(pointerPosition);
        int layerMask = 1 << LayerMask.NameToLayer("TapZone");
        Collider2D hitCollider = Physics2D.OverlapPoint(screenPosition, layerMask);
        
        if (hitCollider != null){
            if (hitCollider.CompareTag(gameObject.name))
            {
                vfxStarParticle.transform.position = screenPosition;
                vfxStarParticle.Play();
        
                scoreValue!.Value += clickPowerValue!.Value;
            } else if (hitCollider.CompareTag("ClickItemMG"))
            {
                GameObject touchedObject = hitCollider.transform.gameObject;
                circleObserver.Changing(touchedObject);
                vfxCritStarParticle.transform.position = screenPosition;
                vfxCritStarParticle.Play();
                float addCoefficient = Random.Range(1.05f, 2f);
                Debug.Log($"Крит удар: {Convert.ToUInt64(Math.Abs(clickPowerValue!.Value * addCoefficient))}; кэф: {addCoefficient}");
                scoreValue!.Value += Convert.ToUInt64(Math.Abs(clickPowerValue!.Value * addCoefficient));
            }
        }

        // if (hitCollider != null && hitCollider.name == gameObject.name)
        // {
        //     Debug.Log(hitCollider.name);
        //
        //     ClickerEvents.InvokeClickOrTap();
        // }
        // if (Mouse.current != null && _clickAction.WasReleasedThisFrame())
        // {
        //     Vector2 screenPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        //     IncrementCounter(screenPosition);
        // }
        //
        // if (Touchscreen.current != null && _tapAction.WasReleasedThisFrame())
        // {
        //     Vector2 screenPosition =
        //         Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
        //     IncrementCounter(screenPosition);
        // }
        //
        // void IncrementCounter(Vector2 screenPosition)
        // {
        //     RaycastHit2D hitInformation = Physics2D.Raycast(screenPosition, Camera.main.transform.forward);
        //
        // if (hitInformation.collider != null){
        //     if (hitInformation.collider.CompareTag(gameObject.name))
        //     {
        //         vfxStarParticle.transform.position = screenPosition;
        //         vfxStarParticle.Play();
        //
        //         scoreValue!.Value += clickPowerValue!.Value;
        //     } else if (hitInformation.collider.CompareTag("ClickItemMG"))
        //     {
        //         GameObject touchedObject = hitInformation.transform.gameObject;
        //         circleObserver.Changing(touchedObject);
        //         vfxCritStarParticle.transform.position = screenPosition;
        //         vfxCritStarParticle.Play();
        //         float addCoefficient = Random.Range(1.05f, 2f);
        //         Debug.Log($"Крит удар: {Convert.ToUInt64(Math.Abs(clickPowerValue!.Value * addCoefficient))}; кэф: {addCoefficient}");
        //         scoreValue!.Value += Convert.ToUInt64(Math.Abs(clickPowerValue!.Value * addCoefficient));
        //     }
        // }
        // }
    }
    
    private bool TryGetPointerPosition(out Vector3 pointerPosition)
    {
        if (Mouse.current != null && _clickAction.WasPressedThisFrame())
        {
            pointerPosition = Mouse.current.position.ReadValue();
            return true;
        }

        if (Touchscreen.current != null && _tapAction.WasPressedThisFrame())
        {
            pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        pointerPosition = new Vector3();
        return false;
    }
}