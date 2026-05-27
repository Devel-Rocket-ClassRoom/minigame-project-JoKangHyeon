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

    public void StartDiceSelect(GameManager gameManager, Action<Dice> callback)
    {
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

            diceSelectors[i].Show(gameManager.currentRun.dices[i], callback, this);
            diceSelectors[i].gameObject.SetActive(true);
        }
    }

    public void FinishDiceSelect()
    {
        foreach(var dice in dicePreviews)
        {
            Destroy(dice.gameObject);
        }

        this.gameObject.SetActive(false);
    }
}
