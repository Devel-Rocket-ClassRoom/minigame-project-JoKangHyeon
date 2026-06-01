using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DiceSelectButton : MonoBehaviour   
{
    public GameObject fallowPosition;

    public FaceView faceViewPrefab;
    public Transform faceViewParent;

    public TextMeshProUGUI diceNameText;

    GameManager gameManager;
    List<FaceView> faceViews = new();
    DiceSelectCanvas parentCanvas;
    Button button;
    Action<Dice> callback;
    Dice dice;
    DiceObject dice3D;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    public void Show(GameManager gameManager, Dice dice, Action<Dice> callback, DiceSelectCanvas parentCanvas)
    {
        this.gameManager = gameManager;
        this.parentCanvas = parentCanvas;
        this.callback = callback;
        this.dice = dice;

        dice3D = Instantiate(dice.prefab, parentCanvas.diceObjectParent);
        dice3D.rb.isKinematic = true;
        dice3D.dice = dice;

        FallowObject fallow = dice3D.AddComponent<FallowObject>();
        fallow.target = fallowPosition;

        diceNameText.text = dice.name;

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

    public void Remove3dDice()
    {
        Destroy(dice3D.gameObject);
    }
}