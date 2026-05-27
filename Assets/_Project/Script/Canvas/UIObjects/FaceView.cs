using TMPro;
using UnityEngine;

public class FaceView : MonoBehaviour
{
    public TextMeshProUGUI faceText;
    DiceFace face;

    public void Set(DiceFace face)
    {
        this.face = face;
        faceText.text = face.Value.ToString();
    }
}
