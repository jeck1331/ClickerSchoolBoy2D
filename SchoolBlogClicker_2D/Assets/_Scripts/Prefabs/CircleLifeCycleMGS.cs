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
        [SerializeField] private float warningAnimationStartNormalized = 0.65f;
        [SerializeField] private float warningScaleMultiplier = 1.08f;
        [SerializeField] private ObserverGameObjSO circleClickObserver;
        
        private Coroutine _lifetimeCoroutine;
        private Coroutine _warningCoroutine;
        private RectTransform _rectTransform;
        private Image _image;
        private Vector3 _baseScale;
        private Color _baseColor;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            if (_rectTransform != null) _baseScale = _rectTransform.localScale;
            if (_image != null) _baseColor = _image.color;
        }
        
        private void OnEnable()
        {
            _lifetimeCoroutine = StartCoroutine(CircleDestroy_Coroutine());
            _warningCoroutine = StartCoroutine(WarningAnimation_Coroutine());
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

            while (true)
            {
                var pulse = Mathf.PingPong(Time.time * 3.5f, 1f);
                if (_rectTransform != null)
                    _rectTransform.localScale = Vector3.Lerp(_baseScale, _baseScale * warningScaleMultiplier, pulse);
                if (_image != null)
                {
                    var color = _baseColor;
                    color.a = Mathf.Lerp(0.45f, _baseColor.a, pulse);
                    _image.color = color;
                }
                yield return null;
            }
        }

        public void Configure(float newTimeout, float warningStartNormalized)
        {
            timeout = Mathf.Max(0.1f, newTimeout);
            warningAnimationStartNormalized = Mathf.Clamp01(warningStartNormalized);
        }
    }
}