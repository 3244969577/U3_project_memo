using UnityEngine;

public class CameraRescale : MonoBehaviour
{
    [Header("Camera Settings")]
    public float transitionSpeed = 5f; // 视野过渡速度
    public float zoomMultiplier = 2f; // 放大倍数

    private Camera mainCamera; // 主摄像机
    private float originalOrthographicSize; // 原始视野大小
    private float targetOrthographicSize; // 目标视野大小

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 获取主摄像机组件
        mainCamera = Camera.main;
        if (mainCamera == null) {
            Debug.LogError("Main camera not found!");
            return;
        }

        // 记录原始视野大小
        originalOrthographicSize = mainCamera.orthographicSize;
        targetOrthographicSize = originalOrthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        // 检测Tab键的按下和释放
        if (Input.GetKeyDown(KeyCode.Tab)) {
            // 按下Tab键，视野扩大为2倍
            targetOrthographicSize = originalOrthographicSize * zoomMultiplier;
        }
        else if (Input.GetKeyUp(KeyCode.Tab)) {
            // 释放Tab键，视野还原
            targetOrthographicSize = originalOrthographicSize;
        }

        // 平滑过渡到目标视野大小
        if (mainCamera != null) {
            mainCamera.orthographicSize = Mathf.Lerp(
                mainCamera.orthographicSize,
                targetOrthographicSize,
                transitionSpeed * Time.deltaTime
            );
        }
    }
}
