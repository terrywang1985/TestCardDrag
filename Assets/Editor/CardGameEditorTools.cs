using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

/// <summary>
/// 卡牌游戏编辑器工具，提供便捷的场景设置功能
/// </summary>
public class CardGameEditorTools : EditorWindow
{
    [MenuItem("Card Game Tools/Setup Card Game Scene")]
    public static void SetupCardGameScene()
    {
        // 检查当前场景
        SceneSetup();
        
        // 添加引导程序
        AddBootstrapper();
        
        // 保存场景
        SaveCurrentScene();
        
        EditorUtility.DisplayDialog("场景设置完成", "卡牌游戏场景已成功设置！\n\n点击Play按钮即可运行游戏。", "确定");
    }

    [MenuItem("Card Game Tools/Create Test Card Data")]
    public static void CreateTestCardData()
    {
        // 确保CardData文件夹存在
        string folderPath = "Assets/Resources/CardData";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        // 创建5张测试卡牌
        for (int i = 0; i < 5; i++)
        {
            CreateTestCard(i + 1);
        }
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("测试卡牌创建完成", "已创建5张测试卡牌数据！", "确定");
    }

    /// <summary>
    /// 设置场景
    /// </summary>
    private static void SceneSetup()
    {
        // 设置相机为正交模式
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5;
        }
        
