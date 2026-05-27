using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FaceView : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI faceText;
    DiceFace face;

    public void Set(DiceFace face)
    {
        this.face = face;
        faceText.text = face.Value.ToString();
    }
}
