using System.Collections;
using UnityEngine;

public class EnhancedCameraController : MonoBehaviour
{
    [Header("Look Settings")]
    [Range(0.1f, 10f)] public float sensitivity = 2.0f;
    public bool invertY = false;

    [Header("Smoothing")]
    public bool enableSmoothing = true;
    [Range(1f, 20f)] public float smoothSpeed = 10f;

    [Header("View Limits")]
    public bool enableVerticalLimits = true;
    [Range(0f, 90f)] public float upperLookLimit = 80.0f;
    [Range(0f, 90f)] public float lowerLookLimit = 80.0f;

    [Header("Field of View")]
    public bool enableFOVChanges = true;
    [Range(30f, 120f)] public float defaultFOV = 60f;
    [Range(10f, 60f)] public float zoomFOV = 30f;
    [Range(1f, 20f)] public float fovChangeSpeed = 10f;
    public KeyCode zoomKey = KeyCode.Mouse1;

    [Header("Head Bob")]
    public bool enableHeadBob = true;
    [Range(0f, 0.1f)] public float headBobAmount = 0.05f;
    [Range(1f, 30f)] public float headBobSpeed = 10f;

    [Header("Cursor")]
    public bool showCursor = true;
    public KeyCode toggleCursorKey = KeyCode.C;

    private float rotationX = 0;
    private float rotationY = 0;
    private float targetRotationX = 0;
    private float targetRotationY = 0;
    private float currentFOV;
    private bool isZooming = false;
    private Vector3 initialPosition;
    private float bobTimer = 0f;
    private Camera cameraComponent;

    void Start()
    {
        cameraComponent = GetComponent<Camera>();
        if (cameraComponent == null)
        {
            Debug.LogError("Camera component not found!");
            enabled = false;
            return;
        }

        initialPosition = transform.localPosition;
        currentFOV = defaultFOV;
        cameraComponent.fieldOfView = currentFOV;

        Vector3 rotation = transform.eulerAngles;
        rotationX = rotation.x;
        rotationY = rotation.y;
        targetRotationX = rotationX;
        targetRotationY = rotationY;

        SetCursorState(showCursor);
    }

    void Update()
    {
        HandleMouseLook();

        if (enableFOVChanges)
            HandleZoom();

        if (enableHeadBob)
            HandleHeadBob();

        if (Input.GetKeyDown(toggleCursorKey))
        {
            showCursor = !showCursor;
            SetCursorState(showCursor);
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        if (invertY)
            mouseY = -mouseY;

        targetRotationY += mouseX;
        targetRotationX -= mouseY;

        if (enableVerticalLimits)
            targetRotationX = Mathf.Clamp(targetRotationX, -lowerLookLimit, upperLookLimit);

        if (enableSmoothing)
        {
            rotationX = Mathf.Lerp(rotationX, targetRotationX, Time.deltaTime * smoothSpeed);
            rotationY = Mathf.Lerp(rotationY, targetRotationY, Time.deltaTime * smoothSpeed);
        }
        else
        {
            rotationX = targetRotationX;
            rotationY = targetRotationY;
        }

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }

    void HandleZoom()
    {
        if (Input.GetKeyDown(zoomKey))
            isZooming = true;
        else if (Input.GetKeyUp(zoomKey))
            isZooming = false;

        float targetFOV = isZooming ? zoomFOV : defaultFOV;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovChangeSpeed);
        cameraComponent.fieldOfView = currentFOV;
    }

    void HandleHeadBob()
    {
        if (IsMoving())
        {
            bobTimer += Time.deltaTime * headBobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * headBobAmount;
            Vector3 newPosition = initialPosition;
            newPosition.y += bobOffset;
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * 5f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 5f);
        }
    }

    bool IsMoving()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        return Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
    }

    public void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public IEnumerator LookAt(Vector3 worldPosition, float duration)
    {
        Vector3 direction = worldPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float targetX = targetRotation.eulerAngles.x;
        float targetY = targetRotation.eulerAngles.y;

        if (targetX > 180) targetX -= 360;
        if (enableVerticalLimits)
            targetX = Mathf.Clamp(targetX, -lowerLookLimit, upperLookLimit);

        float startTime = Time.time;
        float endTime = startTime + duration;

        float originalX = rotationX;
        float originalY = rotationY;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            rotationX = Mathf.Lerp(originalX, targetX, t);
            rotationY = Mathf.Lerp(originalY, targetY, t);
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
            yield return null;
        }

        rotationX = targetX;
        rotationY = targetY;
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);

        targetRotationX = rotationX;
        targetRotationY = rotationY;
    }
}
