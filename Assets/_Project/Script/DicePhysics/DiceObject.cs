using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<GameObject> faces;
    public Rigidbody rb;
    public List<TextMeshPro> diceText;

    public Dice dice;

    GameManager manager;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        manager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    public int GetFace()
    {
        int maxIndex = -1;
        float maxDot = -255f;
        for (int i = 0; i < faces.Count; i++)
        {
            float dot = Vector3.Dot(faces[i].transform.up, Vector3.up);
            if (dot > maxDot)
            {
                maxDot = dot;
                maxIndex = i;
            }
        }
        return maxIndex + 1;
    }

    public void TextSet(List<string> data, int faceIndex, int valueIndex)
    {
        int dataIndex = valueIndex - faceIndex + data.Count;
        for (int i = 0; i < 6; i++)
        {
            diceText[i].text = data[(dataIndex + i) % data.Count];
        }
    }

    public void TextSetOffset(int faceIndex)
    {
        List<string> data = dice.faces.ConvertAll(face => face.Value.ToString());
        //Debug.Log($"FaceIndex: {faceIndex}, DiceResultIndex: {dice.diceResultIndex}");
        TextSet(data, faceIndex, dice.diceResultIndex);
    }

    public void SetOutline(bool enable)
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.SetFloat("_Size", enable ? 0.005f : 0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manager == null) return;
        if (dice == null) return;
        manager.tooltip.ShowDiceTooltip(dice, dice.diceResultIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(manager == null ) return;
        manager.tooltip.HideTooltip();
    }
}
