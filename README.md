# Unity 2D卡牌对战系统

这是一个基于Unity 2D的竖屏卡牌对战系统，允许玩家从手牌区域拖拽卡牌到桌面上进行游戏。

## 项目结构

```
Assets/
├── Resources/             # 资源文件夹
│   └── CardData/          # 卡牌数据文件
├── Scenes/                # 场景文件
│   └── SampleScene.unity  # 示例场景
├── Scripts/               # 脚本文件
│   └── CardSystem/        # 卡牌系统相关脚本
│       ├── CardData.cs           # 卡牌数据类
│       ├── CardVisual.cs         # 卡牌视觉组件
│       ├── CardDragHandler.cs    # 卡牌拖拽处理器
│       ├── CardManager.cs        # 卡牌管理器
│       ├── GameManager.cs        # 游戏管理器
│       ├── TestCardGenerator.cs  # 测试卡牌生成器
│       └── PrefabCreationGuide.txt # 预制体创建指南
└── Settings/              # 项目设置
```

## 功能说明

1. **卡牌数据系统**：使用ScriptableObject存储卡牌信息
2. **卡牌拖拽功能**：支持从手牌区域拖拽到桌面区域
3. **手牌管理**：自动排列手牌，确保显示美观
4. **测试数据生成**：自动生成测试卡牌，方便开发调试

## 核心组件介绍

### CardData.cs
定义卡牌的基本属性，包括名称、描述、费用、攻击力、生命值等。

### CardVisual.cs
负责卡牌的视觉表现，更新卡牌的显示内容和样式。

### CardDragHandler.cs
实现卡牌的拖拽逻辑，包括开始拖拽、拖拽中和结束拖拽的处理。

### CardManager.cs
管理卡牌的生成、分发和排列，是卡牌系统的核心控制器。

### GameManager.cs
负责整个游戏的流程控制，包括屏幕设置、组件初始化等。

### TestCardGenerator.cs
用于在游戏运行时自动生成测试卡牌数据，便于开发和调试。

## 使用指南

### 1. 设置场景

1. 在Unity编辑器中打开SampleScene
2. 创建游戏画布(GameCanvas)，设置为竖屏模式
3. 创建手牌区域(HandArea)和桌面区域(TableArea)
4. 创建卡牌预制体(CardPrefab)

### 2. 配置组件

1. 将GameManager脚本添加到场景中的GameObject上
2. 将CardManager和TestCardGenerator脚本添加到子GameObject上
3. 在Inspector面板中正确设置所有引用

### 3. 测试游戏

1. 点击Play按钮运行游戏
2. 系统会自动生成测试卡牌并显示在手牌区域
3. 尝试拖拽卡牌到手牌区域和桌面区域
4. 验证拖拽功能是否正常工作

## 开发建议

1. 为卡牌添加真实的图片资源
2. 扩展卡牌系统，添加更多功能如卡牌效果、动画等
3. 根据需要调整UI布局和视觉效果
4. 添加玩家状态、回合制系统等游戏核心机制
5. 实现卡牌的不同类型和效果

## 注意事项

- 确保设置正确的竖屏分辨率和UI缩放模式
- 本项目使用Unity的UI系统实现卡牌显示和交互
- 如需添加3D效果或更复杂的动画，可以考虑使用其他方法
- 在发布前，移除测试代码和生成器

## License

MIT License

## 致谢

感谢使用本卡牌系统模板，祝您开发愉快！