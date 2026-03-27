# NPC对话系统实现逻辑与知识图谱

## 一、系统概述

当前NPC对话系统分为两个部分：
1. **传统对话系统**：基于预设对话内容的简单对话
2. **智能对话系统**：基于AIGC的动态对话生成

## 二、核心组件详解

### 2.1 传统对话系统

#### Dialog数据结构
```csharp
public class Dialog
{
    public List<string> Lines; // 预设的对话行
}
```

#### DialogManager（对话管理器）
**文件位置**：`Assets/Scripts/GameManager/DialogManager.cs`

**主要功能**：
- 单例模式管理对话显示
- `ShowDialog(Dialog dialog)`：显示对话内容
- `TypeDialog(string line)`：实现打字机效果
- 冻结游戏移动显示对话
- 管理对话进度和行数

**使用流程**：
1. 玩家按空格键触发对话
2. DialogManager显示对话框
3. 逐字显示对话内容
4. 显示完毕后隐藏对话框

#### NPC脚本
**文件位置**：`Assets/Scripts/Character/NPC/NPC.cs`

**主要功能**：
- 继承自Collidable，包含碰撞检测
- 实现简单移动AI（站立/移动状态）
- 检测玩家交互（Trigger碰撞）
- 按空格键触发对话
- 朝向玩家功能

**交互流程**：
```
玩家接近NPC → OnTriggerEnter2D → interacble = true
玩家按空格 → DialogManager.ShowDialog(dialog)
玩家离开 → OnTriggerExit2D → interacble = false
```

### 2.2 智能对话系统（AIGC）

#### NPCDialogueConfig（对话配置）
**文件位置**：`Assets/Scripts/AIGC/NPC/Memory/NPCDialogueConfig.cs`

**配置参数**：
```csharp
[Header("基础信息")]
public string npcName;        // NPC名字
public string npcId;          // 唯一ID，用于存储记忆

[Header("性格设定")]
public string systemPrompt;   // 系统提示词，核心设定

[Header("对话参数")]
public float temperature = 0.7f;           // 回复随机性
public int maxHistoryRound = 10;           // 最多保留多少轮对话
public string defaultReply = "我现在有点忙，等会儿再说吧。"; // 默认回复
```

#### NPCDialogue（对话管理）
**文件位置**：`Assets/Scripts/AIGC/NPC/Memory/NPCDialogue.cs`

**核心方法**：
```csharp
// 玩家发送消息的入口
public void SendPlayerMessage(string playerInput, Action<string> onReply)
{
    // 1. 把玩家的消息加入历史
    _dialogueHistory.Add(new ChatMessage(){Role = "user", Content = playerInput});
    
    // 2. 历史太长的话做裁剪
    TrimHistory();
    
    // 3. 调用API
    DoubaoApiManager.Instance.SendChatRequest(_dialogueHistory, 
        reply => {
            // 4. 把NPC的回复加入历史
            _dialogueHistory.Add(new ChatMessage(){Role = "assistant", Content = reply});
            onReply?.Invoke(reply);
        },
        error => {
            onReply?.Invoke(config.defaultReply);
        }
    );
}

// 裁剪历史对话
private void TrimHistory()
{
    int maxTotalCount = 1 + config.maxHistoryRound * 2;
    while (_dialogueHistory.Count > maxTotalCount)
    {
        _dialogueHistory.RemoveRange(1, 2); // 删除最早的一轮对话
    }
}
```

**记忆管理**：
- `_dialogueHistory`：对话历史列表
- `SaveHistoryToLocal()`：保存到本地（可选）
- `LoadHistoryFromLocal()`：从本地加载（可选）
- `ClearHistory()`：清除记忆

#### DoubaoApiManager（API管理器）
**文件位置**：`Assets/Scripts/AIGC/NPC/DoubaoApiManager.cs`

**主要功能**：
- 单例模式，全局调用
- 发送HTTP请求到豆包API
- 转换ChatMessage为ArkInputItem格式
- 处理API响应并回调结果

**请求流程**：
```
ChatMessage列表 → 转换为ArkInputItem → 序列化为JSON → 发送POST请求 → 解析响应 → 回调结果
```

#### 数据结构

**ChatMessage（对话消息）**
```csharp
public class ChatMessage
{
    public string Role;      // system/user/assistant
    public string Content;   // 消息内容
}
```

