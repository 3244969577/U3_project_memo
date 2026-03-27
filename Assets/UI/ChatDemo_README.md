# 聊天Demo系统使用指南

## 系统概述
这是一个用于测试API对话功能的简单Demo界面，使用静态UI搭建，支持发送消息和显示聊天记录。

## 系统结构

### 1. UI层级结构
```
ChatDemoCanvas (Canvas - UI画布)
├── ChatPanel (聊天面板)
│   ├── ChatDisplay (聊天记录显示区域)
│   │   └── Content (消息容器)
│   └── InputArea (输入区域)
│       ├── InputField (输入框)
│       │   ├── Text (输入文本)
│       │   └── Placeholder (占位符)
│       └── SendButton (发送按钮)
│           └── Text (按钮文本)
EventSystem (事件系统)
MessagePrefab (消息预制体)
```

### 2. 核心组件

#### ChatDemoCanvas
- **Canvas**: UI画布，渲染模式为Screen Space Overlay
- **Canvas Scaler**: UI缩放器，确保UI在不同分辨率下正常显示
- **Graphic Raycaster**: UI射线检测器，处理UI交互

#### ChatPanel
- **RectTransform**: UI布局组件
- **Image**: 背景图像组件

#### ChatDisplay
- **ScrollRect**: 滚动视图组件，支持消息滚动查看
- **Image**: 背景图像组件
- **Content**: 消息容器，包含VerticalLayoutGroup和ContentSizeFitter

#### InputArea
- **HorizontalLayoutGroup**: 水平布局组件，用于排列输入框和按钮

#### InputField
- **InputField**: 输入框组件，用于用户输入消息
- **Text**: 显示输入的文本
- **Placeholder**: 显示占位符文本

#### SendButton
- **Button**: 按钮组件，点击发送消息
- **Image**: 按钮背景图像
- **Text**: 按钮文本显示"发送"

#### EventSystem
- **EventSystem**: 事件系统，处理UI交互事件
- **StandaloneInputModule**: 标准输入模块

#### MessagePrefab
- **Text**: 消息文本组件，用于显示单条消息

### 3. 脚本组件

#### ChatDemoManager.cs
聊天管理器脚本，负责：
- 处理用户输入和发送消息
- 调用API接口获取回应
- 显示聊天记录
- 管理对话历史

**主要方法**：
- `OnSendButtonClicked()`: 发送按钮点击事件
- `SendMessageToAPI()`: 发送消息到API
- `ProcessAPIResponse()`: 处理API响应
- `AddMessageToUI()`: 添加消息到UI界面

## 使用方式

### 1. 配置API接口
在ChatDemoCanvas的ChatDemoManager组件中设置：
- **API URL**: API接口地址
- **API Key**: API密钥（如果需要）

### 2. 绑定UI组件
将以下UI组件绑定到ChatDemoManager：
- **InputField**: 输入框组件
- **SendButton**: 发送按钮组件
- **ContentContainer**: 消息容器（Content对象）
- **MessagePrefab**: 消息预制体

### 3. 运行测试
1. 运行游戏场景
2. 在输入框中输入消息
3. 点击"发送"按钮或按回车键发送消息
4. 等待API响应并显示在聊天记录中

## API集成说明

### 支持的API格式
脚本支持标准的聊天API格式，请求和响应格式如下：

**请求格式**：
```json
{
  "messages": [
    {
      "role": "user",
      "content": "用户消息内容"
    }
  ]
}
```

**响应格式**：
```json
{
  "choices": [
    {
      "message": {
        "role": "assistant",
        "content": "AI回应内容"
      }
    }
  ]
}
```

### 自定义API适配
如果API格式不同，需要修改以下方法：
- `SendMessageToAPI()`: 修改请求格式
- `ProcessAPIResponse()`: 修改响应解析逻辑

## 扩展功能建议

### 1. 添加消息样式
- 为不同角色的消息设置不同颜色
- 添加消息气泡背景
- 支持富文本格式

### 2. 添加更多功能
- 支持图片消息
- 添加消息时间戳
- 支持消息复制和删除
- 添加对话历史保存和加载

### 3. 优化用户体验
- 添加加载动画
- 支持消息重试
- 添加网络状态检测
- 支持离线模式

## 常见问题

### Q: 消息不显示
A: 检查MessagePrefab和ContentContainer是否正确绑定

### Q: 无法输入文本
A: 确保EventSystem存在且InputField组件配置正确

### Q: API调用失败
A: 检查API URL和API Key是否正确，查看控制台错误信息

### Q: 按钮点击无响应
A: 检查Button组件是否正确添加点击事件监听器

## 技术细节

### UI布局
- 使用RectTransform进行UI布局
- 使用LayoutGroup自动排列子元素
- 使用ContentSizeFitter自动调整容器大小

### 消息显示
- 使用Instantiate动态创建消息对象
- 使用Text组件显示消息内容
- 支持富文本格式（加粗、颜色等）

### API通信
- 使用UnityWebRequest发送HTTP请求
- 使用JSON格式进行数据交换
- 使用协程处理异步请求

## 文件清单

### 脚本文件
- `Assets/Scripts/UI/ChatDemoManager.cs`: 聊天管理器脚本

### 场景文件
- `Assets/Scenes/DialogueScene.unity`: Demo场景文件

### UI组件
- ChatDemoCanvas: UI画布
- ChatPanel: 聊天面板
- ChatDisplay: 聊天记录显示区域
- InputArea: 输入区域
- InputField: 输入框
- SendButton: 发送按钮
- EventSystem: 事件系统
- MessagePrefab: 消息预制体

## 后续开发建议

1. **完善UI样式**: 添加更多视觉效果，如阴影、圆角等
2. **增加配置选项**: 支持在Inspector中配置更多参数
3. **添加错误处理**: 更完善的错误提示和重试机制
4. **性能优化**: 使用对象池管理消息对象
5. **多语言支持**: 支持多种语言界面
