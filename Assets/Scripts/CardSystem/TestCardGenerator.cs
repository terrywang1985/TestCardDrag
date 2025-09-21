using UnityEngine;
using System.IO;

/// <summary>
/// 测试卡牌生成器，用于创建测试用的卡牌数据
/// </summary>
public class TestCardGenerator : MonoBehaviour
{
    [Header("测试设置")]
    public int numberOfTestCards = 10;  // 生成的测试卡牌数量
    public CardManager cardManager;     // 卡牌管理器引用

    private void Awake()
    {
        DebugLogger.Log("TestCardGenerator Awake: 初始化测试卡牌生成器");
        // 检查是否已经有卡牌数据，如果没有则生成
        if (cardManager != null && cardManager.cardDatabase.Count == 0)
        {
            DebugLogger.Log("TestCardGenerator Awake: 卡牌数据库为空，开始生成测试卡牌");
            GenerateTestCards();
        }
    }

    /// <summary>
    /// 生成测试卡牌数据
    /// </summary>
    [ContextMenu("Generate Test Cards")]
    public void GenerateTestCards()
    {
        DebugLogger.Log("TestCardGenerator: 开始生成测试卡牌数据");
        if (cardManager == null) 
        {
            DebugLogger.LogWarning("TestCardGenerator: CardManager 为空，无法生成测试卡牌");
            return;
        }

        // 清空现有数据库
        DebugLogger.Log("TestCardGenerator: 清空现有卡牌数据库");
        cardManager.cardDatabase.Clear();

        // 生成测试卡牌
        for (int i = 0; i < numberOfTestCards; i++)
        {
            DebugLogger.Log("TestCardGenerator: 生成卡牌 " + (i + 1));
            CardData newCard = CreateTestCard(i);
            cardManager.cardDatabase.Add(newCard);
        }

        DebugLogger.Log("TestCardGenerator: 已生成 " + numberOfTestCards + " 张测试卡牌");
        
        // 调用CardManager的方法记录数据库初始化信息
        if (cardManager != null)
        {
            DebugLogger.Log("TestCardGenerator: 卡牌数据生成完成，调用CardManager.InitializeCardDatabase()");
            cardManager.InitializeCardDatabase();
        }
    }

    /// <summary>
    /// 创建一张测试卡牌
    /// </summary>
    /// <param name="index">卡牌索引</param>
    /// <returns>生成的卡牌数据</returns>
    private CardData CreateTestCard(int index)
    {
        DebugLogger.Log("TestCardGenerator: 创建卡牌 - 索引: " + index);
        // 创建新的卡牌数据
        CardData card = ScriptableObject.CreateInstance<CardData>();
        
        // 设置卡牌基本信息
        card.cardName = "测试卡牌 " + (index + 1);
        DebugLogger.Log("TestCardGenerator: 卡牌名称: " + card.cardName);
        
        card.description = "这是一张测试卡牌，用于演示卡牌系统的功能。\n卡牌效果将在这里显示。";
        card.cost = Random.Range(1, 10);
        DebugLogger.Log("TestCardGenerator: 卡牌费用: " + card.cost);
        
        card.attack = Random.Range(1, 10);
        card.health = Random.Range(1, 10);
        DebugLogger.Log("TestCardGenerator: 卡牌攻击力: " + card.attack + ", 生命值: " + card.health);
        
        // 随机设置卡牌类型
        CardType[] types = { CardType.Minion, CardType.Spell, CardType.Weapon, CardType.HeroPower };
        card.cardType = types[Random.Range(0, types.Length)];
        DebugLogger.Log("TestCardGenerator: 卡牌类型: " + card.cardType);

        // 尝试加载对应的测试精灵
        string spritePath = "CardSprites/Test";
        
        switch (card.cardType)
        {
            case CardType.Minion:
                spritePath += "Minion";
                break;
            case CardType.Spell:
                spritePath += "Spell";
                break;
            case CardType.Weapon:
                spritePath += "Weapon";
                break;
            case CardType.HeroPower:
                spritePath += "HeroPower";
                break;
            default:
                spritePath += "Default";
                break;
        }
        
        // 尝试加载指定的测试精灵
        DebugLogger.Log("TestCardGenerator: 尝试加载精灵 - 路径: " + spritePath);
        Sprite testSprite = Resources.Load<Sprite>(spritePath);
        string loadedFrom = spritePath;
        
        // 如果指定的测试精灵不存在，尝试加载通用测试精灵
        if (testSprite == null)
        {
            DebugLogger.LogWarning("TestCardGenerator: 未找到类型特定的精灵，尝试加载默认精灵");
            spritePath = "CardSprites/TestCard";
            loadedFrom = spritePath;
            testSprite = Resources.Load<Sprite>(spritePath);
        }
        
        // 设置卡牌精灵
        card.cardSprite = testSprite;
        DebugLogger.Log("TestCardGenerator: 精灵加载" + (testSprite != null ? "成功" : "失败") + ", 路径: " + loadedFrom);

        return card;
    }
}