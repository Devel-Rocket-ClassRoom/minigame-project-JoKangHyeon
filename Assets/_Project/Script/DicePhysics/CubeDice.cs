using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CubeDice : MonoBehaviour
{
    public List<Quaternion> faceQuaternion;
    public List<TextMeshPro> diceText;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(Roll(2));
    }

    public void TextSet(List<string> data)
    {
        for (int i = 0; i < 6; i++)
        {
            diceText[i].text = data[i];
        }
    }

    public IEnumerator Roll(int face)
    {
        rb.linearVelocity = Random.insideUnitSphere * 3f;
        rb.angularVelocity = Random.insideUnitSphere * 10f;

        yield return new WaitUntil(() => rb.IsSleeping());

        rb.isKinematic = true;
        transform.rotation = faceQuaternion[face];
    }
}
