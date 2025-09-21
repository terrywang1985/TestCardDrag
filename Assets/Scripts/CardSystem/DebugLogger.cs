using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;

/// <summary>
/// 调试日志记录器，用于详细记录卡牌游戏的运行状态和问题
/// </summary>
public class DebugLogger : MonoBehaviour
{
    private static DebugLogger instance;
    private StringBuilder logBuilder = new StringBuilder();
    private string logFilePath;
    private float logUpdateInterval = 1.0f; // 每秒更新一次日志文件
    private float lastLogUpdateTime = 0;

    public static DebugLogger Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject loggerObj = new GameObject("DebugLogger");
                instance = loggerObj.AddComponent<DebugLogger>();
                DontDestroyOnLoad(loggerObj);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 设置日志文件路径到项目根目录
        logFilePath = Path.Combine(Application.dataPath, "..", "CardGameDebugLog.txt");

        // 清除旧日志
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        Log("======= 卡牌游戏调试日志 =======");
        Log("启动时间: " + System.DateTime.Now.ToString());
        Log("Unity版本: " + Application.unityVersion);
        Log("平台: " + Application.platform);
        Log("屏幕分辨率: " + Screen.width + "x" + Screen.height);
    }

    private void Update()
    {
        // 定期更新日志文件
        if (Time.time - lastLogUpdateTime > logUpdateInterval)
        {
            lastLogUpdateTime = Time.time;
            SaveLogToFile();
        }
    }

    private void OnApplicationQuit()
    {
        Log("应用程序退出时间: " + System.DateTime.Now.ToString());
        Log("======= 日志结束 =======");
        SaveLogToFile();
    }

    /// <summary>
    /// 记录日志信息
    /// </summary>
    /// <param name="message">日志消息</param>
    public static void Log(string message)
    {
        Debug.Log("[DebugLogger] " + message);
        Instance.logBuilder.AppendLine("[" + System.DateTime.Now.ToString("HH:mm:ss.fff") + "] " + message);
    }

    /// <summary>
    /// 记录警告信息
    /// </summary>
    /// <param name="message">警告消息</param>
    public static void LogWarning(string message)
    {
        Debug.LogWarning("[DebugLogger] " + message);
        Instance.logBuilder.AppendLine("[WARN] [" + System.DateTime.Now.ToString("HH:mm:ss.fff") + "] " + message);
    }

    /// <summary>
    /// 记录错误信息
    /// </summary>
    /// <param name="message">错误消息</param>
    public static void LogError(string message)
    {
        Debug.LogError("[DebugLogger] " + message);
        Instance.logBuilder.AppendLine("[ERROR] [" + System.DateTime.Now.ToString("HH:mm:ss.fff") + "] " + message);
    }

    /// <summary>
    /// 保存日志到文件
    /// </summary>
    private void SaveLogToFile()
    {
        try
        {
            File.WriteAllText(logFilePath, logBuilder.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError("无法写入日志文件: " + e.Message);
        }
    }
    
    /// <summary>
    /// 保存日志（公开方法）
    /// </summary>
    public static void SaveLog()
    {
        Instance.SaveLogToFile();
    }
}