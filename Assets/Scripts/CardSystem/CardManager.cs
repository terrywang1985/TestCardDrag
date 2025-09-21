using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 卡牌管理器，负责卡牌的生成、分发和管理
/// </summary>
public class CardManager : MonoBehaviour
{
    [Header("卡牌设置")]
    public List<CardData> cardDatabase; // 卡牌数据库
    public GameObject cardPrefab;       // 卡牌预制体
    public Transform handArea;          // 手牌区域
    public Transform tableArea;         // 桌面区域
    public int initialHandSize = 5;     // 初始手牌数量

    private List<GameObject> playerHand = new List<GameObject>(); // 玩家手牌
    private float lastUpdateTime = 0f;
    private float updateInterval = 0.5f; // 每0.5秒检查一次卡牌状态

    private void Awake()
    {
        DebugLogger.Log("CardManager Awake: 初始化卡牌管理器");
    }

    private void Start()
    {
        DebugLogger.Log("CardManager Start: 开始初始化手牌，初始手牌数量: " + initialHandSize);
        
        // 检查卡牌管理器的引用
        LogManagerReferences();
        
        // 启动定期检查
        InvokeRepeating("PeriodicCardStateCheck", 1f, updateInterval);
        
        // 初始化手牌
        DrawInitialHand();
    }

    /// <summary>
    /// 记录卡牌管理器的引用状态
    /// </summary>
    private void LogManagerReferences()
    {
        DebugLogger.Log("=== 卡牌管理器引用状态 ===");
        DebugLogger.Log("卡牌数据库是否为空: " + (cardDatabase == null || cardDatabase.Count == 0));
        DebugLogger.Log("卡牌预制体是否为空: " + (cardPrefab == null));
        DebugLogger.Log("手牌区域是否为空: " + (handArea == null));
        DebugLogger.Log("桌面区域是否为空: " + (tableArea == null));
        
        if (cardDatabase != null)
        {
            DebugLogger.Log("卡牌数据库中卡牌数量: " + cardDatabase.Count);
        }
        
        if (handArea != null)
        {
            RectTransform handRect = handArea.GetComponent<RectTransform>();
            if (handRect != null)
            {
                DebugLogger.Log("手牌区域RectTransform: 宽=" + handRect.rect.width + ", 高=" + handRect.rect.height);
            }
        }
        
        DebugLogger.Log("=========================");
    }

    /// <summary>
    /// 定期检查卡牌状态
    /// </summary>
    private void PeriodicCardStateCheck()
    {
        if (playerHand == null || playerHand.Count == 0)
            return;
        
        DebugLogger.Log("===== 卡牌状态检查 =====");
        for (int i = 0; i < playerHand.Count; i++)
        {
            GameObject card = playerHand[i];
            if (card == null)
            {
                DebugLogger.LogWarning("手牌中的卡牌对象为空，索引: " + i);
                continue;
            }
            
            RectTransform cardRect = card.GetComponent<RectTransform>();
            if (cardRect == null)
            {
                DebugLogger.LogError("卡牌'" + card.name + "'缺少RectTransform组件");
                continue;
            }
            
            // 检查卡牌的可见性相关属性
            CanvasRenderer renderer = card.GetComponent<CanvasRenderer>();
            bool isVisible = renderer != null && renderer.cull == false;
            
            // 检查父对象层级
            string hierarchyPath = GetGameObjectPath(card.transform);
            
            DebugLogger.Log("卡牌'" + card.name + "': 位置=" + cardRect.anchoredPosition + ", 世界位置=" + card.transform.position + ", 缩放=" + cardRect.localScale + ", 层级路径='" + hierarchyPath + "', 可见性=" + isVisible + ", UI层级=" + card.transform.GetSiblingIndex() + "/" + card.transform.parent.childCount);
        }
        DebugLogger.Log("=====================");
    }

