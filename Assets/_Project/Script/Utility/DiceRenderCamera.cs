using Unity.VisualScripting;
using UnityEngine;

public class DiceRenderCamera : MonoBehaviour
{
    public Transform diceSpawnPoint;
    DiceObject currentDice;

    public RenderTexture renderTexture;
    new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        renderTexture = new RenderTexture(256, 256, 16);
        camera.targetTexture = renderTexture;
    }

    public RenderTexture Render(Dice dice)
    {
        if(currentDice!=null)
        {
            Destroy(currentDice.gameObject);
        }

        currentDice = Instantiate(dice.prefab, diceSpawnPoint);
        currentDice.transform.localPosition = Vector3.zero;
        currentDice.Dice = dice;
        currentDice.TextSetOffset(0);
        currentDice.rb.isKinematic = true;
        currentDice.AddComponent<SpinningObject>();

        return renderTexture;
    }
}
