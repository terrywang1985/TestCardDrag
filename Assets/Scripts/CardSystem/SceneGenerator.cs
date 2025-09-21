using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 场景生成器，负责通过代码自动创建卡牌游戏所需的所有UI元素
/// </summary>
public class SceneGenerator : MonoBehaviour
{
    [Header("生成设置")]
    public bool generateOnStart = true;  // 是否在启动时自动生成场景
    public int initialHandSize = 5;      // 初始手牌数量
    public int testCardCount = 10;       // 测试卡牌数量

    // 引用存储
    private Canvas gameCanvas;
    private RectTransform handArea;
    private RectTransform tableArea;
    private GameObject cardPrefab;
    private GameManager gameManager;
    private CardManager cardManager;

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateCompleteScene();
        }
    }

    /// <summary>
    /// 生成完整的游戏场景
    /// </summary>
    [ContextMenu("Generate Complete Scene")]
    public void GenerateCompleteScene()
    {
        Debug.Log("开始生成游戏场景...");

        // 1. 创建游戏画布
        CreateGameCanvas();

        // 2. 创建手牌区域
        CreateHandArea();

        // 3. 创建桌面区域
        CreateTableArea();

        // 4. 创建卡牌预制体
        CreateCardPrefab();

        // 5. 创建游戏管理器
        CreateGameManager();

        // 6. 创建卡牌管理器
        CreateCardManager();

        Debug.Log("游戏场景生成完成！");
    }

    /// <summary>
    /// 创建游戏画布
    /// </summary>
    private void CreateGameCanvas()
    {
        // 查找或创建Canvas
        gameCanvas = FindObjectOfType<Canvas>();
        if (gameCanvas == null)
        {
            GameObject canvasObj = new GameObject("GameCanvas");
            gameCanvas = canvasObj.AddComponent<Canvas>();
            gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // 添加CanvasScaler组件
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920); // 竖屏分辨率
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            // 添加GraphicRaycaster组件
            canvasObj.AddComponent<GraphicRaycaster>();
        }
    }

    /// <summary>
    /// 创建手牌区域
    /// </summary>
    private void CreateHandArea()
    {
        if (gameCanvas == null) return;

        // 查找或创建手牌区域
        GameObject handAreaObj = GameObject.Find("HandArea");
        if (handAreaObj == null)
        {
            handAreaObj = new GameObject("HandArea");
            handAreaObj.transform.SetParent(gameCanvas.transform);
            
            // 添加Image组件作为背景
            Image handBackground = handAreaObj.AddComponent<Image>();
            handBackground.color = new Color(0, 0, 0, 0.5f); // 半透明黑色
            
            // 设置RectTransform
            handArea = handAreaObj.GetComponent<RectTransform>();
            handArea.anchorMin = new Vector2(0, 0);
            handArea.anchorMax = new Vector2(1, 0);
            handArea.pivot = new Vector2(0.5f, 0);
            handArea.sizeDelta = new Vector2(0, 200); // 高度200
            handArea.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            handArea = handAreaObj.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// 创建桌面区域
    /// </summary>
    private void CreateTableArea()
    {
        if (gameCanvas == null) return;

        // 查找或创建桌面区域
        GameObject tableAreaObj = GameObject.Find("TableArea");
        if (tableAreaObj == null)
        {
            tableAreaObj = new GameObject("TableArea");
            tableAreaObj.transform.SetParent(gameCanvas.transform);
            
            // 添加Image组件作为背景
            Image tableBackground = tableAreaObj.AddComponent<Image>();
            tableBackground.color = new Color(0, 0.5f, 0, 0.3f); // 半透明绿色
            
            // 设置RectTransform
            tableArea = tableAreaObj.GetComponent<RectTransform>();
            tableArea.anchorMin = new Vector2(0, 0.2f);
            tableArea.anchorMax = new Vector2(1, 0.8f);
            tableArea.pivot = new Vector2(0.5f, 0.5f);
            tableArea.sizeDelta = Vector2.zero;
            tableArea.anchoredPosition = Vector2.zero;
        }
        else
        {
            tableArea = tableAreaObj.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// 创建卡牌预制体
    /// </summary>
    private void CreateCardPrefab()
    {
        // 查找或创建卡牌预制体
        cardPrefab = Resources.Load<GameObject>("CardPrefab");
        if (cardPrefab == null)
        {
            // 创建卡牌游戏对象
            cardPrefab = new GameObject("CardPrefab");
            
            // 添加RectTransform
            RectTransform cardRect = cardPrefab.AddComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(100, 150); // 卡牌尺寸
            
            // 添加CanvasGroup组件
            CanvasGroup canvasGroup = cardPrefab.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            
            // 添加卡牌背景
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(cardPrefab.transform);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = Color.white;
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            
            // 添加卡牌图片
            GameObject imageObj = new GameObject("CardImage");
            imageObj.transform.SetParent(cardPrefab.transform);
            Image cardImage = imageObj.AddComponent<Image>();
            // 使用Unity默认的白色纹理
            cardImage.color = Color.gray;
            RectTransform imageRect = imageObj.GetComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.1f, 0.3f);
            imageRect.anchorMax = new Vector2(0.9f, 0.7f);
            imageRect.sizeDelta = Vector2.zero;
            
            // 添加卡牌名称文本
            GameObject nameObj = new GameObject("CardName");
            nameObj.transform.SetParent(cardPrefab.transform);
            Text nameText = nameObj.AddComponent<Text>();
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.fontSize = 14;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = Color.black;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.1f, 0.7f);
            nameRect.anchorMax = new Vector2(0.9f, 0.85f);
            nameRect.sizeDelta = Vector2.zero;
            
            // 添加卡牌描述文本
            GameObject descObj = new GameObject("Description");
            descObj.transform.SetParent(cardPrefab.transform);
            Text descText = descObj.AddComponent<Text>();
            descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            descText.fontSize = 10;
            descText.alignment = TextAnchor.MiddleCenter;
            descText.color = Color.black;
            descText.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform descRect = descObj.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.1f, 0.15f);
            descRect.anchorMax = new Vector2(0.9f, 0.3f);
            descRect.sizeDelta = Vector2.zero;
            
            // 添加费用文本
            GameObject costObj = new GameObject("Cost");
            costObj.transform.SetParent(cardPrefab.transform);
            Text costText = costObj.AddComponent<Text>();
            costText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            costText.fontSize = 14;
            costText.alignment = TextAnchor.MiddleCenter;
            costText.color = Color.white;
            RectTransform costRect = costObj.GetComponent<RectTransform>();
            costRect.sizeDelta = new Vector2(20, 20);
            costRect.anchoredPosition = new Vector2(-40, 60);
            
            // 添加费用背景
            GameObject costBgObj = new GameObject("CostBackground");
            costBgObj.transform.SetParent(costObj.transform);
            Image costBgImage = costBgObj.AddComponent<Image>();
            costBgImage.color = Color.blue;
            RectTransform costBgRect = costBgObj.GetComponent<RectTransform>();
            costBgRect.anchorMin = Vector2.zero;
            costBgRect.anchorMax = Vector2.one;
            costBgRect.sizeDelta = Vector2.zero;
            costBgRect.SetAsFirstSibling();
            
            // 添加攻击力文本
            GameObject attackObj = new GameObject("Attack");
            attackObj.transform.SetParent(cardPrefab.transform);
            Text attackText = attackObj.AddComponent<Text>();
            attackText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            attackText.fontSize = 14;
            attackText.alignment = TextAnchor.MiddleCenter;
            attackText.color = Color.white;
            RectTransform attackRect = attackObj.GetComponent<RectTransform>();
            attackRect.sizeDelta = new Vector2(20, 20);
            attackRect.anchoredPosition = new Vector2(-40, -60);
            
            // 添加攻击力背景
            GameObject attackBgObj = new GameObject("AttackBackground");
            attackBgObj.transform.SetParent(attackObj.transform);
            Image attackBgImage = attackBgObj.AddComponent<Image>();
            attackBgImage.color = Color.red;
            RectTransform attackBgRect = attackBgObj.GetComponent<RectTransform>();
            attackBgRect.anchorMin = Vector2.zero;
            attackBgRect.anchorMax = Vector2.one;
            attackBgRect.sizeDelta = Vector2.zero;
            attackBgRect.SetAsFirstSibling();
            
            // 添加生命值文本
            GameObject healthObj = new GameObject("Health");
            healthObj.transform.SetParent(cardPrefab.transform);
            Text healthText = healthObj.AddComponent<Text>();
            healthText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            healthText.fontSize = 14;
            healthText.alignment = TextAnchor.MiddleCenter;
            healthText.color = Color.white;
            RectTransform healthRect = healthObj.GetComponent<RectTransform>();
            healthRect.sizeDelta = new Vector2(20, 20);
            healthRect.anchoredPosition = new Vector2(40, -60);
            
            // 添加生命值背景
            GameObject healthBgObj = new GameObject("HealthBackground");
            healthBgObj.transform.SetParent(healthObj.transform);
            Image healthBgImage = healthBgObj.AddComponent<Image>();
            healthBgImage.color = Color.green;
            RectTransform healthBgRect = healthBgObj.GetComponent<RectTransform>();
            healthBgRect.anchorMin = Vector2.zero;
            healthBgRect.anchorMax = Vector2.one;
            healthBgRect.sizeDelta = Vector2.zero;
            healthBgRect.SetAsFirstSibling();
            
            // 添加CardVisual组件
            CardVisual cardVisual = cardPrefab.AddComponent<CardVisual>();
            cardVisual.cardBackground = bgImage;
            cardVisual.cardImage = cardImage;
            cardVisual.cardNameText = nameText;
            cardVisual.descriptionText = descText;
            cardVisual.costText = costText;
            cardVisual.attackText = attackText;
            cardVisual.healthText = healthText;
            
            // 添加CardDragHandler组件
            CardDragHandler dragHandler = cardPrefab.AddComponent<CardDragHandler>();
            
            // 保存到Resources文件夹，以便后续加载
            // 注意：在实际运行时无法直接保存资源到Assets文件夹，这里仅为了演示
            Debug.Log("卡牌预制体已创建，请手动保存为预制体。");
        }
    }

    /// <summary>
    /// 创建游戏管理器
    /// </summary>
    private void CreateGameManager()
    {
        // 查找或创建游戏管理器
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            GameObject managerObj = new GameObject("GameManager");
            gameManager = managerObj.AddComponent<GameManager>();
        }
        
        // 设置游戏管理器的引用
        gameManager.gameCanvas = gameCanvas.gameObject;
        gameManager.handArea = handArea.transform;
        gameManager.tableArea = tableArea.transform;
    }

    /// <summary>
    /// 创建卡牌管理器
    /// </summary>
    private void CreateCardManager()
    {
        // 查找或创建卡牌管理器
        if (gameManager.cardManager == null)
        {
            GameObject cardManagerObj = new GameObject("CardManager");
            cardManagerObj.transform.SetParent(gameManager.transform);
            cardManager = cardManagerObj.AddComponent<CardManager>();
            
            // 添加测试卡牌生成器
            TestCardGenerator testGenerator = cardManagerObj.AddComponent<TestCardGenerator>();
            testGenerator.numberOfTestCards = testCardCount;
            testGenerator.cardManager = cardManager;
            
            // 设置卡牌管理器的引用
            gameManager.cardManager = cardManager;
        }
        else
        {
            cardManager = gameManager.cardManager;
        }
        
        // 设置卡牌管理器的参数
        cardManager.cardPrefab = cardPrefab;
        cardManager.handArea = handArea.transform;
        cardManager.tableArea = tableArea.transform;
        cardManager.initialHandSize = initialHandSize;
        
        // 如果没有卡牌数据，生成测试数据
        if (cardManager.cardDatabase == null)
        {
            cardManager.cardDatabase = new List<CardData>();
        }
        
        // 确保有CardManager组件上的TestCardGenerator引用
        TestCardGenerator generator = cardManager.GetComponent<TestCardGenerator>();
        if (generator == null)
        {
            generator = cardManager.gameObject.AddComponent<TestCardGenerator>();
            generator.cardManager = cardManager;
            generator.numberOfTestCards = testCardCount;
        }
    }
}