using UnityEngine;
using Fungus;
using System.Collections.Generic;
using System.Collections;

public class FungusDialogRenderer : MonoBehaviour
{
    private static FungusDialogRenderer instance;
    public static FungusDialogRenderer Instance => instance;
    
    private Flowchart targetFlowchart;
    private int currentLineIndex = 0;
    private List<string> currentLines;
    private System.Action currentOnComplete;
    private Fungus.Character currentCharacter;
    private Sprite currentPortrait;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    private void Start()
    {
        SetFlowchart(GameManager.Instance.flowchart);
    }
    
    public void SetFlowchart(Flowchart flowchart)
    {
        targetFlowchart = flowchart;
    }
    
    public void SetCharacter(Fungus.Character character)
    {
        currentCharacter = character;
    }
    
    public void SetPortrait(Sprite portrait)
    {
        currentPortrait = portrait;
    }
    
    public void RenderDialogLines(List<string> lines, System.Action onComplete = null)
    {
        if (lines == null || lines.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }
        
        currentLines = lines;
        currentLineIndex = 0;
        currentOnComplete = onComplete;
        
        ShowNextLine();
    }
    
    public void RenderDialogLines(List<string> lines, Fungus.Character character, Sprite portrait = null, System.Action onComplete = null)
    {
        currentCharacter = character;
        currentPortrait = portrait;
        RenderDialogLines(lines, onComplete);
    }
    
    private void ShowNextLine()
    {
        if (currentLineIndex >= currentLines.Count)
        {
            currentOnComplete?.Invoke();
            return;
        }
        
        string line = currentLines[currentLineIndex];
        currentLineIndex++;
        
        Debug.Log($"Showing line: {line}");
        
        SayDialog sayDialog = SayDialog.GetSayDialog();
        if (sayDialog == null)
        {
            Debug.LogError("SayDialog not found!");
            ShowNextLine();
            return;
        }
        
        sayDialog.SetActive(true);
        sayDialog.SetCharacter(currentCharacter);
        sayDialog.SetCharacterImage(currentPortrait);
        
        sayDialog.Say(
            line,
            true,
            true,
            true,
            true,
            false,
            null,
            () => 
            {
                ShowNextLine();
            }
        );
    }
    
    public void ShowSingleLine(string line, System.Action onComplete = null)
    {
        RenderDialogLines(new List<string> { line }, onComplete);
    }
    
    public void ShowSingleLine(string line, Fungus.Character character, Sprite portrait = null, System.Action onComplete = null)
    {
        RenderDialogLines(new List<string> { line }, character, portrait, onComplete);
    }
}
