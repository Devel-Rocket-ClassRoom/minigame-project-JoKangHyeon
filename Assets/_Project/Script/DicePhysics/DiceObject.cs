using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class DiceObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioClip rollClip;

    public List<GameObject> faces;
    public Rigidbody rb;
    public List<TextMeshPro> diceText;

    private Dice _dice;
    public Dice Dice
    {
        get { return _dice; }
        set { 
            _dice = value;
            rollClip = gameManager.soundDefine.Find(_dice.rollSFXKey);
            audioSource.clip = rollClip;
        }
    }
    GameManager gameManager;
    AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = rollClip;

        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameManager == null) return;
        if (Dice == null) return;
        gameManager.tooltip.ShowDiceTooltip(Dice, Dice.diceResultIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameManager == null) return;
        gameManager.tooltip.HideTooltip();
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
        for (int i = 0; i < faces.Count; i++)
        {
            diceText[i].text = data[(dataIndex + i) % data.Count];
        }
    }

    public void TextSetOffset(int faceIndex)
    {
        List<string> data = Dice.faces.ConvertAll(face => face.Value.ToString());
        //Debug.Log($"FaceIndex: {faceIndex}, DiceResultIndex: {dice.diceResultIndex}");
        TextSet(data, faceIndex, Dice.diceResultIndex);
    }

    public void SetOutline(bool enable)
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.SetFloat("_Size", enable ? 0.005f : 0f);
    }


}
