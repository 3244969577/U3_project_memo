# Fungus API 知识图谱

## 核心类

### Flowchart
**命名空间**: `Fungus`

#### 主要方法

##### CreateBlock
```csharp
public virtual Block CreateBlock(Vector2 position)
```
**参数**:
- `position`: Vector2 - Block在编辑器中的位置坐标

**返回值**: Block - 新创建的Block对象

**用法示例**:
```csharp
Block block = flowchart.CreateBlock(Vector2.zero);
block.BlockName = "MyBlock";
```

**错误用法**:
```csharp
// ❌ 错误：CreateBlock不接受string参数
Block block = flowchart.CreateBlock("BlockName");

// ✅ 正确：使用Vector2参数
Block block = flowchart.CreateBlock(Vector2.zero);
block.BlockName = "BlockName";
```

##### SetStartBlock
```csharp
public virtual void SetStartBlock(Block block)
```
**参数**:
- `block`: Block - 设置为启动块的Block对象

**用法示例**:
```csharp
flowchart.SetStartBlock(myBlock);
```

##### ExecuteBlock
```csharp
public virtual bool ExecuteBlock(Block block, int commandIndex = 0, Action onComplete = null)
public virtual void ExecuteBlock(string blockName)
```
**参数**:
- `block`: Block - 要执行的Block对象
- `commandIndex`: int - 开始执行的命令索引（默认0）
- `onComplete`: Action - 执行完成后的回调
- `blockName`: string - 要执行的Block名称

**用法示例**:
```csharp
// 通过Block对象执行
flowchart.ExecuteBlock(myBlock);

// 通过Block名称执行
flowchart.ExecuteBlock("MyBlock");
```

##### FindBlock
```csharp
public virtual Block FindBlock(string blockName)
```
**参数**:
- `blockName`: string - Block名称

**返回值**: Block - 找到的Block对象，未找到返回null

**用法示例**:
```csharp
Block block = flowchart.FindBlock("MyBlock");
if (block != null)
{
    // 找到了Block
}
```

##### GetComponents<Block>
```csharp
var blocks = flowchart.GetComponents<Block>();
```
**用法**: 获取Flowchart中的所有Block组件

**示例**:
```csharp
var blocks = flowchart.GetComponents<Block>();
for (int i = 0; i < blocks.Length; i++)
{
    // 处理每个Block
}
```

##### NextItemId
```csharp
public virtual int NextItemId()
```
**返回值**: int - 下一个可用的Item ID

**用法示例**:
```csharp
int itemId = flowchart.NextItemId();
```

---

### Block
**命名空间**: `Fungus`

#### 主要属性

##### BlockName
```csharp
public string BlockName { get; set; }
```
**类型**: string

**用法示例**:
```csharp
block.BlockName = "MyBlock";
```

##### ItemId
```csharp
public int ItemId { get; set; }
```
**类型**: int

**用法示例**:
```csharp
block.ItemId = flowchart.NextItemId();
```

##### CommandList
```csharp
public List<Command> CommandList { get; }
```
**类型**: List<Command>

**用法示例**:
```csharp
block.CommandList.Add(newCommand);
```

#### 主要方法

##### GetFlowchart
```csharp
public Flowchart GetFlowchart()
```
**返回值**: Flowchart - Block所属的Flowchart

**用法示例**:
```csharp
Flowchart flowchart = block.GetFlowchart();
```

##### IsExecuting
```csharp
public bool IsExecuting()
```
**返回值**: bool - Block是否正在执行

**用法示例**:
```csharp
if (block.IsExecuting())
{
    // Block正在执行
}
```

---

### Command
**命名空间**: `Fungus`

**基类**: 所有Fungus命令的基类

#### 主要属性

##### ParentBlock
```csharp
public Block ParentBlock { get; set; }
```
**类型**: Block

**用法示例**:
```csharp
command.ParentBlock = block;
```

##### CommandIndex
```csharp
public int CommandIndex { get; set; }
```
**类型**: int

**用法示例**:
```csharp
command.CommandIndex = index;
```

##### ItemId
```csharp
public int ItemId { get; set; }
```
**类型**: int

**用法示例**:
```csharp
command.ItemId = flowchart.NextItemId();
```

---

### Say
**命名空间**: `Fungus`

**继承**: Command, ILocalizable

**用途**: 显示对话文本

#### 主要方法

##### SetStandardText
```csharp
public virtual void SetStandardText(string standardText)
```
**参数**:
- `standardText`: string - 要显示的对话文本

