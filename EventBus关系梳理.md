# EventBus 事件发布与订阅关系

## 全局事件

Enemy.cs -> EnemyKilledEvent -> PlayerEventBus.cs
Death.cs -> EnemyKilledEvent -> PlayerEventBus.cs
Boss.cs -> EnemyKilledEvent -> PlayerEventBus.cs
Player.cs -> PlayerDieEvent -> GameManager.cs
Player.cs -> PlayerDieEvent -> PlayerAnimationController.cs
Player.cs -> PlayerHPChangeEvent -> PlayerHPUI.cs
Damageable.cs -> PlayerHitEvent -> PlayerEventBus.cs
Player.cs -> PlayerAttackEvent -> PlayerEventBus.cs
Bullet.cs -> PlayerHitEnemyEvent -> PlayerEventBus.cs
Bullet.cs -> PlayerBulletMissEvent -> PlayerEventBus.cs
Collectible.cs -> ObtainEquipmentEvent -> PlayerEventBus.cs
UIManager.cs -> QuitEvent -> GameManager.cs
UIManager.cs -> RetryEvent -> GameManager.cs
GameManager.cs -> GameOverEvent -> UIManager.cs
WinPortal.cs -> GameWinEvent -> GameManager.cs
WinPortal.cs -> GameWinEvent -> UIManager.cs
UIManager.cs -> GamePauseEvent -> GameManager.cs
UIManager.cs -> GameResumeEvent -> GameManager.cs
NPCSettingManagerAsync.cs -> NPCStartGenerateEvent -> NPCGenerator.cs
NPCSettingManagerAsync.cs -> NPCGeneratedEvent -> NPCSocialManager.cs
NPCSettingManagerAsync.cs -> NPCGeneratedEvent -> TipListener.cs
NPCDialogue.cs -> NPCRspActionEvent -> NPCAction.cs
PlayerInputManager.cs -> PlayerInputEvent -> NPCDialogue.cs
PlayerInputManager.cs -> PlayerInputEvent -> NPCEmotion.cs
Dialogable.cs -> PlayerInteractEvent -> (无订阅者)
BossPortal.cs -> BossSpawnEvent -> (无订阅者)
Boss.cs -> BossKilledEvent -> (无订阅者)
Death.cs -> BossKilledEvent -> (无订阅者)
Bullet.cs -> BulletHitEvent -> Damageable.cs
Bullet.cs -> BulletHitEvent -> Player.cs
Character.cs -> EntityDieEvent -> (无订阅者)

## NPC 本地事件

NPCMovementController.cs -> NPCMoveEvent -> NPCAnimationController.cs
NPCMovementController.cs -> NPCStopMoveEvent -> NPCAnimationController.cs
NPCMovementController.cs -> NPCChangeDirEvent -> NPCAnimationController.cs
NPCAction.cs -> NPCFollowEvent -> NPCMovementActions.cs
NPCAction.cs -> NPCEscapeEvent -> NPCMovementActions.cs
NPCAction.cs -> NPCStandbyEvent -> NPCMovementActions.cs
NPCDialogue.cs -> NPCEmotionEvent -> NPCEmotion.cs
NPCSocialAware.cs -> NPCEmotionEvent -> NPCEmotion.cs
NPCDialogue.cs -> NPCSocialRelationChangeEvent -> NPCSocialManager.cs

## 说明
- 此梳理基于代码搜索结果
- 标记为"无订阅者"的事件表示代码中有发布但未找到订阅者
- 实际运行时的事件流可能会根据具体场景有所不同
