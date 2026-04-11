using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Game.Scripts;

public class CameraZoomController : MonoBehaviour, ISceneUnloadHandler
{
    [Header("Zoom Animation Settings")]
    [Tooltip("Durata dell'animazione di zoom (in secondi)")]
    public float zoomDuration = 1f;
    
    [Tooltip("Curva di animazione per lo zoom")]
    public AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Blur Settings")]
    [Tooltip("Abilita il blur quando zoomato")]
    public bool enableBlur = true;

    [Tooltip("Intensità del blur (0-10)")]
    public float blurIntensity = 5f;

    private Volume globalVolume;
    private VolumeProfile runtimeProfile;
    private DepthOfField depthOfField;
    private Camera controlledCamera;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isZoomed = false;
    private Transform currentTarget = null;
    private Coroutine currentZoomCoroutine;
    private ClickableObject currentObject = null;
    private ClickableObject _clickedObject;
    private Vector3 _targetWorldPos;
    private float _scale;
    private float _cameraDistance;

    // Proprietà pubbliche per ClickableObject
    public bool IsZoomed => isZoomed;
    public Transform CurrentTarget => currentTarget;

    private void Start()
    {
        controlledCamera = GetComponent<Camera>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Trova o crea il Global Volume per il blur
        if (enableBlur)
        {
            SetupBlurVolume();
        }
    }

    private void SetupBlurVolume()
    {
        if (controlledCamera == null)
        {
            Debug.LogWarning("CameraZoomController richiede un componente Camera.");
            return;
        }

        if (TryGetComponent(out UniversalAdditionalCameraData cameraData))
        {
            cameraData.renderPostProcessing = true;
            cameraData.requiresDepthTexture = true;
        }

        // Usa un volume dedicato su questa camera, invece di cercarne uno qualsiasi nella scena.
        globalVolume = GetComponent<Volume>();
        if (globalVolume == null)
        {
            globalVolume = gameObject.AddComponent<Volume>();
        }

        globalVolume.isGlobal = true;
        globalVolume.priority = 100f;

        runtimeProfile = ScriptableObject.CreateInstance<VolumeProfile>();
        depthOfField = runtimeProfile.Add<DepthOfField>(true);
        ConfigureDepthOfField(3f);

        globalVolume.profile = runtimeProfile;
        globalVolume.weight = 0f;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && isZoomed)
        {
            ResetCamera();
        }

