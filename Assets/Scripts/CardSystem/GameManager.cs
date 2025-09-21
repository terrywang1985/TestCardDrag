using UnityEngine;

/// <summary>
/// 游戏管理器，负责整个游戏的流程控制
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("游戏组件")]
    public CardManager cardManager;     // 卡牌管理器
    public GameObject gameCanvas;       // 游戏UI画布
    public Transform handArea;          // 手牌区域
    public Transform tableArea;         // 桌面区域

    private void Awake()
    {
        // DebugLogger will be initialized automatically when first accessed
    }

    private void Start()
    {
        DebugLogger.Log("游戏管理器开始初始化");
        // 初始化游戏
        InitializeGame();
    }

    /// <summary>
    /// 初始化游戏设置
    /// </summary>
    public void InitializeGame()
    {
        // 设置屏幕为竖屏
        Screen.orientation = ScreenOrientation.Portrait;
        DebugLogger.Log("设置屏幕方向为竖屏");
        
        // 确保游戏画布和区域设置正确
        if (gameCanvas == null)
        {
            DebugLogger.LogWarning("游戏画布未设置，尝试查找");
            gameCanvas = GameObject.Find("GameCanvas");
            if (gameCanvas != null)
            {
                DebugLogger.Log("已找到游戏画布: " + gameCanvas.name);
            }
            else
            {
                DebugLogger.LogError("未找到游戏画布！");
            }
        }
        else
        {
            DebugLogger.Log("游戏画布已设置: " + gameCanvas.name);
        }

        if (handArea == null)
        {
            DebugLogger.LogWarning("手牌区域未设置，尝试查找");
            handArea = GameObject.Find("HandArea")?.transform;
            if (handArea != null)
            {
                DebugLogger.Log("已找到手牌区域: " + handArea.name);
                LogHandAreaInfo();
            }
            else
            {
                DebugLogger.LogError("未找到手牌区域！");
            }
        }
        else
        {
            DebugLogger.Log("手牌区域已设置: " + handArea.name);
            LogHandAreaInfo();
        }

        if (tableArea == null)
        {
            DebugLogger.LogWarning("桌面区域未设置，尝试查找");
            tableArea = GameObject.Find("TableArea")?.transform;
            if (tableArea != null)
            {
                DebugLogger.Log("已找到桌面区域: " + tableArea.name);
                LogTableAreaInfo();
            }
            else
            {
                DebugLogger.LogError("未找到桌面区域！");
            }
        }
        else
        {
            DebugLogger.Log("桌面区域已设置: " + tableArea.name);
            LogTableAreaInfo();
        }

        // 初始化卡牌管理器
        if (cardManager != null)
        {
            DebugLogger.Log("卡牌管理器已找到: " + cardManager.name);
            cardManager.handArea = handArea;
            cardManager.tableArea = tableArea;
            DebugLogger.Log("已将手牌区域和桌面区域引用传递给卡牌管理器");
        }
        else
        {
            DebugLogger.LogError("卡牌管理器未设置！");
        }

        DebugLogger.Log("游戏初始化完成，当前屏幕分辨率: " + Screen.width + "x" + Screen.height);
    }

    /// <summary>
    /// 记录手牌区域的详细信息
    /// </summary>
    private void LogHandAreaInfo()
    {
        if (handArea == null)
            return;
        
        RectTransform handRect = handArea.GetComponent<RectTransform>();
        if (handRect != null)
        {
            DebugLogger.Log("手牌区域RectTransform信息: 位置=" + handRect.anchoredPosition + ", 大小=" + handRect.rect.width + "x" + handRect.rect.height + ", 锚点Min=" + handRect.anchorMin + ", 锚点Max=" + handRect.anchorMax + ", 中心点=" + handRect.pivot);
        }
        else
        {
            DebugLogger.LogWarning("手牌区域缺少RectTransform组件");
        }
        
        DebugLogger.Log("手牌区域子对象数量: " + handArea.childCount);
    }

    /// <summary>
    /// 记录桌面区域的详细信息
    /// </summary>
    private void LogTableAreaInfo()
    {
        if (tableArea == null)
            return;
        
        RectTransform tableRect = tableArea.GetComponent<RectTransform>();
        if (tableRect != null)
        {
            DebugLogger.Log("桌面区域RectTransform信息: 位置=" + tableRect.anchoredPosition + ", 大小=" + tableRect.rect.width + "x" + tableRect.rect.height + ", 锚点Min=" + tableRect.anchorMin + ", 锚点Max=" + tableRect.anchorMax + ", 中心点=" + tableRect.pivot);
        }
        else
        {
            DebugLogger.LogWarning("桌面区域缺少RectTransform组件");
        }
        
        DebugLogger.Log("桌面区域子对象数量: " + tableArea.childCount);
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        // 清除当前场景中的所有卡牌
        ClearAllCards();
        
        // 重新初始化游戏
        InitializeGame();
    }

    /// <summary>
    /// 清除所有卡牌
    /// </summary>
    public void ClearAllCards()
    {
        // 清除桌面上的卡牌
        if (tableArea != null)
        {
            for (int i = tableArea.childCount - 1; i >= 0; i--)
            {
                Destroy(tableArea.GetChild(i).gameObject);
            }
        }

        // 重置卡牌管理器
        if (cardManager != null)
        {
            // 这里可以添加重置卡牌管理器的逻辑
        }
    }
}