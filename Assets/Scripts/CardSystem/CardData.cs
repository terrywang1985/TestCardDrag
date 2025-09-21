using UnityEngine;

/// <summary>
/// 卡牌数据类，存储卡牌的基本信息
/// </summary>
[CreateAssetMenu(fileName = "NewCard", menuName = "Card System/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;            // 卡牌名称
    public Sprite cardSprite;          // 卡牌精灵图
    public string description;         // 卡牌描述
    public int cost;                   // 卡牌费用
    public int attack;                 // 攻击力
    public int health;                 // 生命值
    public CardType cardType;          // 卡牌类型
}

/// <summary>
/// 卡牌类型枚举
/// </summary>
public enum CardType
{
    Minion,        // 随从
    Spell,         // 法术
    Weapon,        // 武器
    HeroPower      // 英雄技能
}