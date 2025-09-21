using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 测试精灵生成器，用于快速创建测试卡牌所需的图片资源
/// </summary>
public class TestSpriteGenerator : MonoBehaviour
{
    [Header("精灵设置")]
    public int width = 300;          // 卡牌宽度（像素）
    public int height = 400;         // 卡牌高度（像素）
    public string spriteName = "TestCard";
    public Color backgroundColor = new Color(0.7f, 0.7f, 1f); // 浅蓝色背景

#if UNITY_EDITOR
    [MenuItem("Card Game Tools/Generate Test Sprites")]
    public static void GenerateTestSprites()
    {
        // 确保Resources目录存在
        string spriteFolderPath = "Assets/Resources/CardSprites";
        if (!Directory.Exists(spriteFolderPath))
        {
            Directory.CreateDirectory(spriteFolderPath);
        }

        // 创建5种不同颜色的测试卡牌
        Color[] colors = {
            new Color(0.7f, 0.7f, 1f),   // 浅蓝色
            new Color(1f, 0.7f, 0.7f),   // 浅红色
            new Color(0.7f, 1f, 0.7f),   // 浅绿色
            new Color(1f, 1f, 0.7f),   // 浅黄色
            new Color(0.9f, 0.7f, 1f)    // 浅紫色
        };

        // 创建不同类型的卡牌背景
        string[] typeNames = {"Minion", "Spell", "Weapon", "HeroPower", "Default"};
        
        for (int i = 0; i < typeNames.Length; i++)
        {
            CreateTestSprite(
                spriteFolderPath + "/Test" + typeNames[i] + ".png", 
                300, 400, 
                colors[i % colors.Length], 
                typeNames[i]
            );
        }

        // 刷新资源数据库
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("测试精灵创建完成", 
            "已成功创建5张测试卡牌精灵图片！\n\n路径: Assets/Resources/CardSprites/", 
            "确定");
    }

    /// <summary>
    /// 创建一个测试精灵图
    /// </summary>
    /// <param name="filePath">文件保存路径</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="backgroundColor">背景颜色</param>
    /// <param name="label">标签文本</param>
    private static void CreateTestSprite(string filePath, int width, int height, Color backgroundColor, string label)
    {
        // 创建纹理
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        // 填充背景色
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, backgroundColor);
            }
        }
        
        // 添加简单的边框
        int borderSize = 10;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < borderSize || x >= width - borderSize || y < borderSize || y >= height - borderSize)
                {
                    texture.SetPixel(x, y, Color.black);
                }
            }
        }
        
        // 应用像素更改
        texture.Apply();
        
        // 保存为PNG文件
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        
        // 释放纹理资源
        Object.DestroyImmediate(texture);
        
        // 设置导入设置
        string assetPath = filePath.Replace("Assets/", "");
        TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }
    }
#endif
}