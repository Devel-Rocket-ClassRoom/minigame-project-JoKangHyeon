using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopRelic : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;

    public void SetRelic(Relic relic, float multiplier)
    {
        image.sprite = relic.sprite;
        nameText.text = StringTable.GetString(relic.name);
        costText.text = Mathf.RoundToInt((relic.cost * multiplier)).ToString();
    }
}
