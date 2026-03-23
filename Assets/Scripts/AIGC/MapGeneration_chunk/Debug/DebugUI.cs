using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Debug UI system, inspired by Minecraft style
/// </summary>
public class DebugUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text debugText; // Text component for debug information
    public GameObject debugUICanvas; // Debug UI canvas game object
    
    // Debug information dictionary
    private Dictionary<string, string> _debugInfo = new Dictionary<string, string>();
    
    // Singleton instance
    public static DebugUI Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Initialize UI
        InitializeUI();
    }
    
    /// <summary>
    /// Initialize UI
    /// </summary>
    private void InitializeUI()
    {
        if (debugText != null)
        {
            Debug.Log("DebugUI initialized successfully, TextMeshProUGUI component assigned");
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component not assigned in inspector");
        }
        
        if (debugUICanvas != null)
        {
            Debug.Log("DebugUI initialized successfully, DebugUICanvas assigned");
        }
        else
        {
            Debug.LogWarning("DebugUICanvas not assigned in inspector");
        }
    }
    
    /// <summary>
    /// Add debug information
    /// </summary>
    /// <param name="key">Info key</param>
    /// <param name="value">Info value</param>
    public void AddDebugInfo(string key, string value)
    {
        // Update existing key or add new key-value pair
        _debugInfo[key] = value;
        UpdateDebugText();
    }
    
    /// <summary>
    /// Remove debug information
    /// </summary>
    /// <param name="key">Info key</param>
    public void RemoveDebugInfo(string key)
    {
        if (_debugInfo.ContainsKey(key))
        {
            _debugInfo.Remove(key);
            UpdateDebugText();
        }
    }
    
    /// <summary>
    /// Clear all debug information
    /// </summary>
    public void ClearDebugInfo()
    {
        _debugInfo.Clear();
        UpdateDebugText();
    }
    
    /// <summary>
    /// Update debug text
    /// </summary>
    private void UpdateDebugText()
    {
        // Build text from dictionary
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var kvp in _debugInfo)
        {
            sb.AppendLine($"{kvp.Key}: {kvp.Value}");
        }
        
        if (debugText != null)
        {
            debugText.text = sb.ToString();
        }
    }
    
    /// <summary>
    /// Show or hide debug UI
    /// </summary>
    /// <param name="show">Whether to show</param>
    public void ShowDebugUI(bool show)
    {
        if (debugUICanvas != null)
        {
            debugUICanvas.SetActive(show);
        }
    }
    
    private void Update()
    {
        // Toggle debug UI display with F1 key
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (debugUICanvas != null)
            {
                bool isActive = debugUICanvas.activeSelf;
                debugUICanvas.SetActive(!isActive);
                Debug.Log($"Debug UI {(isActive ? "disabled" : "enabled")}");
            }
        }
    }
}