using UnityEngine;

/// <summary>
/// 游戏引导程序，负责初始化游戏环境
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    [Header("启动设置")]
    public bool autoInitialize = true;  // 是否自动初始化

    private void Awake()
    {
        if (autoInitialize)
        {
            InitializeGame();
        }
    }

    /// <summary>
    /// 初始化游戏环境
    /// </summary>
    private void InitializeGame()
    {
        // 设置屏幕为竖屏
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.fullScreen = false;
        
        // 检查是否已经有场景生成器
        SceneGenerator existingGenerator = FindObjectOfType<SceneGenerator>();
        if (existingGenerator == null)
        {
            // 创建场景生成器对象
            GameObject generatorObj = new GameObject("SceneGenerator");
            SceneGenerator generator = generatorObj.AddComponent<SceneGenerator>();
            
            // 执行场景生成
            generator.GenerateCompleteScene();
        }
        else
        {
            // 如果已存在场景生成器，确保它生成场景
            if (!existingGenerator.generateOnStart)
            {
                existingGenerator.generateOnStart = true;
                existingGenerator.GenerateCompleteScene();
            }
        }
    }
}