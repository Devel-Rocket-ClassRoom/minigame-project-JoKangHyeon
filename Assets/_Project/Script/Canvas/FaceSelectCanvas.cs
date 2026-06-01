using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FaceSelectCanvas : MonoBehaviour
{
    public Transform dice3dParent;

    public List<FaceView> faceViews;
    public FaceView faceViewPrefab;
    public Transform faceViewParent;

    MouseSpinDice dice3D;
    Dice diceData;
    Action<Dice,int> callback; 
    GameManager gameManager;

    int diceIndex = -1;

    private void Update()
    {
        if (dice3D == null)
            return;

        if(diceIndex != dice3D.selectedFace)
        {
            diceIndex = dice3D.selectedFace;

            foreach (FaceView faceView in faceViews)
            {
                faceView.background.color = Color.white;
            }

            if(diceIndex != -1)
            {
                faceViews[diceIndex].background.color = Defines.colorGold;
            }
        }
    }

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void StartFaceSelect(Dice dice, Action<Dice,int> callback)
    {
        diceData = dice;
        this.callback = callback;

        foreach (FaceView faceView in faceViews)
        {
            faceView.gameObject.SetActive(false);
        }

        for(int i=0; i < dice.faces.Count; i++)
        {
            if (i >= faceViews.Count)
            {
                FaceView newFaceView = Instantiate(faceViewPrefab,faceViewParent);
                faceViews.Add(newFaceView);
                int index = i;
                Button button = faceViews[i].GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    SelectFace(index);
                });
            }

            faceViews[i].Set(gameManager.tooltip, dice.faces[i], i);
            faceViews[i].gameObject.SetActive(true);
        }

        DiceObject newDice3D = Instantiate(diceData.prefab,dice3dParent);
        newDice3D.rb.useGravity = false;
        newDice3D.rb.isKinematic = true;
        dice3D = newDice3D.AddComponent<MouseSpinDice>();
        SelectFace(0);

        this.gameObject.SetActive(true);
    }

    public void SelectFace(int index)
    {
        dice3D.SetFaceShown(index);
        dice3D.forceFaceChanged = true;
    }

    public void EndSelect()
    {
        callback?.Invoke(diceData, dice3D.selectedFace);
        Destroy(dice3D.gameObject);
        this.gameObject.SetActive(false);   
    }
}
