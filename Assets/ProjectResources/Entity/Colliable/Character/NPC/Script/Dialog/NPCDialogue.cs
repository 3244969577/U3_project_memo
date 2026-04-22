using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using GlobalEvents;
using NPCSocialEvents;
using NPCActionEvents;
using LLMAPI;
using NPCDialogAPI;
using System.Threading.Tasks;

public class NPCDialogue : MonoBehaviour
{
    public NPCDialogueConfig config;
    private List<LLMMessage> _dialogueHistory = new List<LLMMessage>();
    private EventBinding<PlayerInputEvent> _playerInputEventBinding;
    
    private void Awake()
    {
        _playerInputEventBinding = new EventBinding<PlayerInputEvent>(HandlePlayerInput);
    }
    private void OnEnable()
    {
        EventBus<PlayerInputEvent>.Register(_playerInputEventBinding);
    }
    private void OnDisable()
    {
        EventBus<PlayerInputEvent>.Deregister(_playerInputEventBinding);
    }

    private async void HandlePlayerInput(PlayerInputEvent e)
    {
        if (e.target != gameObject)
        {
            return;
        }
        string dialog = await SendPlayerMessage(e.input);
        List<string> replyLines = ProcessReplyLines(dialog);
        
        FungusDialogRenderer.Instance.RenderDialogLines(
            replyLines,
            gameObject.GetComponentInChildren<Fungus.Character>(),
            gameObject.GetComponent<SpriteRenderer>().sprite
        );
        //     (reply) => {
        //     },
        //     (replyLines) => {
        //         Debug.Log($"NPC reply lines: {string.Join("\n", replyLines)}");
                
        //         // 使用对话渲染
        //         FungusDialogRenderer.Instance.RenderDialogLines(
        //             replyLines,
        //             gameObject.GetComponentInChildren<Fungus.Character>(),
        //             gameObject.GetComponent<SpriteRenderer>().sprite
        //         );
        //     }
        // );
    }

    private void Start()
    {
        _dialogueHistory.Add(new LLMMessage(){Role = "system", Content = config.systemPrompt});
    }
    
    // public void SendPlayerMessage(string playerInput, System.Action<string> onReply)
    // {
    //     SendPlayerMessage(playerInput, onReply, (replyLines) =>
    //     {
    //         Debug.Log($"NPC reply lines: {string.Join("\n", replyLines)}");
    //     });
    // }
    
    public async Task<string> SendPlayerMessage(string playerInput)
    {
        _dialogueHistory.Add(new LLMMessage(){Role = "user", Content = playerInput});
        TrimHistory();

        LLMMessage socialRelationMsg = new LLMMessage(){Role = "system", Content = getSocialRelationPrompt()};
        _dialogueHistory.Insert(1, socialRelationMsg);

        string reply = await DoubaoApiManager.Instance.SendChatRequestAsync(_dialogueHistory);
        Debug.Log($"原始响应: {reply}");

        NPCLLMRsp rsp = JsonConvert.DeserializeObject<NPCLLMRsp>(reply);
        _dialogueHistory.Add(new LLMMessage(){Role = "assistant", Content = rsp.dialogue});

        // 处理NPC行为
        EventBus<NPCRspActionEvent>.Raise(
            new NPCRspActionEvent(){npc = gameObject, action = rsp.action}
        );
        // 处理NPC表情
        GetComponent<NPC_Genearted>().NPC_EventBus.Raise<NPCEmotionEvent>(
            new NPCEmotionEvent() {
                npc = gameObject, 
                emotionName = rsp.emotion
            }
        );

        EventBus<NPCSocialRelationChangeEvent>.Raise(
            new NPCSocialRelationChangeEvent() {
                npc = gameObject.name,
                target = rsp.social_alter.name,
                delta = rsp.social_alter.relation
            }
        );

        // List<string> replyLines = ProcessReplyLines(rsp.dialogue);
        // onReply?.Invoke(rsp.dialogue);
        // onReplyLines?.Invoke(replyLines);

        _dialogueHistory.RemoveAt(1);
        return rsp.dialogue;
    }

    public string getSocialRelationPrompt() {
        Dictionary<string, int> socialRelation = NPCSocialManager.Instance.GetSocialRelationOf(gameObject.name);
        return JsonConvert.SerializeObject(socialRelation);
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
            _dialogueHistory = JsonConvert.DeserializeObject<List<LLMMessage>>(json);
        }
    }
    
    public void ClearHistory()
    {
        _dialogueHistory.Clear();
        _dialogueHistory.Add(new LLMMessage(){Role = "system", Content = config.systemPrompt});
        PlayerPrefs.DeleteKey($"NPC_History_{config.npcId}");
    }
}