        // Click sul vuoto per resettare - usa LateUpdate per dare priorità a OnMouseDown
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && isZoomed)
        {
            StartCoroutine(CheckClickOnEmpty());
        }
    }

    private IEnumerator CheckClickOnEmpty()
    {
        // Aspetta la fine del frame per permettere a OnMouseDown di essere processato prima
        yield return new WaitForEndOfFrame();

        if (!isZoomed) yield break; // Se non siamo più zoomati, esci

        // Controlla se abbiamo cliccato su un oggetto vuoto
        Ray ray = controlledCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        // Se NON colpiamo nessun oggetto con ClickableObject, resettiamo
        if (!Physics.Raycast(ray, out hit) || hit.collider.GetComponent<ClickableObject>() == null)
        {
            ResetCamera();
        }
    }

    public void ZoomToObject(ClickableObject obj, Vector3 targetWorldPos, float scale, float cameraDistance)
    {
        if (obj == null)
        {
            Debug.LogWarning("ClickableObject è null!");
            return;
        }
        _clickedObject = obj;
        _targetWorldPos = targetWorldPos;
        _scale = scale;
        _cameraDistance = cameraDistance;

        if (currentZoomCoroutine != null)
        {
            StopCoroutine(currentZoomCoroutine);
        }

        // Salva l'oggetto corrente
        currentObject = obj;
        currentTarget = obj.transform;

        if (enableBlur)
        {
            ConfigureDepthOfField(Mathf.Max(0.01f, cameraDistance));
        }

        // In 2D con sfondo fisso, NON muoviamo la camera
        // Avvia le animazioni solo per l'oggetto
        currentZoomCoroutine = StartCoroutine(ZoomObjectCoroutine(_clickedObject, targetWorldPos, scale));
    }

    public void ResetCamera()
    {
        if (currentZoomCoroutine != null)
        {
            StopCoroutine(currentZoomCoroutine);
        }

        if (currentObject != null)
        {
            currentZoomCoroutine = StartCoroutine(ResetObjectCoroutine(currentObject));
        }
        else
        {
            currentZoomCoroutine = StartCoroutine(ResetCameraOnlyCoroutine());
        }

        currentTarget = null;
        currentObject = null;
        isZoomed = false;
    }

    private IEnumerator ZoomObjectCoroutine(ClickableObject obj, Vector3 targetPos, float targetScale)
    {
        Transform objTransform = obj.transform;
        Vector3 startPos = objTransform.position;
        Vector3 startScale = objTransform.localScale;

        Vector3 endScale = obj.OriginalScale * targetScale;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;
            float curveValue = zoomCurve.Evaluate(t);

            // Muovi e scala l'oggetto (solo l'oggetto, non la camera)
            objTransform.position = Vector3.Lerp(startPos, targetPos, curveValue);
            objTransform.localScale = Vector3.Lerp(startScale, endScale, curveValue);

            if (enableBlur && globalVolume != null)
            {
                globalVolume.weight = curveValue;
            }

            yield return null;
        }

        objTransform.position = targetPos;
        objTransform.localScale = endScale;
        isZoomed = true;

        // Attiva il blur
        if (enableBlur && globalVolume != null)
        {
            globalVolume.weight = 1f;
        }

        currentZoomCoroutine = null;
    }

    private IEnumerator ResetObjectCoroutine(ClickableObject obj)
    {
        Transform objTransform = obj.transform;
        Vector3 startPos = objTransform.position;
        Vector3 startScale = objTransform.localScale;

        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;
            float curveValue = zoomCurve.Evaluate(t);

            // Riporta oggetto a posizione e scala originali (solo oggetto, non camera)
            objTransform.position = Vector3.Lerp(startPos, obj.OriginalPosition, curveValue);
            objTransform.localScale = Vector3.Lerp(startScale, obj.OriginalScale, curveValue);

            if (enableBlur && globalVolume != null)
            {
                globalVolume.weight = 1f - curveValue;
            }

            yield return null;
        }

        objTransform.position = obj.OriginalPosition;
        objTransform.localScale = obj.OriginalScale;

        // Disattiva il blur
        if (enableBlur && globalVolume != null)
        {
            globalVolume.weight = 0f;
        }

        currentZoomCoroutine = null;
    }

    private IEnumerator ResetCameraOnlyCoroutine()
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;
            float curveValue = zoomCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, originalPosition, curveValue);

            yield return null;
        }

        transform.position = originalPosition;

        // Disattiva il blur
        if (enableBlur && globalVolume != null)
        {
            globalVolume.weight = 0f;
        }

        currentZoomCoroutine = null;
    }

    private void ConfigureDepthOfField(float focusDistance)
    {
        if (depthOfField == null)
        {
            return;
        }

        float normalizedBlur = Mathf.Clamp01(blurIntensity / 10f);
        float blurStart = focusDistance + 0.35f;
        float blurRange = Mathf.Lerp(1.5f, 4f, normalizedBlur);

        depthOfField.active = true;

        depthOfField.mode.overrideState = true;
        depthOfField.mode.value = DepthOfFieldMode.Gaussian;

        depthOfField.gaussianStart.overrideState = true;
        depthOfField.gaussianStart.value = blurStart;

        depthOfField.gaussianEnd.overrideState = true;
        depthOfField.gaussianEnd.value = blurStart + blurRange;

        depthOfField.gaussianMaxRadius.overrideState = true;
        depthOfField.gaussianMaxRadius.value = Mathf.Lerp(0.2f, 1f, normalizedBlur);

        depthOfField.highQualitySampling.overrideState = true;
        depthOfField.highQualitySampling.value = normalizedBlur > 0.5f;

        depthOfField.focusDistance.overrideState = true;
        depthOfField.focusDistance.value = focusDistance;
    }

    public void OnBeforeSceneUnload()
    {
        StopCoroutine(ZoomObjectCoroutine(_clickedObject, _targetWorldPos, _scale));;
        _clickedObject = null;
        _targetWorldPos = Vector3.zero;
        _scale = 1f;
        _cameraDistance = 0f;
        ResetCamera();
    }
}
