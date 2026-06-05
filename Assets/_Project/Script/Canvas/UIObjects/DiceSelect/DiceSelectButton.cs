using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceSelectButton : MonoBehaviour   
{
    public FaceView faceViewPrefab;
    public Transform faceViewParent;

    public TextMeshProUGUI diceNameText;

    public RawImage diceImage;

    GameManager gameManager;
    List<FaceView> faceViews = new();
    DiceSelectPanel parentCanvas;
    Button button;
    Action<Dice> callback;
    Dice dice;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    public void Show(GameManager gameManager, Dice dice, Action<Dice> callback, DiceSelectPanel parentCanvas, DiceRenderCamera diceRenderCamera)
    {
        this.gameManager = gameManager;
        this.parentCanvas = parentCanvas;
        this.callback = callback;
        this.dice = dice;

        diceRenderCamera.gameObject.SetActive(true);
        RenderTexture texture = diceRenderCamera.Render(dice);
        diceImage.texture = texture;

        diceNameText.text = dice.Name;

        foreach (FaceView faceView in faceViews)
        {
            faceView?.gameObject.SetActive(false);
        }

        for (int i = 0; i < dice.faces.Count; i++)
        {
            if(i>= faceViews.Count)
            {
                FaceView newFaceView = Instantiate(faceViewPrefab, faceViewParent);
                faceViews.Add(newFaceView);
            }

            faceViews[i].Set(gameManager.tooltip, dice.faces[i], i);
            faceViews[i].gameObject.SetActive(true);
        }
    }

    public void OnButtonClicked()
    {
        callback?.Invoke(dice);
        parentCanvas.FinishDiceSelect();
    }
}