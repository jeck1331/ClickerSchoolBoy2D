using System.Collections;
using UnityEngine;

namespace _Scripts.Prefabs
{
    /// <summary>
    /// Class for circle gameobject from MiniGameSystem
    /// Needed that yourself destroing by time or click
    /// </summary>
    public class CircleLifeCycleMGS: MonoBehaviour
    {
        [SerializeField] private float timeout = 3f;
        [SerializeField] private ObserverGameObjSO circleClickObserver;
        
        private Coroutine _lifetimeCoroutine;
        private void OnEnable()
        {
            _lifetimeCoroutine = StartCoroutine(CircleDestroy_Coroutine());
            circleClickObserver.OnValueChanged += HandleCircleClick;
        }
        private void OnDestroy()
        {
            circleClickObserver.OnValueChanged -= HandleCircleClick;
        }

        private void HandleCircleClick(GameObject value)
        {
            if (value == gameObject) Kill();
        }

        private void Kill()
        {
            if (_lifetimeCoroutine != null) StopCoroutine(_lifetimeCoroutine);
            Destroy(gameObject);
        }
        
        
        private IEnumerator CircleDestroy_Coroutine()
        {
            yield return new WaitForSeconds(timeout);
            Destroy(gameObject);
        }
    }
}