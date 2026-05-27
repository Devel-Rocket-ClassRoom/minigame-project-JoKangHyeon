using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Tooltip tooltip;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }
}
