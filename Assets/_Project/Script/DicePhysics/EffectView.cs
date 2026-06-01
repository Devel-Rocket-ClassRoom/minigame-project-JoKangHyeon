using UnityEngine;

public struct EffectView
{
    public string name;           // StringTable 키
    public string description;    // StringTable 키
    public Sprite sprite;
    public bool isPermanent;      // false = 사이클 시작 시 제거
    public int targetFaceValue;   // 면 단위 효과: 해당 면 값. 전체 주사위 효과: -1
}