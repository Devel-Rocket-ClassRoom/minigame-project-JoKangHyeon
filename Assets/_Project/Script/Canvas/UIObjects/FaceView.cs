using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FaceView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image background;
    public TextMeshProUGUI faceText;

    int index;
    DiceFace face;
    Tooltip tooltip;

    public void Set(Tooltip tooltip, DiceFace face, int index)
    {
        this.tooltip = tooltip;
        this.face = face;
        this.index = index;
        faceText.text = face.Value.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowDiceTooltip(face.dice, index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }

}
