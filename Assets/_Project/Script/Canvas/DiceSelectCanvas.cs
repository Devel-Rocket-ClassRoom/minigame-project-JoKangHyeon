using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceSelectCanvas : MonoBehaviour
{
    public GameManager manager;

    public List<DiceObject> dicePreviews;
    public List<DiceSelectButton> diceSelectors;

    public DiceSelectButton diceSelectorPrefab;
    public Transform diceSelectorParent;

    public Transform diceObjectParent;

    public GameObject OverlayUI;

    public void StartDiceSelect(GameManager gameManager, Action<Dice> callback, bool isCycle = false)
    {
        OverlayUI.SetActive(false);

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

            if (isCycle)
            {
                diceSelectors[i].Show(gameManager, gameManager.currentRun.currentRound.currentCycle.dices[i], callback, this);
            }
            else
            {
                diceSelectors[i].Show(gameManager, gameManager.currentRun.dices[i], callback, this);
            }
            diceSelectors[i].gameObject.SetActive(true);
        }

        this.gameObject.SetActive(true);
    }

    public void FinishDiceSelect()
    {
        foreach(DiceSelectButton diceSelector in diceSelectors)
        {
            diceSelector.Remove3dDice();
        }

        OverlayUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
