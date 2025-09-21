using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 卡牌拖拽处理器，负责卡牌的拖拽逻辑
/// </summary>
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拖拽设置")]
    public Transform handArea;          // 手牌区域
    public Transform tableArea;         // 桌面区域
    public float dragOffset = 20f;      // 拖拽时卡牌相对于鼠标的偏移量
    public float dragScale = 1.2f;      // 拖拽时卡牌的缩放比例

    private Transform originalParent;   // 原始父物体
    private Vector2 originalAnchoredPosition; // 原始锚点位置
    private Vector3 originalScale;      // 原始缩放比例
    private Vector2 dragStartOffset;    // 拖拽开始时的偏移量
    private bool isDragging = false;    // 是否正在拖拽
    private CanvasGroup canvasGroup;    // CanvasGroup组件，用于设置透明度
    private RectTransform cardRect;     // 卡牌的RectTransform
    private Canvas mainCanvas;          // 主Canvas
    private Camera mainCamera;          // 主摄像机

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        cardRect = GetComponent<RectTransform>();
        if (cardRect == null)
        {
            Debug.LogError("卡牌缺少RectTransform组件");
        }
        
        // 查找主Canvas
        mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null)
        {
            Debug.LogError("场景中未找到Canvas");
        }
        
        // 查找主摄像机
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("未找到主摄像机，尝试使用Canvas的渲染相机");
        }
    }

    private void Start()
    {
        // 确保卡牌在正确的层级和位置
        EnsureCorrectHierarchy();
    }

    /// <summary>
    /// 确保卡牌在正确的层级结构中
    /// </summary>
    private void EnsureCorrectHierarchy()
    {
        if (transform.parent == null || (transform.parent != handArea && transform.parent != tableArea))
        {
            Debug.LogWarning("卡牌" + gameObject.name + "不在正确的父层级中，将移入手牌区域");
            transform.SetParent(handArea, false);
            
            if (cardRect != null)
            {
                // 重置位置和旋转等属性
                cardRect.localPosition = Vector3.zero;
                cardRect.localScale = Vector3.one;
                cardRect.localRotation = Quaternion.identity;
                cardRect.anchorMin = new Vector2(0.5f, 0.5f);
                cardRect.anchorMax = new Vector2(0.5f, 0.5f);
                cardRect.pivot = new Vector2(0.5f, 0.5f);
            }
        }
    }

    /// <summary>
    /// 开始拖拽时的处理
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cardRect == null)
        {
            Debug.LogError("无法拖拽卡牌，缺少RectTransform组件");
            return;
        }
        
        // 保存原始状态
        originalParent = transform.parent;
        originalAnchoredPosition = cardRect.anchoredPosition;
        originalScale = transform.localScale; // 保存原始缩放比例
        isDragging = true;

        // 设置CanvasGroup属性
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f; // 稍微降低透明度

        // 应用拖拽时的缩放，使卡牌变大
        transform.localScale = originalScale * dragScale;

        // 将卡牌移动到最上层
        transform.SetAsLastSibling();
        
        // 记录开始拖拽时的鼠标位置
        Debug.Log("开始拖拽卡牌: " + gameObject.name + "，原始位置: " + originalAnchoredPosition + "，鼠标位置: " + eventData.position);
    }

    /// <summary>
    /// 拖拽过程中的处理
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || cardRect == null)
            return;

        // 计算卡牌应该跟随鼠标的位置
        Vector2 localPoint;
        RectTransform parentRect = cardRect.parent.GetComponent<RectTransform>();
        
        // 将屏幕坐标转换为父物体的本地坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, null, out localPoint))
        {
            cardRect.anchoredPosition = localPoint;
        }
    }

    /// <summary>
    /// 结束拖拽时的处理
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // 记录结束拖拽时的鼠标位置
        Debug.Log("结束拖拽卡牌: " + gameObject.name + "，鼠标位置: " + eventData.position);

        // 检查是否拖拽到了桌面区域
        if (IsOverTableArea(eventData))
        {
            // 获取桌面区域的RectTransform
            RectTransform tableRect = tableArea.GetComponent<RectTransform>();
            
            // 移动到桌面区域
            transform.SetParent(tableArea, false);
            transform.localScale = Vector3.one;
            
            // 设置卡牌在桌面区域的位置
            if (cardRect != null)
            {
                // 确保卡牌的锚点设置正确
                cardRect.anchorMin = new Vector2(0.5f, 0.5f);
                cardRect.anchorMax = new Vector2(0.5f, 0.5f);
                cardRect.pivot = new Vector2(0.5f, 0.5f);
                
                // 将屏幕坐标转换为桌面区域的本地坐标
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(tableRect, eventData.position, null, out localPoint))
                {
                    cardRect.anchoredPosition = localPoint;
                    Debug.Log("卡牌放置在桌面区域，位置: " + localPoint);
                }
            }
            
            // 卡牌放置到桌面上的逻辑
            OnCardPlacedOnTable();
        }
        else
        {
            // 返回原始位置和缩放
            transform.SetParent(originalParent, false);
            if (cardRect != null)
            {
                cardRect.anchoredPosition = originalAnchoredPosition;
            }
            transform.localScale = originalScale; // 恢复原始缩放比例
        }
        
        // 确保卡牌在正确的层级
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// 检查是否拖拽到了桌面区域
    /// </summary>
    /// <param name="eventData">事件数据</param>
    /// <returns>是否在桌面区域上</returns>
    private bool IsOverTableArea(PointerEventData eventData)
    {
        if (tableArea == null)
        {
            Debug.LogWarning("桌面区域未设置");
            return false;
        }

        // 检查鼠标是否在桌面区域的矩形内
        RectTransform tableRect = tableArea.GetComponent<RectTransform>();
        if (tableRect == null)
        {
            Debug.LogWarning("桌面区域缺少RectTransform组件");
            return false;
        }

        // 使用RectTransformUtility.RectangleContainsScreenPoint直接检查屏幕坐标是否在桌面区域内
        return RectTransformUtility.RectangleContainsScreenPoint(tableRect, eventData.position, null);
    }

    /// <summary>
    /// 卡牌被放置在桌面上时的处理
    /// </summary>
    private void OnCardPlacedOnTable()
    {
        Debug.Log("卡牌被放置在桌面上: " + gameObject.name);
    }
}