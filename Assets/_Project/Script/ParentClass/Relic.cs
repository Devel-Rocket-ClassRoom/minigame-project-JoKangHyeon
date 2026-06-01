using System;
using UnityEngine;

[Serializable]
public abstract class Relic
{
    public string nameStringKey;
    public string Name=>StringTable.GetString(nameStringKey);

    public string descriptionStringKey;
    public string Description => StringTable.GetString(descriptionStringKey);
    public string flavorTextStringKey;
    public string FlavorText => StringTable.GetString(flavorTextStringKey);
    public Sprite sprite;
    public Defines.Rarity rarity;
    public int cost;
    public Defines.RelicCategory category;


    #region 우선순위 상수
    /// <summary>
    /// 첫 굴림시 우선순위
    /// </summary>
    public const int c_priorityFirstRollEffect = 20;
    /// <summary>
    /// 부서진 룬 우선순위
    /// </summary>
    public const int c_priorityBrokenRune = 30;
    /// <summary>
    /// 금빛 거울 우선순위
    /// </summary>
    public const int c_priorityGoldenMirror = 50;
    /// <summary>
    /// 그 외 기타 일반순위
    /// </summary>
    public const int c_priorityDefault = 100;

    #endregion

    public abstract void OnObtain(GameManager gameManager);
    public abstract void OnRemove();
}