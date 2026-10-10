using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Prefabs
{
    /// <summary>
    /// Class for circle gameobject from MiniGameSystem
    /// Needed that yourself destroing by time or click
    /// </summary>
    public class CircleLifeCycleMGS: MonoBehaviour
    {
        [SerializeField] private float timeout = 3f;
        [SerializeField] private float warningAnimationStartNormalized = 0.4f;
        [SerializeField] private float warningScaleMultiplier = 1.08f;
        [SerializeField] private ObserverGameObjSO circleClickObserver;
        
        private Coroutine _lifetimeCoroutine;
        private Coroutine _warningCoroutine;
        private RectTransform _rectTransform;
        private Image[] _images;
        private Vector3 _baseScale;
        private Color[] _baseColors;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _images = GetComponentsInChildren<Image>(true);
            _baseColors = new Color[_images.Length];
            if (_rectTransform != null) _baseScale = _rectTransform.localScale;
            for (var i = 0; i < _images.Length; i++)
                _baseColors[i] = _images[i].color;
        }
        
        private void OnEnable()
        {
            _lifetimeCoroutine = StartCoroutine(CircleDestroy_Coroutine());
            _warningCoroutine = StartCoroutine(WarningAnimation_Coroutine());
            if (circleClickObserver != null)
                circleClickObserver.OnValueChanged += HandleCircleClick;
        }
        private void OnDestroy()
        {
            if (circleClickObserver != null)
                circleClickObserver.OnValueChanged -= HandleCircleClick;
        }

        private void HandleCircleClick(GameObject value)
        {
            if (value == gameObject) Kill();
        }

        private void Kill()
        {
            if (_lifetimeCoroutine != null) StopCoroutine(_lifetimeCoroutine);
            if (_warningCoroutine != null) StopCoroutine(_warningCoroutine);
            Destroy(gameObject);
        }
        
        
        private IEnumerator CircleDestroy_Coroutine()
        {
            yield return new WaitForSeconds(timeout);
            Destroy(gameObject);
        }

        private IEnumerator WarningAnimation_Coroutine()
        {
            var warningStart = timeout * warningAnimationStartNormalized;
            if (warningStart > 0f)
                yield return new WaitForSeconds(warningStart);

            var elapsed = 0f;
            while (true)
            {
                // Start from the original color and smoothly blink twice per second.
                var pulse = (1f - Mathf.Cos(elapsed * Mathf.PI * 4f)) * 0.5f;
                if (_rectTransform != null)
                    _rectTransform.localScale = Vector3.Lerp(_baseScale, _baseScale * warningScaleMultiplier, pulse);
                for (var i = 0; i < _images.Length; i++)
                {
                    if (_images[i] == null) continue;
                    var original = _baseColors[i];
                    var red = new Color(1f, 0f, 0f, original.a);
                    _images[i].color = Color.Lerp(original, red, pulse);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        public void Configure(float newTimeout, float warningStartNormalized)
        {
            timeout = Mathf.Max(0.1f, newTimeout);
            warningAnimationStartNormalized = Mathf.Clamp01(warningStartNormalized);
            // Instantiate invokes OnEnable before MiniGameSystem supplies the config.
            if (!isActiveAndEnabled) return;

            if (_lifetimeCoroutine != null) StopCoroutine(_lifetimeCoroutine);
            if (_warningCoroutine != null) StopCoroutine(_warningCoroutine);
            _lifetimeCoroutine = StartCoroutine(CircleDestroy_Coroutine());
            _warningCoroutine = StartCoroutine(WarningAnimation_Coroutine());
        }
    }
}
