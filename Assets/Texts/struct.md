# RPGShooter2D 项目脚本结构分析

## 📁 Scripts文件夹结构总览

### 🔧 **核心系统脚本**

**Collidable.cs**
- 基础碰撞系统类，提供2D碰撞检测的虚函数框架
- 所有需要碰撞检测的游戏对象都继承自此类

**Damageable.cs**
- 可伤害对象基类，继承自Collidable
- 管理生命值系统，提供受伤和治疗功能
- 包含HealthBar引用用于UI显示

**Character.cs**
- 角色基类，继承自Damageable
- 定义角色的基本属性：护盾、速度、伤害
- 提供移动、攻击、死亡等虚函数

### 🎮 **游戏管理器**

**GameManager.cs**
- 单例模式的游戏管理器
- 管理游戏状态、玩家数据、场景切换
- 处理游戏结束和胜利界面
- 提供存档/读档功能框架

### 👤 **角色系统**

**玩家相关：**
- **Player.cs** - 玩家控制器，处理输入、武器切换、背包管理

**敌人相关：**
- **Enemy.cs** - 敌人基类，实现AI移动、攻击、掉落
- **EnemyAI.cs** - 敌人AI系统，包含巡逻、追击、攻击三种状态
- **Boss.cs** - Boss敌人类
- **Zombie.cs** - 僵尸敌人类
- **Treant.cs** - 树精敌人类
- **Ghost.cs** - 幽灵敌人类

**NPC相关：**
- **NPC.cs** - NPC基类
- **Vendor.cs** - 商人NPC

### 🔫 **武器系统**

**Weapon.cs**
- 武器基类，继承自Collectible
- 管理武器耐久度、攻击功能
- 处理武器与玩家的附着逻辑

**Gun.cs**
- 枪械类，继承自Weapon
- 实现鼠标跟随、射击功能
- 管理射击点和子弹发射

**子弹相关：**
- **Bullet.cs** - 玩家子弹
- **EnemyBullet.cs** - 敌人子弹

### 📦 **收集品系统**

**Collectible.cs**
- 收集品基类，处理碰撞检测和收集逻辑

**具体收集品：**
- **Coin.cs** - 金币
- **Potion.cs** - 药水
- **Chest.cs** - 宝箱
- **GoldChest.cs** - 黄金宝箱

### 💰 **背包与商店系统**

**Inventory.cs**
- 简单的列表式背包系统
- 管理物品添加和获取

**UI_Inventory.cs**
- 背包UI界面管理

**商店相关：**
- **Shop.cs** - 商店逻辑
- **UI_Shop.cs** - 商店UI

### 🏛️ **传送门系统**

**Portal.cs**
- 基础传送门，触发后加载下一个场景

**WinPortal.cs** 
- 胜利传送门，触发游戏胜利界面

### 📊 **UI系统**

**HealthBar.cs**
- 生命值条管理类（注意：实际代码中是一个数据类，不是UI组件）

**其他UI：**
- **CoinCounter.cs** - 金币计数器
- **MapMenu.cs** - 地图菜单
- **UpgradeMenu.cs** - 升级菜单
- **GameOverUI.cs** - 游戏结束界面
- **GameWinUI.cs** - 游戏胜利界面
- **MoneyCounterUI.cs** - 金钱计数器UI

### 🎯 **浮动文字系统**

**FloatingText.cs**
- 浮动文字单个实例

**FloatingTextManager.cs**
- 浮动文字管理器，用于显示伤害数字等临时文字

### 📷 **相机系统**

**CameraFollow.cs**
- 相机跟随玩家

**Crosshair.cs**
- 准星控制

### 🗣️ **对话系统**

**Dialog.cs**
- 对话系统基础类

**DialogManager.cs**
- 对话管理器

### 🔊 **音效系统**

**Sound.cs**
- 音效数据类

**SoundManager.cs**
- 音效管理器，单例模式

### 🏚️ **可破坏物**

**Destructible.cs**
- 可破坏物基类

**Barrel.cs**
- 木桶可破坏物

### ⚡ **触发器系统**

**TriggerZone.cs**
- 触发区域基类

**TriggerPoint.cs**
- 触发点

**陷阱系统：**
- **Spawner.cs** - 敌人生成器
- **BossTrigger.cs** - Boss触发器
- **BossPortalTrigger.cs** - Boss传送门触发器
- **EnemyTriggerZone.cs** - 敌人触发区域
- **PortalTrigger.cs** - 传送门触发器

---

## 📋 **项目架构总结**

这个项目是一个完整的2D RPG射击游戏框架，包含了：

