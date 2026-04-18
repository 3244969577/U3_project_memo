using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    
    [Header("UI Components")]
    public GameObject inputPanel;
    public InputField inputField;
    public Button submitButton;
    
    [Header("Settings")]
    public int maxHistoryCount = 50;
    
    public event Action<string> OnInputSubmitted;
    
    private List<string> inputHistory = new List<string>();
    private bool isInputActive = false;
    
    public bool IsInputActive => isInputActive;
    
    private void Awake()
    {
        base.Awake();
    }
    
    private void Start()
    {
        Debug.Assert(inputPanel != null, "InputPanel is null");
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SubmitInput);
        }
        
        Debug.Assert(inputField != null, "InputField is null");
        if (inputField != null)
        {
            inputField.lineType = InputField.LineType.MultiLineNewline;
        }
        
        HideInput();
    }
    
    private void Update() 
    {
        
        // 按下 ESC 键退出输入面板
        if (Input.GetKeyDown(KeyCode.Escape) && isInputActive)
        {
            HideInput();
        }
    }
    
    public void ShowInput()
    {
        if (inputPanel != null)
        {
            inputPanel.SetActive(true);
            isInputActive = true;
            
            if (inputField != null)
            {
                inputField.ActivateInputField();
            }
            
            GameManager.Instance.FreezeAllMovement();
        }
    }
    
    public void HideInput()
    {
        if (inputPanel != null)
        {
            inputPanel.SetActive(false);
            isInputActive = false;
            
            if (inputField != null)
            {
                inputField.text = "";
            }
            
            GameManager.Instance.UnFreezeAllMovement();
        }
    }
    
    public void ToggleInput()
    {
        if (isInputActive)
        {
            HideInput();
        }
        else
        {
            ShowInput();
        }
    }
    
    public void SubmitInput()
    {
        if (inputField == null || string.IsNullOrEmpty(inputField.text.Trim()))
        {
            return;
        }
        
        string input = inputField.text.Trim();
        
        AddToHistory(input);
        
        Debug.Log($"Player input: {input}");
        OnInputSubmitted?.Invoke(input);
        var npcInteracting = GameManager.Instance.GetNPCInteracting();
        npcInteracting?.GetComponent<NPCDialogue>()?.SendPlayerMessage(input, 
            (reply) => {
                Debug.Log($"NPC reply: {reply}");
            }, 
            (replyLines) => {
                FungusDialogRenderer.Instance.RenderDialogLines(replyLines, 
                    npcInteracting.GetComponentInChildren<Fungus.Character>(), 
                    npcInteracting.GetComponent<SpriteRenderer>().sprite
                );
            });

        
        HideInput();
    }
    
    private void AddToHistory(string input)
    {
        inputHistory.Add(input);
        
        if (inputHistory.Count > maxHistoryCount)
        {
            inputHistory.RemoveAt(0);
        }
    }
    
    public List<string> GetHistory()
    {
        return new List<string>(inputHistory);
    }
    
    public string GetLastInput()
    {
        return inputHistory.Count > 0 ? inputHistory[inputHistory.Count - 1] : "";
    }
    
    public void ClearHistory()
    {
        inputHistory.Clear();
    }
}
