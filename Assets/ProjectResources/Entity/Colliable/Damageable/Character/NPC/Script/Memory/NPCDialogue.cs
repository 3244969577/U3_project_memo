using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NPCDialogue : MonoBehaviour
{
    public NPCDialogueConfig config;
    private List<ChatMessage> _dialogueHistory = new List<ChatMessage>();
    
    private void Start()
    {
        _dialogueHistory.Add(new ChatMessage(){Role = "system", Content = config.systemPrompt});
    }
    
    public void SendPlayerMessage(string playerInput, System.Action<string> onReply)
    {
        SendPlayerMessage(playerInput, onReply, (replyLines) =>
        {
            Debug.Log($"NPC reply lines: {string.Join("\n", replyLines)}");
        });
    }
    
    public void SendPlayerMessage(string playerInput, System.Action<string> onReply, System.Action<List<string>> onReplyLines)
    {
        _dialogueHistory.Add(new ChatMessage(){Role = "user", Content = playerInput});
        
        TrimHistory();
        
        DoubaoApiManager.Instance.SendChatRequest(_dialogueHistory, 
            reply =>
            {
                _dialogueHistory.Add(new ChatMessage(){Role = "assistant", Content = reply});
                
                List<string> replyLines = ProcessReplyLines(reply);
                
                onReply?.Invoke(reply);
                onReplyLines?.Invoke(replyLines);
            },
            error =>
            {
                onReply?.Invoke(config.defaultReply);
                onReplyLines?.Invoke(new List<string>{config.defaultReply});
            }
        );
    }
    
    private List<string> ProcessReplyLines(string reply)
    {
        List<string> lines = new List<string>();
        
        if (string.IsNullOrEmpty(reply))
        {
            return lines;
        }
        
        string[] splitLines = reply.Split(new char[] {'\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string line in splitLines)
        {
            string trimmedLine = line.Trim();
            if (!string.IsNullOrEmpty(trimmedLine))
            {
                lines.Add(trimmedLine);
            }
        }
        
        return lines;
    }
    
    private void TrimHistory()
    {
        int maxTotalCount = 1 + config.maxHistoryRound * 2;
        while (_dialogueHistory.Count > maxTotalCount)
        {
            _dialogueHistory.RemoveRange(1, 2);
        }
    }
    
    private void SaveHistoryToLocal()
    {
        string json = JsonConvert.SerializeObject(_dialogueHistory);
        PlayerPrefs.SetString($"NPC_History_{config.npcId}", json);
    }
    
    private void LoadHistoryFromLocal()
    {
        if (PlayerPrefs.HasKey($"NPC_History_{config.npcId}"))
        {
            string json = PlayerPrefs.GetString($"NPC_History_{config.npcId}");
            _dialogueHistory = JsonConvert.DeserializeObject<List<ChatMessage>>(json);
        }
    }
    
    public void ClearHistory()
    {
        _dialogueHistory.Clear();
        _dialogueHistory.Add(new ChatMessage(){Role = "system", Content = config.systemPrompt});
        PlayerPrefs.DeleteKey($"NPC_History_{config.npcId}");
    }
}
