using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI costText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(Card card, float multiplier)
    {
        cardImage.sprite = card.sprite;
        costText.text = Mathf.RoundToInt((card.cost * multiplier)).ToString();
    }
}