**ArkChatCompletionRequest（方舟请求体）**
```csharp
public class ArkChatCompletionRequest
{
    public string Model = "doubao-seed-2-0-pro-260215";
    public List<ArkInputItem> Messages;
}
```

**ArkInputItem（输入项）**
```csharp
public class ArkInputItem
{
    public string Role;
    public List<ArkContentItem> Content; // 支持多模态
}
```

## 三、知识图谱

### 3.1 实体关系图

```
NPC对话系统
├── 包含 → NPCDialogue
│   ├── 依赖 → NPCDialogueConfig
│   ├── 调用 → DoubaoApiManager
│   └── 管理 → ChatMessage
├── 包含 → NPCDialogueConfig
├── 包含 → DoubaoApiManager
│   ├── 转换 → ArkChatCompletionRequest
│   └── 发送请求 → ChatMessage
├── 使用 → ChatMessage
└── 使用 → ArkChatCompletionRequest

传统对话系统：
NPC
├── 使用传统对话 → Dialog
└── 调用 → DialogManager
    └── 显示 → Dialog
```

### 3.2 核心流程图

#### 传统对话流程
```
玩家接近NPC → 检测碰撞 → interacble = true
    ↓
玩家按空格 → DialogManager.ShowDialog(dialog)
    ↓
显示对话框 → 打字机效果 → 显示下一行
    ↓
对话结束 → 隐藏对话框 → 解冻游戏移动
```

#### 智能对话流程
```
玩家发送消息 → NPCDialogue.SendPlayerMessage()
    ↓
添加到对话历史 → 裁剪历史 → 调用API
    ↓
DoubaoApiManager.SendChatRequest()
    ↓
转换格式 → 发送HTTP请求 → 等待响应
    ↓
解析响应 → 回调NPC回复 → 添加到历史
    ↓
显示回复给玩家
```

## 四、如何实现Player与NPC的对话功能

### 4.1 方案一：扩展现有NPC类

**步骤1：修改NPC类，添加智能对话支持**
```csharp
public class NPC : Collidable
{
    // 添加智能对话组件
    private NPCDialogue npcDialogue;
    
    protected override void Start()
    {
        base.Start();
        npcDialogue = GetComponent<NPCDialogue>();
    }
    
    protected virtual void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (this.interacble)
            {
                // 检查是否有智能对话组件
                if (npcDialogue != null && npcDialogue.config != null)
                {
                    // 打开智能对话界面
                    OpenChatUI();
                }
                else
                {
                    // 使用传统对话
                    DialogManager.instance.ShowDialog(dialog);
                }
            }
        }
    }
    
    private void OpenChatUI()
    {
        // 显示聊天UI
        ChatUIManager.instance.Show(npcDialogue);
    }
}
```

**步骤2：创建ChatUIManager**
```csharp
public class ChatUIManager : MonoBehaviour
{
    public static ChatUIManager instance;
    
    public GameObject chatPanel;
    public InputField inputField;
    public Button sendButton;
    public Transform contentContainer;
    public GameObject messagePrefab;
    
    private NPCDialogue currentDialogue;
    
    public void Show(NPCDialogue dialogue)
    {
        currentDialogue = dialogue;
        chatPanel.SetActive(true);
        GameManager.instance.FreezeAllMovement();
    }
    
    public void Hide()
    {
        chatPanel.SetActive(false);
        currentDialogue = null;
        GameManager.instance.UnFreezeAllMovement();
    }
    
    public void OnSendButtonClicked()
    {
        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;
        
        // 显示玩家消息
        AddMessageToUI("你", message, true);
        inputField.text = "";
        
        // 发送到NPC
        currentDialogue.SendPlayerMessage(message, reply => 
        {
            // 显示NPC回复
            AddMessageToUI(currentDialogue.config.npcName, reply, false);
        });
    }
    
    private void AddMessageToUI(string sender, string message, bool isPlayer)
    {
        GameObject messageObj = Instantiate(messagePrefab, contentContainer);
        Text messageText = messageObj.GetComponentInChildren<Text>();
        messageText.text = $"<b>{sender}:</b> {message}";
        messageText.color = isPlayer ? Color.blue : Color.black;
    }
}
```

### 4.2 方案二：创建独立的对话系统

