using TMPro;
using UnityEngine;

public class GameOverCanvas : MonoBehaviour
{
    public TextMeshProUGUI clearText;
    public TextMeshProUGUI detailsText;

    public TextMeshProUGUI restartButtonText;
    public TextMeshProUGUI exitButtonText;

    const string c_clearStringKey = "game_cleared";
    const string c_failStringKey = "game_failed";
    const string c_restartStringKey = "game_restart";
    const string c_exitStringKey = "game_exit";

    const string c_detailsStringKey = "game_details";

    public void Show(GameManager gameManager, bool isCleared)
    {
        if (isCleared)
        {
            clearText.text = StringTable.GetString(c_clearStringKey);
        }
        else
        {
            clearText.text = StringTable.GetString(c_failStringKey);
        }
        restartButtonText.text = StringTable.GetString(c_restartStringKey);
        exitButtonText.text = StringTable.GetString(c_exitStringKey);

        int lastStage = gameManager.currentRun.level;
        int maxStage = 15; //DEMO
        string score = gameManager.currentRun.currentScore.ToString();
        if(lastStage != maxStage)
        {
            score += $"/{gameManager.currentRun.TargetScore}";
        }
        int coinLeft = gameManager.currentRun.Coin;
        int relicCount = gameManager.currentRun.relics.Count;
        int cardCount = gameManager.currentRun.cards.Count;

        detailsText.text = string.Format(StringTable.GetString(c_detailsStringKey), 
            lastStage.ToString(), maxStage.ToString(), score, coinLeft.ToString(), relicCount.ToString(), cardCount.ToString());

        gameObject.SetActive(true);
    }
}
