using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceObject : MonoBehaviour
{
    public List<GameObject> faces;
    public Rigidbody rb;
    public List<TextMeshPro> diceText;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        int dataIndex = valueIndex - faceIndex - 1 + data.Count;
        for (int i = 0; i < 6; i++)
        {
            diceText[i].text = data[(dataIndex + i) % data.Count];
        }
    }
}