**步骤1：创建DialogueTrigger组件**
```csharp
public class DialogueTrigger : MonoBehaviour
{
    public NPCDialogueConfig dialogueConfig;
    private NPCDialogue npcDialogue;
    
    private void Start()
    {
        npcDialogue = gameObject.AddComponent<NPCDialogue>();
        npcDialogue.config = dialogueConfig;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 显示交互提示
            InteractionPrompt.instance.Show("按E键对话");
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            InteractionPrompt.instance.Hide();
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && /* 玩家在范围内 */)
        {
            ChatUIManager.instance.Show(npcDialogue);
        }
    }
}
```

**步骤2：创建对话UI预制体**
- 创建Canvas和Panel
- 添加InputField和Button
- 添加ScrollView显示对话历史
- 添加MessagePrefab显示单条消息

### 4.3 方案三：整合到现有ChatDemo系统

**修改ChatDemoManager以支持NPC对话**
```csharp
public class ChatDemoManager : MonoBehaviour
{
    // ... 现有代码 ...
    
    private NPCDialogue currentNPCDialogue;
    
    public void SetNPCDialogue(NPCDialogue dialogue)
    {
        currentNPCDialogue = dialogue;
        // 更新API URL和Key
        // 注意：这里应该使用DoubaoApiManager而不是直接设置
    }
    
    private void OnSendButtonClicked()
    {
        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;
        
        if (currentNPCDialogue != null)
        {
            // 使用NPC对话系统
            AddUserMessage(message);
            inputField.text = "";
            
            currentNPCDialogue.SendPlayerMessage(message, reply => 
            {
                AddAssistantMessage(reply);
            });
        }
        else
        {
            // 使用通用API
            // ... 现有代码 ...
        }
    }
}
```

## 五、实现建议

### 5.1 推荐方案
**推荐使用方案一**，原因：
1. 最小化修改现有代码
2. 保持传统对话和智能对话的兼容性
3. 易于扩展和维护
4. 符合现有架构设计

### 5.2 实现步骤

**第一步：准备UI组件**
1. 创建ChatUIManager脚本
2. 创建对话UI预制体
3. 配置UI组件引用

**第二步：修改NPC类**
1. 添加NPCDialogue组件引用
2. 添加智能对话检测逻辑
3. 添加打开聊天界面的方法

**第三步：配置NPC**
1. 为需要智能对话的NPC添加NPCDialogue组件
2. 创建并配置NPCDialogueConfig
3. 设置systemPrompt定义NPC性格

**第四步：测试和优化**
1. 测试对话功能
2. 优化UI显示
3. 添加错误处理
4. 优化性能

### 5.3 注意事项

1. **API密钥安全**：不要在客户端硬编码API密钥，使用云函数代理
2. **对话历史管理**：定期清理历史，避免token溢出
3. **错误处理**：提供默认回复，避免游戏卡死
4. **UI优化**：添加加载动画，提升用户体验
5. **性能优化**：使用对象池管理消息对象

## 六、文件清单

### 6.1 核心脚本
- `Assets/Scripts/AIGC/NPC/Memory/NPCDialogue.cs` - 智能对话管理
- `Assets/Scripts/AIGC/NPC/Memory/NPCDialogueConfig.cs` - 对话配置
- `Assets/Scripts/AIGC/NPC/DoubaoApiManager.cs` - API管理器
- `Assets/Scripts/AIGC/NPC/ApiCore.cs` - 数据结构定义
- `Assets/Scripts/Character/NPC/NPC.cs` - NPC基类
- `Assets/Scripts/GameManager/DialogManager.cs` - 传统对话管理器

### 6.2 需要创建的脚本
- `ChatUIManager.cs` - 聊天UI管理器
- `DialogueTrigger.cs` - 对话触发器（可选）
- `InteractionPrompt.cs` - 交互提示（可选）

### 6.3 UI资源
- ChatPanel预制体 - 对话面板
- MessagePrefab预制体 - 消息显示模板
- InputField组件 - 输入框
- Button组件 - 发送按钮

## 七、总结

当前NPC对话系统已经具备了完整的智能对话能力，包括：
- ✅ 对话历史管理
- ✅ API调用和响应处理
- ✅ NPC性格配置
- ✅ 记忆持久化（可选）

要实现Player与NPC的对话功能，需要：
1. 创建聊天UI界面
2. 修改NPC类以支持智能对话
3. 整合现有的智能对话组件
4. 配置NPC对话参数

通过以上步骤，可以实现一个完整的、基于AIGC的NPC对话系统，支持玩家与NPC进行自然语言交互。