    /// <summary>
    /// 获取游戏对象的完整路径
    /// </summary>
    private string GetGameObjectPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }

    /// <summary>
    /// 绘制初始手牌
    /// </summary>
    private void DrawInitialHand()
    {
        DebugLogger.Log("开始绘制初始手牌，数量: " + initialHandSize);
        
        // 检查是否有卡牌数据
        if (cardDatabase == null || cardDatabase.Count == 0)
        {
            DebugLogger.LogError("卡牌数据库为空，无法绘制初始手牌");
            // 尝试找到TestCardGenerator并生成卡牌数据
            TestCardGenerator generator = GetComponent<TestCardGenerator>();
            if (generator != null)
            {
                DebugLogger.Log("找到TestCardGenerator，尝试生成测试卡牌数据");
                generator.GenerateTestCards();
                
                // 等待一帧让数据生成完成
                StartCoroutine(WaitAndDrawCardsAfterGeneration());
            }
            return;
        }
        
        for (int i = 0; i < initialHandSize; i++)
        {
            DrawCard();
        }
        
        // 排列手牌
        ArrangeHandCards();
    }

    /// <summary>
    /// 等待卡牌数据生成后再绘制卡牌
    /// </summary>
    private System.Collections.IEnumerator WaitAndDrawCardsAfterGeneration()
    {
        yield return null; // 等待一帧
        
        if (cardDatabase != null && cardDatabase.Count > 0)
        {
            DebugLogger.Log("卡牌数据生成完成，现在绘制初始手牌，数量: " + initialHandSize);
            for (int i = 0; i < initialHandSize; i++)
            {
                DrawCard();
            }
            ArrangeHandCards();
        }
        else
        {
            DebugLogger.LogError("卡牌数据生成失败，仍然为空");
        }
    }

    /// <summary>
    /// 抽一张卡牌
    /// </summary>
    public void DrawCard()
    {
        DebugLogger.Log("开始抽牌流程");
        
        if (cardDatabase == null || cardDatabase.Count == 0)
        {
            DebugLogger.LogError("卡牌数据库为空，无法抽牌");
            return;
        }

        if (handArea == null)
        {
            DebugLogger.LogError("手牌区域未设置，无法抽牌");
            return;
        }

        if (cardPrefab == null)
        {
            DebugLogger.LogError("卡牌预制体未设置，无法抽牌");
            return;
        }

        // 从数据库中随机选择一张卡牌
        int randomIndex = Random.Range(0, cardDatabase.Count);
        CardData selectedCard = cardDatabase[randomIndex];
        DebugLogger.Log("抽取卡牌: " + selectedCard.cardName + " (类型: " + selectedCard.cardType + ", 费用: " + selectedCard.cost + ", 攻击力: " + selectedCard.attack + ", 生命值: " + selectedCard.health + ")");
        
        // 检查卡牌精灵
        DebugLogger.Log("卡牌精灵存在: " + (selectedCard.cardSprite != null) + (selectedCard.cardSprite != null ? ", 精灵名称: " + selectedCard.cardSprite.name : ""));

        // 实例化卡牌 - 注意：这里先不设置父物体，等设置完属性后再设置
        GameObject newCard = Instantiate(cardPrefab);
        newCard.name = selectedCard.cardName;
        DebugLogger.Log("已实例化卡牌: " + newCard.name + ", ID: " + newCard.GetInstanceID());

        // 设置卡牌数据
        CardVisual cardVisual = newCard.GetComponent<CardVisual>();
        if (cardVisual != null)
        {
            cardVisual.SetCardData(selectedCard);
            DebugLogger.Log("已设置卡牌视觉数据: " + selectedCard.cardName);
        }
        else
        {
            DebugLogger.LogError("卡牌" + selectedCard.cardName + "没有CardVisual组件，无法显示卡牌信息");
        }

        // 设置拖拽处理器
        CardDragHandler dragHandler = newCard.GetComponent<CardDragHandler>();
        if (dragHandler != null)
        {
            // 确保正确设置handArea和tableArea引用
            if (handArea == null)
            {
                DebugLogger.LogError("CardManager的handArea引用为空");
            }
            if (tableArea == null)
            {
                DebugLogger.LogError("CardManager的tableArea引用为空");
            }
            
            dragHandler.handArea = handArea;
            dragHandler.tableArea = tableArea;
            DebugLogger.Log("已设置卡牌" + selectedCard.cardName + "的拖拽处理器引用");
        }
        else
        {
            DebugLogger.LogWarning("卡牌" + selectedCard.cardName + "没有CardDragHandler组件");
            // 添加一个警告性的临时拖拽处理器，以便至少可以看到卡牌
            CardDragHandler tempHandler = newCard.AddComponent<CardDragHandler>();
            tempHandler.handArea = handArea;
            tempHandler.tableArea = tableArea;
            DebugLogger.Log("已添加临时拖拽处理器到卡牌: " + selectedCard.cardName);
        }

        // 确保卡牌有RectTransform并设置正确的属性
        RectTransform cardRect = newCard.GetComponent<RectTransform>();
        if (cardRect != null)
        {
            // 设置正确的层级 - 确保在Canvas的子层级中
            DebugLogger.Log("设置卡牌" + newCard.name + "的父对象为手牌区域前，当前父对象: " + (newCard.transform.parent ? newCard.transform.parent.name : "无"));
            newCard.transform.SetParent(handArea, false); // 添加false参数避免世界坐标转换
            newCard.transform.localScale = Vector3.one;
            newCard.transform.localRotation = Quaternion.identity;
            
            // 临时设置一个明显的位置，以便在排列前可见
            cardRect.anchoredPosition = Vector2.zero;
            DebugLogger.Log("卡牌" + newCard.name + "初始位置设置为: " + cardRect.anchoredPosition);
            
            // 检查Canvas层级
            Canvas canvas = FindParentCanvas(newCard.transform);
            if (canvas != null)
            {
                DebugLogger.Log("卡牌所在Canvas: " + canvas.name + ", RenderMode: " + canvas.renderMode);
            }
            else
            {
                DebugLogger.LogError("卡牌不在任何Canvas层级下，这可能导致显示问题！");
            }
            
            // 确保卡牌可见
            CanvasRenderer renderer = newCard.GetComponent<CanvasRenderer>();
            if (renderer != null)
            {
                renderer.cull = false;
                DebugLogger.Log("已设置卡牌" + newCard.name + "的CanvasRenderer.cull = false");
            }
            
            // 确保CanvasGroup设置正确
            CanvasGroup canvasGroup = newCard.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
                DebugLogger.Log("已设置卡牌" + newCard.name + "的CanvasGroup属性: alpha=1, blocksRaycasts=true, interactable=true");
            }
        }
        else
        {
            DebugLogger.LogError("卡牌" + selectedCard.cardName + "没有RectTransform组件");
        }

        // 添加到玩家手牌
        playerHand.Add(newCard);
        DebugLogger.Log("卡牌" + selectedCard.cardName + "已添加到手牌，当前手牌数量: " + playerHand.Count);
        
        // 重新排列手牌
        ArrangeHandCards();
    }

    /// <summary>
    /// 查找父对象中的Canvas组件
    /// </summary>
    private Canvas FindParentCanvas(Transform transform)
    {
        Transform current = transform;
        while (current != null)
        {
            Canvas canvas = current.GetComponent<Canvas>();
            if (canvas != null)
                return canvas;
            current = current.parent;
        }
        return null;
    }

    /// <summary>
    /// 排列手牌
    /// </summary>
    private void ArrangeHandCards()
    {
        DebugLogger.Log("开始排列手牌，当前手牌数量: " + (playerHand == null ? 0 : playerHand.Count));
        
        if (handArea == null)
        {
            DebugLogger.LogError("手牌区域未设置，无法排列手牌");
            return;
        }

        RectTransform handRect = handArea.GetComponent<RectTransform>();
        if (handRect == null)
        {
            DebugLogger.LogError("手牌区域的RectTransform不存在！");
            return;
        }

        if (playerHand == null || playerHand.Count == 0)
        {
            DebugLogger.Log("手牌为空，无需排列");
            return;
        }

        // 确保所有卡牌都是handArea的直接子物体
        for (int i = 0; i < playerHand.Count; i++)
        {
            if (playerHand[i].transform.parent != handArea)
            {
                DebugLogger.Log("卡牌" + playerHand[i].name + "不在手牌区域层级下，重新设置父对象");
                playerHand[i].transform.SetParent(handArea, false); // 添加false参数避免世界坐标转换
                playerHand[i].transform.localScale = Vector3.one;
            }
        }

        float handWidth = handRect.rect.width;
        float cardWidth = cardPrefab.GetComponent<RectTransform>().rect.width;
        float spacing = (handWidth - (playerHand.Count * cardWidth)) / (playerHand.Count + 1);
        
        DebugLogger.Log("手牌排列计算: 手牌区域宽度=" + handWidth + ", 卡牌宽度=" + cardWidth + ", 初始间距=" + spacing);

        // 如果卡牌太挤，调整间距
        if (spacing < 10f)
        {
            spacing = 10f;
            float totalWidth = (playerHand.Count * cardWidth) + ((playerHand.Count - 1) * spacing);
            float startX = -totalWidth / 2 + cardWidth / 2;
            
            DebugLogger.Log("卡牌较挤模式: 调整间距=" + spacing + ", 总宽度=" + totalWidth + ", 起始X=" + startX);

            for (int i = 0; i < playerHand.Count; i++)
            {
                GameObject card = playerHand[i];
                RectTransform cardRect = card.GetComponent<RectTransform>();
                if (cardRect != null)
                {
                    Vector2 newPosition = new Vector2(startX + (i * (cardWidth + spacing)), 75);
                    DebugLogger.Log("卡牌" + card.name + "排列前位置: " + cardRect.anchoredPosition + ", 排列后位置: " + newPosition);
                    
                    cardRect.anchoredPosition = newPosition;
                    
                    // 确保卡牌可见
                    CanvasRenderer renderer = card.GetComponent<CanvasRenderer>();
                    if (renderer != null)
                    {
                        renderer.cull = false;
                    }
                    
                    // 确保CanvasGroup设置正确
                    CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 1f;
                        canvasGroup.blocksRaycasts = true;
                        canvasGroup.interactable = true;
                    }
                    
                    cardRect.localRotation = Quaternion.identity;
                    cardRect.localScale = Vector3.one;
                    // 确保卡牌在最上层
                    card.transform.SetAsLastSibling();
                    
                    // 设置锚点和中心点以确保正确显示在Canvas上
                    cardRect.anchorMin = new Vector2(0.5f, 0);
                    cardRect.anchorMax = new Vector2(0.5f, 0);
                    cardRect.pivot = new Vector2(0.5f, 0.5f);
                    
                    // 强制刷新UI
                    Canvas.ForceUpdateCanvases();
                    
                    // 验证设置是否生效
                    DebugLogger.Log("卡牌" + card.name + "排列后验证: 位置=" + cardRect.anchoredPosition + ", 层级=" + card.transform.GetSiblingIndex());
                }
                else
                {
                    DebugLogger.LogWarning("卡牌" + card.name + "没有RectTransform组件");
                }
            }
        }
        else
        {
            // 正常排列
            float startX = -handWidth / 2 + spacing + cardWidth / 2;
            
            DebugLogger.Log("正常排列模式: 间距=" + spacing + ", 起始X=" + startX);
            
            for (int i = 0; i < playerHand.Count; i++)
            {
                GameObject card = playerHand[i];
                RectTransform cardRect = card.GetComponent<RectTransform>();
                if (cardRect != null)
                {
                    Vector2 newPosition = new Vector2(startX + (i * (cardWidth + spacing)), 75);
                    DebugLogger.Log("卡牌" + card.name + "排列前位置: " + cardRect.anchoredPosition + ", 排列后位置: " + newPosition);
                    
                    cardRect.anchoredPosition = newPosition;
                    
                    // 确保卡牌可见
                    CanvasRenderer renderer = card.GetComponent<CanvasRenderer>();
                    if (renderer != null)
                    {
                        renderer.cull = false;
                    }
                    
                    // 确保CanvasGroup设置正确
                    CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 1f;
                        canvasGroup.blocksRaycasts = true;
                        canvasGroup.interactable = true;
                    }
                    
                    cardRect.localRotation = Quaternion.identity;
                    cardRect.localScale = Vector3.one;
                    // 确保卡牌在最上层
                    card.transform.SetAsLastSibling();
                    
                    // 设置锚点和中心点以确保正确显示在Canvas上
                    cardRect.anchorMin = new Vector2(0.5f, 0);
                    cardRect.anchorMax = new Vector2(0.5f, 0);
                    cardRect.pivot = new Vector2(0.5f, 0.5f);
                    
                    // 强制刷新UI
                    Canvas.ForceUpdateCanvases();
                    
                    // 验证设置是否生效
                    DebugLogger.Log("卡牌" + card.name + "排列后验证: 位置=" + cardRect.anchoredPosition + ", 层级=" + card.transform.GetSiblingIndex());
                }
                else
                {
                    DebugLogger.LogWarning("卡牌" + card.name + "没有RectTransform组件");
                }
            }
        }
        
        DebugLogger.Log("已完成手牌排列，卡牌数量: " + playerHand.Count);
    }

    /// <summary>
    /// 从手牌中移除卡牌
    /// </summary>
    /// <param name="card">要移除的卡牌</param>
    public void RemoveCardFromHand(GameObject card)
    {
        if (card == null)
        {
            DebugLogger.LogWarning("尝试移除空卡牌对象");
            return;
        }
        
        if (playerHand.Contains(card))
        {
            DebugLogger.Log("从手牌中移除卡牌: " + card.name + "，移除前数量: " + playerHand.Count);
            playerHand.Remove(card);
            DebugLogger.Log("卡牌移除后数量: " + playerHand.Count);
            ArrangeHandCards();
        }
        else
        {
            DebugLogger.LogWarning("卡牌" + card.name + "不在手牌列表中");
        }
    }

    /// <summary>
    /// 初始化卡牌数据库 - 可以被TestCardGenerator调用
    /// </summary>
    public void InitializeCardDatabase()
    {
        DebugLogger.Log("卡牌数据库初始化完成，当前卡牌数量: " + cardDatabase.Count);
        
        // 记录数据库中的卡牌信息
        for (int i = 0; i < Mathf.Min(cardDatabase.Count, 5); i++) // 只记录前5张卡牌以避免日志过长
        {
            CardData card = cardDatabase[i];
            DebugLogger.Log("数据库卡牌[" + i + "]: 名称='" + card.cardName + "', 类型=" + card.cardType + ", 费用=" + card.cost + ", 精灵存在=" + (card.cardSprite != null));
        }
    }
}