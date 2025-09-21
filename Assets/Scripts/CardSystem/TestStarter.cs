using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestStarter : MonoBehaviour
{
    public Button startTestButton; // 测试启动按钮
    public GameManager gameManager; // 游戏管理器引用
    public Text statusText; // 状态文本显示

    private void Awake()
    {
        DebugLogger.Log("TestStarter Awake: 初始化测试启动器");
    }

    private void Start()
    {
        DebugLogger.Log("TestStarter Start: 设置测试启动器");
        
        // 查找GameManager
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                DebugLogger.LogError("TestStarter: 未找到GameManager组件");
                if (statusText != null)
                {
                    statusText.text = "错误: 未找到GameManager组件";
                }
            }
            else
            {
                DebugLogger.Log("TestStarter: 已找到GameManager组件");
            }
        }
        
        // 设置按钮事件
        if (startTestButton != null)
        {
            startTestButton.onClick.AddListener(StartTest);
            DebugLogger.Log("TestStarter: 已设置测试启动按钮事件");
        }
        else
        {
            DebugLogger.LogError("TestStarter: 未设置startTestButton引用");
        }
        
        // 更新状态文本
        UpdateStatusText("准备就绪，点击按钮开始测试");
    }

    /// <summary>
    /// 启动测试
    /// </summary>
    public void StartTest()
    {
        DebugLogger.Log("TestStarter: 测试开始");
        UpdateStatusText("测试开始...");
        
        // 禁用按钮防止重复点击
        if (startTestButton != null)
        {
            startTestButton.interactable = false;
        }
        
        // 检查并初始化GameManager
        if (gameManager == null)
        {
            DebugLogger.LogError("TestStarter: 无法启动测试，GameManager为空");
            UpdateStatusText("错误: GameManager为空");
            return;
        }
        
        // 清除之前的卡牌（如果有）
        gameManager.ClearAllCards();
        
        // 重新初始化游戏
        gameManager.InitializeGame();
        
        // 等待卡牌生成和显示
        StartCoroutine(MonitorTestProgress());
    }

    /// <summary>
    /// 监控测试进度
    /// </summary>
    private IEnumerator MonitorTestProgress()
    {
        DebugLogger.Log("TestStarter: 开始监控测试进度");
        
        // 等待3秒让游戏初始化
        yield return new WaitForSeconds(3f);
        
        // 检查手牌区域是否有卡牌
        CardManager cardManager = FindObjectOfType<CardManager>();
        if (cardManager != null)
        {
            Transform handArea = cardManager.handArea;
            if (handArea != null)
            {
                int childCount = handArea.childCount;
                DebugLogger.Log("TestStarter: 测试完成，手牌区域子对象数量: " + childCount);
                
                if (childCount > 0)
                {
                    UpdateStatusText("测试成功！已生成 " + childCount + " 张卡牌\n日志已保存到项目根目录下的CardGameDebugLog.txt");
                }
                else
                {
                    UpdateStatusText("测试完成，但未显示卡牌\n请查看项目根目录下的CardGameDebugLog.txt获取详细日志");
                }
            }
            else
            {
                DebugLogger.LogError("TestStarter: 手牌区域为空");
                UpdateStatusText("测试完成，但手牌区域为空\n请查看项目根目录下的CardGameDebugLog.txt获取详细日志");
            }
        }
        else
        {
            DebugLogger.LogError("TestStarter: 未找到CardManager");
            UpdateStatusText("测试完成，但未找到CardManager\n请查看项目根目录下的CardGameDebugLog.txt获取详细日志");
        }
        
        // 测试完成，保存日志
        DebugLogger.SaveLog();
        DebugLogger.Log("TestStarter: 测试结束，日志已保存");
    }

    /// <summary>
    /// 更新状态文本
    /// </summary>
    /// <param name="message">要显示的消息</param>
    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    /// <summary>
    /// 重置测试
    /// </summary>
    public void ResetTest()
    {
        DebugLogger.Log("TestStarter: 重置测试");
        
        // 启用按钮
        if (startTestButton != null)
        {
            startTestButton.interactable = true;
        }
        
        // 清除所有卡牌
        if (gameManager != null)
        {
            gameManager.ClearAllCards();
        }
        
        UpdateStatusText("准备就绪，点击按钮重新开始测试");
    }

    /// <summary>
    /// 保存日志到文件
    /// </summary>
    public void SaveLogNow()
    {
        DebugLogger.SaveLog();
        DebugLogger.Log("TestStarter: 手动保存日志");
        UpdateStatusText("日志已保存到项目根目录下的CardGameDebugLog.txt");
    }
}