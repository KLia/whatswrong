using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class BackgroundBlurController : MonoBehaviour, ISceneUnlaodHandler
    {
        [Header("Animation")]
        [SerializeField] private float blurDuration = 0.25f;
        [SerializeField] private AnimationCurve blurCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Blur Settings")]
        [SerializeField] private bool enableBlur = true;
        [SerializeField] [Range(0f, 10f)] private float blurIntensity = 5f;
        [SerializeField] private float focusDistance = 3f;

        private Volume _globalVolume;
        private VolumeProfile _runtimeProfile;
        private DepthOfField _depthOfField;
        private Coroutine _blurCoroutine;

        private void Awake()
        {
            if (enableBlur)
                SetupBlurVolume();
        }

        private void SetupBlurVolume()
        {
            Camera controlledCamera = GetComponent<Camera>();

            if (TryGetComponent(out UniversalAdditionalCameraData cameraData))
            {
                cameraData.renderPostProcessing = true;
                cameraData.requiresDepthTexture = true;
            }

            _globalVolume = GetComponent<Volume>();
            if (_globalVolume == null)
                _globalVolume = gameObject.AddComponent<Volume>();

            _globalVolume.isGlobal = true;
            _globalVolume.priority = 100f;

            _runtimeProfile = ScriptableObject.CreateInstance<VolumeProfile>();
            _depthOfField = _runtimeProfile.Add<DepthOfField>(true);

            ConfigureDepthOfField(focusDistance);

            _globalVolume.profile = _runtimeProfile;
            _globalVolume.weight = 0f;
        }

        private void ConfigureDepthOfField(float targetFocusDistance)
        {
            if (_depthOfField == null)
                return;

            float normalizedBlur = Mathf.Clamp01(blurIntensity / 10f);
            float blurStart = targetFocusDistance + 0.35f;
            float blurRange = Mathf.Lerp(1.5f, 4f, normalizedBlur);

            _depthOfField.active = true;

            _depthOfField.mode.overrideState = true;
            _depthOfField.mode.value = DepthOfFieldMode.Gaussian;

            _depthOfField.gaussianStart.overrideState = true;
            _depthOfField.gaussianStart.value = blurStart;

            _depthOfField.gaussianEnd.overrideState = true;
            _depthOfField.gaussianEnd.value = blurStart + blurRange;

            _depthOfField.gaussianMaxRadius.overrideState = true;
            _depthOfField.gaussianMaxRadius.value = Mathf.Lerp(0.2f, 1f, normalizedBlur);

            _depthOfField.highQualitySampling.overrideState = true;
            _depthOfField.highQualitySampling.value = normalizedBlur > 0.5f;

            _depthOfField.focusDistance.overrideState = true;
            _depthOfField.focusDistance.value = targetFocusDistance;
        }

        public void ShowBlur()
        {
            if (!enableBlur || _globalVolume == null)
                return;

            if (_blurCoroutine != null)
                StopCoroutine(_blurCoroutine);

            _blurCoroutine = StartCoroutine(AnimateBlur(_globalVolume.weight, 1f));
        }

        public void HideBlur()
        {
            if (!enableBlur || _globalVolume == null)
                return;

            if (_blurCoroutine != null)
                StopCoroutine(_blurCoroutine);

            _blurCoroutine = StartCoroutine(AnimateBlur(_globalVolume.weight, 0f));
        }

        private IEnumerator AnimateBlur(float startWeight, float endWeight)
        {
            float elapsed = 0f;

            while (elapsed < blurDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / blurDuration);
                float curved = blurCurve.Evaluate(t);

                _globalVolume.weight = Mathf.Lerp(startWeight, endWeight, curved);
                yield return null;
            }

            _globalVolume.weight = endWeight;
            _blurCoroutine = null;
        }

        public void OnBeforeSceneUnload()
        {
            if (_blurCoroutine != null)
            {
                StopCoroutine(_blurCoroutine);
                _blurCoroutine = null;
            }

            if (_globalVolume != null)
                _globalVolume.weight = 0f;
        }
    }
}