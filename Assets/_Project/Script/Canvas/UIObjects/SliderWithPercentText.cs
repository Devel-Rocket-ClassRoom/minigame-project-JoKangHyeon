using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderWithPercentText : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    Slider slider;


    private void Awake()
    {
        slider= GetComponent<Slider>();

        slider.onValueChanged.AddListener(OnValueChaged);
    }

    private void OnValueChaged(float value)
    {
        valueText.text = string.Format("{0:P0}", value);
    }
}