1. **完整的角色系统** - 玩家、多种敌人、NPC
2. **武器系统** - 枪械、子弹、耐久度管理
3. **收集品系统** - 金币、药水、宝箱
4. **背包与商店** - 物品管理和交易
5. **AI系统** - 敌人AI包含巡逻、追击、攻击状态
6. **UI系统** - 生命值、金币、菜单等完整界面
7. **游戏管理** - 场景切换、游戏状态管理
8. **特效系统** - 浮动文字、音效
9. **触发器系统** - 各种游戏事件触发

### 🔧 **技术特点**
- 采用面向对象设计模式
- 大量使用继承和单例模式
- 代码结构清晰，模块化程度高
- 完整的2D游戏功能实现
- 支持武器切换、背包管理、商店交易等RPG元素
- 包含完整的敌人AI行为树

这是一个功能完整、架构良好的2D RPG射击游戏项目，适合作为学习和开发的基础框架。

---

## 📁 Assets文件夹结构分析

### 🎬 **Animations**
- 存放游戏中所有的动画资源
- 按类型分类存储，包含：
  - **Bullet/** - 子弹相关动画
  - **Chest/** - 宝箱动画
  - **Collectibles/** - 收集品动画
  - **Enemy/** - 敌人动画（包含Boss、Crab、Ghost、Mole、Treant、Zombie等）
  - **Explosion/** - 爆炸动画
  - **Misc/** - 杂项动画
  - **Player/** - 玩家动画
- 包含动画剪辑（.anim）和动画控制器（.controller）

### 🎨 **Brackeys**
- 存放从Brackeys 2D Mega Pack中导入的资源
- 包含：
  - **Characters/** - 角色资源（宇航员、侦探、Blob玩家、士兵）
  - **Effects/** - 特效资源（爆炸、火球）
  - **Enemies/** - 敌人资源（昆虫）
  - **Items & Icons/** - 物品和图标资源
- 提供了多种预设动画和角色模型

### 🧩 **Prefabs**
- 存放游戏预制体
- 目前包含：
  - **UI/** - UI相关预制体（如Health Bar）

### ⚙️ **Preset**
- 存放Unity预设配置
- 包含：
  - **Tags & Layers - Pixel Art Top Down - Basic.preset** - 像素艺术顶视角游戏的标签和层预设

### 📦 **Resources**
- 存放游戏资源
- 包含：
  - **Audio/** - 游戏音效和背景音乐（战斗主题、宝箱、拾取等）
  - **Font/** - 游戏字体（Orange kid、ThaleahFat）
  - **BillingMode.json** - 计费模式配置

### 🎮 **Scenes**
- 存放游戏场景
- 目前包含：
  - **Template.scenetemplate** - 场景模板

### 📝 **Scripts**
- 存放游戏脚本
- 按功能模块分类，包含：
  - **Camera/** - 相机相关脚本
  - **Character/** - 角色相关脚本（玩家、敌人、NPC）
  - **Collectibles/** - 收集品相关脚本
  - **Colliable/** - 碰撞相关脚本
  - **Damageable/** - 可伤害对象相关脚本
  - **Destructibles/** - 可破坏物相关脚本
  - **Dialog/** - 对话系统相关脚本
  - **FloatingText/** - 浮动文字相关脚本
  - **GameManager/** - 游戏管理器相关脚本
  - **Inventory/** - 背包系统相关脚本
  - **Portal/** - 传送门相关脚本
  - **Shop/** - 商店系统相关脚本
  - **Sound/** - 音效相关脚本
  - **TriggerSystem/** - 触发器系统相关脚本
  - **UI/** - UI相关脚本
  - **Weapon/** - 武器系统相关脚本

### 🖼️ **Spriters**
- 存放精灵资源
- 包含：
  - **bullets/** - 子弹精灵
  - **environments/** - 环境精灵（花园场景）
  - **heroes/** - 英雄角色精灵
- 可能包含用于创建精灵的源文件（.aseprite）

### 📄 **Texts**
- 存放文本文件
- 包含：
  - **struct.md** - 项目结构文档
  - **临时文件夹** - 临时文件

### 📋 **Assets文件夹总结**

Assets文件夹是Unity项目的核心资源目录，包含了游戏开发所需的所有资源：

1. **动画资源** - 为游戏元素提供生动的动画效果
2. **预设资源** - 提供了现成的角色和特效
3. **预制体** - 简化游戏对象的创建和管理
4. **配置文件** - 存储游戏设置和预设
5. **媒体资源** - 音效、音乐和字体
6. **场景文件** - 游戏关卡设计
7. **脚本代码** - 游戏逻辑实现
8. **精灵资源** - 游戏视觉元素
9. **文档文件** - 项目说明和结构文档

这种组织结构使得项目资源管理清晰有序，便于开发和维护。