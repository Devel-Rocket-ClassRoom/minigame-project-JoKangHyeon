using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopRarityDefinition")]
public class ShopRarityDefinitionSO : ScriptableObject
{
    public List<Defines.Rarity> rarity;
    public List<float> raritySpread;
}