**用法示例**:
```csharp
Say say = AddCommandToBlock<Say>(block);
say.SetStandardText("Hello, World!");
```

#### 主要属性（通过反射设置）

##### character
```csharp
[SerializeField] protected Character character;
```
**类型**: Character

**设置方法**:
```csharp
var characterField = typeof(Say).GetField("character", BindingFlags.NonPublic | BindingFlags.Instance);
characterField.SetValue(say, myCharacter);
```

---

### Menu
**命名空间**: `Fungus`

**继承**: Command, ILocalizable, IBlockCaller

**用途**: 显示选项菜单

#### 主要方法

##### SetStandardText
```csharp
public virtual void SetStandardText(string standardText)
```
**参数**:
- `standardText`: string - 选项按钮文本

**用法示例**:
```csharp
Fungus.Menu menu = AddCommandToBlock<Fungus.Menu>(block);
menu.SetStandardText("Option 1");
```

#### 主要属性（通过反射设置）

##### targetBlock
```csharp
[SerializeField] protected Block targetBlock;
```
**类型**: Block

**设置方法**:
```csharp
var targetBlockField = typeof(Fungus.Menu).GetField("targetBlock", BindingFlags.NonPublic | BindingFlags.Instance);
targetBlockField.SetValue(menu, targetBlock);
```

---

### Wait
**命名空间**: `Fungus`

**继承**: Command

**用途**: 等待指定时间

#### 主要属性（通过反射设置）

##### _duration
```csharp
[SerializeField] protected FloatData _duration;
```
**类型**: FloatData

**设置方法**:
```csharp
var durationField = typeof(Wait).GetField("_duration", BindingFlags.NonPublic | BindingFlags.Instance);
var floatData = new FloatData(1.0f);
durationField.SetValue(wait, floatData);
```

---

### CallMethod
**命名空间**: `Fungus`

**继承**: Command

**用途**: 调用指定对象的方法

#### 主要属性（通过反射设置）

##### targetObject
```csharp
[SerializeField] protected GameObject targetObject;
```
**类型**: GameObject

##### methodName
```csharp
[SerializeField] protected string methodName;
```
**类型**: string

---

## 辅助类

### FloatData
**命名空间**: `Fungus`

**用途**: 可序列化的浮点数数据

#### 构造函数
```csharp
public FloatData(float value)
```

**用法示例**:
```csharp
FloatData floatData = new FloatData(1.0f);
```

---

## 命名空间冲突

### Menu类冲突
**问题**: 'Menu' is an ambiguous reference between 'Fungus.Menu' and 'UnityEditor.Menu'

**解决方案**: 明确指定命名空间
```csharp
// ❌ 错误：命名冲突
Menu menu = AddCommandToBlock<Menu>(block);

// ✅ 正确：明确指定Fungus命名空间
Fungus.Menu menu = AddCommandToBlock<Fungus.Menu>(block);
```

---

## 常用模式

### 添加命令到Block
```csharp
private static T AddCommandToBlock<T>(Block block) where T : Command
{
    var flowchart = block.GetFlowchart() as Flowchart;
    var newCommand = Undo.AddComponent(block.gameObject, typeof(T)) as T;
    newCommand.ParentBlock = block;
    newCommand.ItemId = flowchart.NextItemId();
    block.CommandList.Add(newCommand);
    return newCommand;
}
```

### 清除所有Blocks
```csharp
var existingBlocks = flowchart.GetComponents<Block>();
for (int i = existingBlocks.Length - 1; i >= 0; i--)
{
    Object.DestroyImmediate(existingBlocks[i]);
}
```

### 创建Block并设置属性
```csharp
Block block = flowchart.CreateBlock(Vector2.zero);
block.BlockName = "MyBlock";
block.ItemId = flowchart.NextItemId();
```

---

## 注意事项

1. **CreateBlock参数**: CreateBlock方法需要Vector2参数，不是string
2. **命名空间冲突**: 使用Menu类时需要明确指定Fungus.Menu
3. **反射设置属性**: 许多Fungus属性是private的，需要通过反射设置
4. **ItemId管理**: 创建Block和Command时需要设置ItemId
5. **ParentBlock**: 添加Command到Block时需要设置ParentBlock
6. **CommandList**: 添加Command后需要添加到Block.CommandList

---

## 版本信息

- Fungus版本: 基于项目使用的版本
- Unity版本: Unity 6兼容
- 最后更新: 2026-04-03
