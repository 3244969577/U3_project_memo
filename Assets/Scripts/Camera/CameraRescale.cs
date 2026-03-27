using UnityEngine;
using Cainos.PixelArtTopDown_Basic;

public class CameraRescale : MonoBehaviour
{
    [Header("Camera Settings")]
    public float transitionSpeed = 5f;
    public float zoomMultiplier = 2f;

    private Camera mainCamera;
    private CameraFollow cameraFollow;
    private float originalOrthographicSize;
    private float targetOrthographicSize;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            Debug.LogError("CameraFollow script not found on main camera!");
            return;
        }

        originalOrthographicSize = mainCamera.orthographicSize;
        targetOrthographicSize = originalOrthographicSize;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            targetOrthographicSize = originalOrthographicSize * zoomMultiplier;
            if (cameraFollow != null && GameManager.instance.crosshair != null)
            {
                cameraFollow.SetTarget(GameManager.instance.crosshair.transform);
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            targetOrthographicSize = originalOrthographicSize;
            if (cameraFollow != null && GameManager.instance.player != null)
            {
                cameraFollow.SetTarget(GameManager.instance.player.transform);
            }
        }

        if (mainCamera != null)
        {
            mainCamera.orthographicSize = Mathf.Lerp(
                mainCamera.orthographicSize,
                targetOrthographicSize,
                transitionSpeed * Time.deltaTime
            );
        }
    }
}