        // 删除默认的灯光（如果有）
        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light light in lights)
        {
            DestroyImmediate(light.gameObject);
        }
    }

    /// <summary>
    /// 添加引导程序
    /// </summary>
    private static void AddBootstrapper()
    {
        // 检查是否已经有引导程序
        Bootstrapper existingBootstrapper = FindObjectOfType<Bootstrapper>();
        if (existingBootstrapper == null)
        {
            // 创建引导程序对象
            GameObject bootstrapperObj = new GameObject("Bootstrapper");
            Bootstrapper bootstrapper = bootstrapperObj.AddComponent<Bootstrapper>();
            bootstrapper.autoInitialize = true;
        }
    }

    /// <summary>
    /// 保存当前场景
    /// </summary>
    private static void SaveCurrentScene()
    {
        // 如果场景尚未保存，提示用户保存
        if (string.IsNullOrEmpty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path))
        {
            bool save = EditorUtility.DisplayDialog("保存场景", "场景尚未保存，是否保存？", "保存", "取消");
            if (save)
            {
                string defaultPath = "Assets/Scenes/CardGame.unity";
                string path = EditorUtility.SaveFilePanelInProject("保存场景", "CardGame", "unity", "请为场景命名");
                if (!string.IsNullOrEmpty(path))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), path);
                }
            }
        }
        else
        {
            // 保存当前场景
            UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
    }

    /// <summary>
    /// 创建一张测试卡牌
    /// </summary>
    /// <param name="index">卡牌索引</param>
    private static void CreateTestCard(int index)
    {
        // 创建卡牌数据
        CardData cardData = ScriptableObject.CreateInstance<CardData>();
        cardData.cardName = "测试卡牌 " + index;
        cardData.description = "这是第" + index + "张测试卡牌，用于演示卡牌系统功能。";
        cardData.cost = index;
        cardData.attack = index * 2;
        cardData.health = index * 2;
        
        // 随机设置卡牌类型
        CardType[] types = { CardType.Minion, CardType.Spell, CardType.Weapon, CardType.HeroPower };
        cardData.cardType = types[index % types.Length];
        
        // 保存卡牌数据
        string path = "Assets/Resources/CardData/TestCard" + index + ".asset";
        AssetDatabase.CreateAsset(cardData, path);
    }

    [MenuItem("Card Game Tools/Open Documentation")]
    public static void OpenDocumentation()
    {
        string readmePath = "Assets/README.md";
        if (File.Exists(Application.dataPath + "/../README.md"))
        {
            Application.OpenURL("file://" + Application.dataPath + "/../README.md");
        }
        else
        {
            EditorUtility.DisplayDialog("文档不存在", "找不到README.md文档文件。", "确定");
        }
    }

    [MenuItem("Card Game Tools/Create Card Prefab")]
    public static void CreateCardPrefab()
    {
        // 确保Prefabs文件夹存在
        string prefabFolderPath = "Assets/Prefabs";
        if (!Directory.Exists(prefabFolderPath))
        {
            Directory.CreateDirectory(prefabFolderPath);
        }

        // 创建卡牌预制体
        string prefabPath = "Assets/Prefabs/CardPrefab.prefab";
        
        // 如果预制体已存在，询问是否覆盖
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            if (!EditorUtility.DisplayDialog("预制体已存在", "CardPrefab.prefab 已存在，是否覆盖？", "覆盖", "取消"))
            {
                return;
            }
        }

        // 打开文件选择对话框让用户选择背景图片
        string[] imageExtensions = { "png", "jpg", "jpeg", "bmp", "tga" };
        string backgroundImagePath = EditorUtility.OpenFilePanelWithFilters("选择卡牌背景图片", "", new string[] { "图片文件", string.Join(",", imageExtensions) });
        
        // 如果用户取消选择，使用默认背景颜色
        bool useCustomBackground = !string.IsNullOrEmpty(backgroundImagePath);

        // 创建卡牌GameObject
        GameObject cardGO = new GameObject("CardPrefab");
        
        // 添加必要的组件
        RectTransform cardRect = cardGO.AddComponent<RectTransform>();
        CanvasRenderer canvasRenderer = cardGO.AddComponent<CanvasRenderer>();
        CanvasGroup canvasGroup = cardGO.AddComponent<CanvasGroup>();
        CardVisual cardVisual = cardGO.AddComponent<CardVisual>();
        CardDragHandler cardDragHandler = cardGO.AddComponent<CardDragHandler>();

        // 设置RectTransform属性
        cardRect.sizeDelta = new Vector2(100, 150);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);

        // 设置CanvasGroup属性
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // 创建卡牌背景
        GameObject backgroundGO = new GameObject("CardBackground");
        backgroundGO.transform.SetParent(cardGO.transform, false);
        RectTransform backgroundRect = backgroundGO.AddComponent<RectTransform>();
        Image backgroundImage = backgroundGO.AddComponent<Image>();
        backgroundRect.sizeDelta = new Vector2(96, 146);
        backgroundRect.anchoredPosition = Vector2.zero;
        
        // 如果用户选择了背景图片，导入并使用它
        Sprite backgroundSprite = null;
        if (useCustomBackground)
        {
            // 将绝对路径转换为相对路径
            string relativePath = "Assets" + backgroundImagePath.Substring(Application.dataPath.Length);
            
            // 确保图片已导入到项目中
            if (File.Exists(relativePath))
            {
                backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
            }
            
            // 如果图片尚未在项目中，导入它
            if (backgroundSprite == null)
            {
                // 创建Images文件夹
                string imagesFolderPath = "Assets/Images";
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }
                
                // 复制文件到项目中
                string fileName = Path.GetFileName(backgroundImagePath);
                string newFilePath = Path.Combine(imagesFolderPath, fileName);
                
                // 确保文件名唯一
                if (File.Exists(newFilePath))
                {
                    newFilePath = Path.Combine(imagesFolderPath, $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now.Ticks}{Path.GetExtension(fileName)}");
                }
                
                File.Copy(backgroundImagePath, newFilePath);
                AssetDatabase.Refresh();
                
                // 加载图片作为Sprite
                backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(newFilePath);
            }
            
            // 设置背景图片
            if (backgroundSprite != null)
            {
                backgroundImage.sprite = backgroundSprite;
                backgroundImage.type = Image.Type.Sliced;
            }
            else
            {
                backgroundImage.color = Color.white;
            }
        }
        else
        {
            backgroundImage.color = Color.white;
        }

        // 创建卡牌图片
        GameObject imageGO = new GameObject("CardImage");
        imageGO.transform.SetParent(cardGO.transform, false);
        RectTransform imageRect = imageGO.AddComponent<RectTransform>();
        Image cardImage = imageGO.AddComponent<Image>();
        imageRect.sizeDelta = new Vector2(80, 60);
        imageRect.anchoredPosition = new Vector2(0, 15);

        // 创建卡牌名称文本
        GameObject nameTextGO = CreateTextGameObject("CardNameText", cardGO.transform, new Vector2(0, 60), new Vector2(90, 20), TextAnchor.MiddleCenter, 12);
        Text nameText = nameTextGO.GetComponent<Text>();
        nameText.text = "卡牌名称";

        // 创建卡牌描述文本
        GameObject descTextGO = CreateTextGameObject("DescriptionText", cardGO.transform, new Vector2(0, -25), new Vector2(90, 30), TextAnchor.UpperCenter, 10);
        Text descText = descTextGO.GetComponent<Text>();
        descText.text = "卡牌描述";
        descText.raycastTarget = false;

        // 创建费用文本
        GameObject costTextGO = CreateTextGameObject("CostText", cardGO.transform, new Vector2(-40, 65), new Vector2(20, 20), TextAnchor.MiddleCenter, 14);
        Text costText = costTextGO.GetComponent<Text>();
        costText.text = "1";
        costText.color = Color.yellow;

        // 创建攻击力文本
        GameObject attackTextGO = CreateTextGameObject("AttackText", cardGO.transform, new Vector2(-20, -65), new Vector2(20, 20), TextAnchor.MiddleCenter, 14);
        Text attackText = attackTextGO.GetComponent<Text>();
        attackText.text = "2";
        attackText.color = Color.red;

        // 创建生命值文本
        GameObject healthTextGO = CreateTextGameObject("HealthText", cardGO.transform, new Vector2(20, -65), new Vector2(20, 20), TextAnchor.MiddleCenter, 14);
        Text healthText = healthTextGO.GetComponent<Text>();
        healthText.text = "2";
        healthText.color = Color.green;

        // 强制刷新编辑器，确保所有组件都已正确创建
        EditorUtility.SetDirty(cardGO);
        EditorUtility.SetDirty(cardVisual);
        EditorUtility.SetDirty(cardDragHandler);
        
        // 等待编辑器刷新
        EditorApplication.delayCall += () =>
        {
            // 重新获取组件引用，确保它们是最新的
            CardVisual refreshedCardVisual = cardGO.GetComponent<CardVisual>();
            if (refreshedCardVisual != null)
            {
                // 设置CardVisual组件的引用
                refreshedCardVisual.cardBackground = backgroundImage;
                refreshedCardVisual.cardImage = cardImage;
                refreshedCardVisual.cardNameText = nameText;
                refreshedCardVisual.descriptionText = descText;
                refreshedCardVisual.costText = costText;
                refreshedCardVisual.attackText = attackText;
                refreshedCardVisual.healthText = healthText;
                EditorUtility.SetDirty(refreshedCardVisual);
            }

            // 设置CardDragHandler组件的引用
            CardDragHandler refreshedDragHandler = cardGO.GetComponent<CardDragHandler>();
            if (refreshedDragHandler != null)
            {
                refreshedDragHandler.dragOffset = 20f;
                EditorUtility.SetDirty(refreshedDragHandler);
            }

            // 创建预制体
            PrefabUtility.SaveAsPrefabAsset(cardGO, prefabPath);
            
            // 刷新AssetDatabase以确保所有更改都被保存
            AssetDatabase.Refresh();
            
            // 销毁临时GameObject
            DestroyImmediate(cardGO);
            
            EditorUtility.DisplayDialog("卡牌预制体创建完成", "CardPrefab.prefab 已成功创建！\n\n路径: " + prefabPath + "\n\n使用说明:\n1. 在场景中添加CardManager\n2. 将创建的CardPrefab拖到CardManager的cardPrefab字段\n3. 可以使用工具创建的HandAreaPrefab和TableAreaPrefab作为区域对象", "确定");
        };
    }
    
    [MenuItem("Card Game Tools/Create Hand Area Prefab")]
    public static void CreateHandAreaPrefab()
    {
        // 确保Prefabs文件夹存在
        string prefabFolderPath = "Assets/Prefabs";
        if (!Directory.Exists(prefabFolderPath))
        {
            Directory.CreateDirectory(prefabFolderPath);
        }

        // 创建手牌区域预制体
        string prefabPath = "Assets/Prefabs/HandAreaPrefab.prefab";
        
        // 如果预制体已存在，询问是否覆盖
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            if (!EditorUtility.DisplayDialog("预制体已存在", "HandAreaPrefab.prefab 已存在，是否覆盖？", "覆盖", "取消"))
            {
                return;
            }
        }

        // 创建手牌区域GameObject
        GameObject handAreaGO = new GameObject("HandAreaPrefab");
        
        // 添加必要的组件
        RectTransform rectTransform = handAreaGO.AddComponent<RectTransform>();
        CanvasRenderer canvasRenderer = handAreaGO.AddComponent<CanvasRenderer>();
        Image backgroundImage = handAreaGO.AddComponent<Image>();

        // 设置RectTransform属性
        rectTransform.sizeDelta = new Vector2(800, 200);
        rectTransform.pivot = new Vector2(0.5f, 0);
        rectTransform.anchorMin = new Vector2(0.5f, 0);
        rectTransform.anchorMax = new Vector2(0.5f, 0);
        rectTransform.anchoredPosition = new Vector2(0, 100);

        // 设置背景图片属性
        backgroundImage.color = new Color(0, 0, 0, 0.3f);
        backgroundImage.raycastTarget = false;

        // 添加区域名称文本
        GameObject nameTextGO = CreateTextGameObject("HandAreaText", handAreaGO.transform, new Vector2(0, 90), new Vector2(200, 30), TextAnchor.MiddleCenter, 16);
        Text nameText = nameTextGO.GetComponent<Text>();
        nameText.text = "手牌区域";
        nameText.color = Color.white;
        
        // 强制刷新编辑器，确保所有组件都已正确创建
        EditorUtility.SetDirty(handAreaGO);
        
        // 创建预制体
        PrefabUtility.SaveAsPrefabAsset(handAreaGO, prefabPath);
        
        // 刷新AssetDatabase
        AssetDatabase.Refresh();
        
        // 销毁临时GameObject
        DestroyImmediate(handAreaGO);
        
        EditorUtility.DisplayDialog("手牌区域预制体创建完成", "HandAreaPrefab.prefab 已成功创建！\n\n路径: " + prefabPath + "\n\n使用说明:\n1. 将预制体拖入场景\n2. 在CardDragHandler组件中设置Hand Area引用", "确定");
    }
    
    [MenuItem("Card Game Tools/Create Table Area Prefab")]
    public static void CreateTableAreaPrefab()
    {
        // 确保Prefabs文件夹存在
        string prefabFolderPath = "Assets/Prefabs";
        if (!Directory.Exists(prefabFolderPath))
        {
            Directory.CreateDirectory(prefabFolderPath);
        }

        // 创建桌面区域预制体
        string prefabPath = "Assets/Prefabs/TableAreaPrefab.prefab";
        
        // 如果预制体已存在，询问是否覆盖
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            if (!EditorUtility.DisplayDialog("预制体已存在", "TableAreaPrefab.prefab 已存在，是否覆盖？", "覆盖", "取消"))
            {
                return;
            }
        }

        // 创建桌面区域GameObject
        GameObject tableAreaGO = new GameObject("TableAreaPrefab");
        
        // 添加必要的组件
        RectTransform rectTransform = tableAreaGO.AddComponent<RectTransform>();
        CanvasRenderer canvasRenderer = tableAreaGO.AddComponent<CanvasRenderer>();
        Image backgroundImage = tableAreaGO.AddComponent<Image>();

        // 设置RectTransform属性
        rectTransform.sizeDelta = new Vector2(1000, 400);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        // 设置背景图片属性
        backgroundImage.color = new Color(0, 0.5f, 0, 0.2f);
        backgroundImage.raycastTarget = false;

        // 添加区域名称文本
        GameObject nameTextGO = CreateTextGameObject("TableAreaText", tableAreaGO.transform, new Vector2(0, 180), new Vector2(200, 30), TextAnchor.MiddleCenter, 16);
        Text nameText = nameTextGO.GetComponent<Text>();
        nameText.text = "桌面区域";
        nameText.color = Color.white;
        
        // 强制刷新编辑器，确保所有组件都已正确创建
        EditorUtility.SetDirty(tableAreaGO);
        
        // 创建预制体
        PrefabUtility.SaveAsPrefabAsset(tableAreaGO, prefabPath);
        
        // 刷新AssetDatabase
        AssetDatabase.Refresh();
        
        // 销毁临时GameObject
        DestroyImmediate(tableAreaGO);
        
        EditorUtility.DisplayDialog("桌面区域预制体创建完成", "TableAreaPrefab.prefab 已成功创建！\n\n路径: " + prefabPath + "\n\n使用说明:\n1. 将预制体拖入场景\n2. 在CardDragHandler组件中设置Table Area引用", "确定");
    }

    /// <summary>
    /// 创建文本GameObject的辅助方法
    /// </summary>
    private static GameObject CreateTextGameObject(string name, Transform parent, Vector2 anchoredPosition, Vector2 sizeDelta, TextAnchor alignment, int fontSize)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        
        RectTransform rect = textGO.AddComponent<RectTransform>();
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        
        Text text = textGO.AddComponent<Text>();
        text.alignment = alignment;
        text.fontSize = fontSize;
        text.color = Color.black;
        
        // 尝试获取默认字体 - 使用LegacyRuntime.ttf代替Arial.ttf
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont != null)
        {
            text.font = defaultFont;
        }
        else
        {
            // 如果找不到LegacyRuntime.ttf，尝试使用系统默认字体
            text.font = Font.CreateDynamicFontFromOSFont("Arial", fontSize);
        }
        
        return textGO;
    }
}