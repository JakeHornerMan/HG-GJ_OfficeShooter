using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MouseMovement : MonoBehaviour
{
    [Header("References")]
    // public Camera playerCamera;
    public GameObject playerCamera;
    public PlayerMovement playerMovement;

    [Header("Input Settings")]
    public float mouseSensitivity = 100f;
    public float topClamp = -60f;
    public float bottomClamp = 60f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private MouseSpeed currentSetting = MouseSpeed.Setting3;

    [Header("Visual Settings")]
    float maxTiltAngle = 0.3f; // Maximum tilt angle in degrees

    [Header("Camera Bounce Settings")]
    public float bounceDistance = 0.3f; // how much the camera moves down
    public float bounceDuration = 0.2f; // how fast the bounce happens
    public AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Dictionary<MouseSpeed, float> mouseValues = new Dictionary<MouseSpeed, float>()
    {
        { MouseSpeed.Setting1, 100f },
        { MouseSpeed.Setting2, 200f },
        { MouseSpeed.Setting3, 300f },
        { MouseSpeed.Setting4, 400f },
        { MouseSpeed.Setting5, 500f },
        { MouseSpeed.Setting6, 600f },
        { MouseSpeed.Setting7, 700f },
        { MouseSpeed.Setting8, 800f },
        { MouseSpeed.Setting9, 900f },
        { MouseSpeed.Setting10, 1000f }
    };

    void Start()
    {
        mouseSensitivity = mouseValues[currentSetting];
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartBounce();
        }
        HandleSensitivityChange();
    }

    #region Look Controls
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        yRotation += mouseX;

        float zRotation = HandleCameraTilt(playerMovement.moveX);
        
        this.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, zRotation);
    }

    private float HandleCameraTilt(float moveX)
    {
        if (moveX > 0.1f) // moving right
        {
            return -maxTiltAngle;
        }
        else if (moveX < -0.1f) // moving left
        {
            return maxTiltAngle;
        }
        return 0f;
    }
    #endregion

    private Coroutine bounceCoroutine; // store running coroutine

    public void StartBounce()
    {
        if (bounceCoroutine != null)
            StopCoroutine(bounceCoroutine);
            
        bounceCoroutine = StartCoroutine(CameraBounce(bounceDuration));
    }

   private IEnumerator CameraBounce(float duration)
    {
        Vector3 originalPosition = playerCamera.transform.localPosition;
        Vector3 downPosition = originalPosition + Vector3.down * bounceDistance;

        float elapsed = 0f;

        // Move down
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveT = bounceCurve.Evaluate(t); // evaluate curve
            playerCamera.transform.localPosition = Vector3.Lerp(originalPosition, downPosition, curveT);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerCamera.transform.localPosition = downPosition;

        // Move back up
        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveT = bounceCurve.Evaluate(t); // evaluate curve
            playerCamera.transform.localPosition = Vector3.Lerp(downPosition, originalPosition, curveT);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerCamera.transform.localPosition = originalPosition;

        bounceCoroutine = null; // mark as finished
    }



    #region Sensitivity Change
    public enum MouseSpeed
    {
        Setting1 = 0,
        Setting2 = 1,
        Setting3 = 2,
        Setting4 = 3,
        Setting5 = 4,
        Setting6 = 5,
        Setting7 = 6,
        Setting8 = 7,
        Setting9 = 8,
        Setting10 = 9
    }

    private void HandleSensitivityChange()
    {
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
        {
            CycleSetting(1);
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Underscore))
        {
            CycleSetting(-1);
        }
    }

    void CycleSetting(int direction)
    {
        int totalSettings = System.Enum.GetNames(typeof(MouseSpeed)).Length;
        int newIndex = ((int)currentSetting + direction + totalSettings) % totalSettings;

        currentSetting = (MouseSpeed)newIndex;
        mouseSensitivity = mouseValues[currentSetting];

        Debug.Log($"Mouse setting changed to {currentSetting}, sensitivity = {mouseSensitivity}");
    }
    #endregion
}
