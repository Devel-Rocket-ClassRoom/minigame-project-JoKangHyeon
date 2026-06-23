using TMPro;
using UnityEngine;

public class RecordPanel : MonoBehaviour
{

    public TMP_Text nickNameText;
    public TMP_Text clearCountText;
    public TMP_Text playCountText;
    public TMP_Text maxScoreText;

    public void Refresh(GameManager gameManager)
    {
        nickNameText.text = gameManager.personalData.NickName;
        clearCountText.text = gameManager.personalData.ClearCount.ToString();
        playCountText.text = gameManager.personalData.PlayCount.ToString();
        maxScoreText.text = gameManager.personalData.MaxScore.ToString();
    }
}
