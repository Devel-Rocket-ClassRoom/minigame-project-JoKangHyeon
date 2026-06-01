using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectItemView : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public void Set(EffectView effect)
    {
        if (icon != null && effect.sprite != null) icon.sprite = effect.sprite;
        nameText.text = StringTable.GetString(effect.name);

        string desc = StringTable.GetString(effect.description);
        descriptionText.text = effect.targetFaceValue > 0
            ? string.Format(desc, effect.targetFaceValue)
            : desc;
    }
}
