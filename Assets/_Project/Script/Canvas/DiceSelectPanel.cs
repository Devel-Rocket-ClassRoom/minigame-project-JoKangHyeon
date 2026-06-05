using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceSelectPanel : MonoBehaviour
{
    public GameManager gameManager;

    public List<DiceObject> dicePreviews;
    public List<DiceSelectButton> diceSelectors;

    public DiceSelectButton diceSelectorPrefab;
    public Transform diceSelectorParent;

    public Transform diceObjectParent;

    public DiceRenderCamera diceRenderCameraPrefab;
    public List<DiceRenderCamera> diceRenderCameras;

    public void StartDiceSelect(GameManager gameManager, Action<Dice> callback, bool isCycle = false)
    {
        this.gameManager = gameManager;

        foreach(DiceSelectButton button in diceSelectors)
        {
            button.gameObject.SetActive(false);
        }

        for(int i=0; i < gameManager.currentRun.dices.Count; i++)
        {
            if(i >= diceSelectors.Count)
            {
                DiceSelectButton newButton = Instantiate(diceSelectorPrefab,diceSelectorParent);
                diceSelectors.Add(newButton);
            }

            if(i>= diceRenderCameras.Count)
            {
                DiceRenderCamera newCamera = Instantiate(diceRenderCameraPrefab, diceObjectParent);
                newCamera.transform.Translate(-i*100, 0, 0);    
                diceRenderCameras.Add(newCamera);
            }

            if (isCycle)
            {
                diceSelectors[i].Show(gameManager, gameManager.currentRun.currentRound.currentCycle.dices[i], callback, this, diceRenderCameras[i]);
            }
            else
            {
                diceSelectors[i].Show(gameManager, gameManager.currentRun.dices[i], callback, this, diceRenderCameras[i]);
            }
            diceSelectors[i].gameObject.SetActive(true);
        }

        this.gameObject.SetActive(true);
    }

    public void FinishDiceSelect()
    {
        foreach(var dicecamera in diceRenderCameras)
        {
            dicecamera.gameObject.SetActive(false);
        }

        gameManager.tooltip.HideTooltip();
        this.gameObject.SetActive(false);
    }
}
