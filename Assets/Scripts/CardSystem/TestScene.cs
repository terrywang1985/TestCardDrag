using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 测试场景脚本，提供简单的游戏控制功能
/// </summary>
public class TestScene : MonoBehaviour
{
    private void Update()
    {
        // 按R键重新开始游戏
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        // 按ESC键退出游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 简单的调试信息显示
    /// </summary>
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 30), "按 R 键重新开始游戏");
        GUI.Label(new Rect(10, 40, 300, 30), "按 ESC 键退出游戏");
    }
}