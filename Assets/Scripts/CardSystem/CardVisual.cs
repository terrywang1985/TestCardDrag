using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 卡牌视觉组件，负责显示卡牌的各种视觉元素
/// </summary>
public class CardVisual : MonoBehaviour
{
    [Header("UI组件")]
    public Image cardBackground;       // 卡牌背景
    public Image cardImage;            // 卡牌图片
    public Text cardNameText;          // 卡牌名称文本
    public Text descriptionText;       // 卡牌描述文本
    public Text costText;              // 费用文本
    public Text attackText;            // 攻击力文本
    public Text healthText;            // 生命值文本

    // 卡牌数据引用
    private CardData cardData;

    /// <summary>
    /// 设置卡牌数据并更新视觉效果
    /// </summary>
    /// <param name="data">卡牌数据</param>
    public void SetCardData(CardData data)
    {
        cardData = data;
        UpdateVisuals();
    }

    /// <summary>
    /// 更新卡牌的视觉表现
    /// </summary>
    private void UpdateVisuals()
    {
        if (cardData == null) return;

        // 更新卡牌基本信息
        if (cardImage != null)
        {
            if (cardData.cardSprite != null)
            {
                cardImage.sprite = cardData.cardSprite;
            }
            else
            {
                // 尝试加载测试精灵
                LoadTestSprite();
            }
        }

        if (cardNameText != null)
        {
            cardNameText.text = cardData.cardName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = cardData.description;
        }

        if (costText != null)
        {
            costText.text = cardData.cost.ToString();
        }

        if (attackText != null)
        {
            attackText.text = cardData.attack.ToString();
        }

        if (healthText != null)
        {
            healthText.text = cardData.health.ToString();
        }

        // 根据卡牌类型设置不同的背景颜色
        if (cardBackground != null)
        {
            switch (cardData.cardType)
            {
                case CardType.Minion:
                    cardBackground.color = new Color(0.9f, 0.9f, 0.9f, 1f); // 随从卡牌白色背景
                    break;
                case CardType.Spell:
                    cardBackground.color = new Color(0.7f, 0.9f, 1f, 1f); // 法术卡牌蓝色背景
                    break;
                case CardType.Weapon:
                    cardBackground.color = new Color(0.9f, 0.8f, 0.6f, 1f); // 武器卡牌棕色背景
                    break;
                case CardType.HeroPower:
                    cardBackground.color = new Color(0.9f, 0.6f, 0.9f, 1f); // 英雄技能紫色背景
                    break;
                default:
                    cardBackground.color = Color.white;
                    break;
            }
        }
    }

    /// <summary>
    /// 加载测试精灵
    /// </summary>
    private void LoadTestSprite()
    {
        // 根据卡牌类型加载对应的测试精灵
        string spritePath = "CardSprites/Test";
        
        switch (cardData.cardType)
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
        Sprite testSprite = Resources.Load<Sprite>(spritePath);
        
        // 如果指定的测试精灵不存在，尝试加载通用测试精灵
        if (testSprite == null)
        {
            testSprite = Resources.Load<Sprite>("CardSprites/TestCard");
        }
        
        // 如果找到了测试精灵，应用到卡牌
        if (testSprite != null)
        {
            cardImage.sprite = testSprite;
        }
        else
        {
            // 如果没有任何测试精灵，至少显示卡牌名称
            Debug.LogWarning("没有找到测试精灵，请使用Card Game Tools > Generate Test Sprites来创建测试精灵");
        }
    }
}