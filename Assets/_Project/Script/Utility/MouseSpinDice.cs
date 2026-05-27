using NUnit.Framework.Internal;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseSpinDice : MonoBehaviour
{
    InputAction mouseDelta;
    InputAction click;

    Quaternion targetRotate;

    DiceObject diceObject;
    Camera cam;

    public int selectedFace = -1;
    public float alignSpeed = 420f;

    public bool forceFaceChanged = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        mouseDelta = InputSystem.actions.FindAction("Look");
        click = InputSystem.actions.FindAction("Click");

        diceObject = GetComponent<DiceObject>();

        cam = Camera.main;
    }

    public void SetFaceShown(int face)
    {
        this.selectedFace = face;
        Vector3 targetDir = -cam.transform.forward;

        // 현재 월드에서의 면 법선 / 글씨 up
        Vector3 currentFaceUp = diceObject.faces[selectedFace].transform.up;
        Vector3 currentTextUp = diceObject.diceText[selectedFace].transform.up;

        // 원하는 월드에서의 면 법선 / 글씨 up
        Vector3 desiredFaceUp = targetDir;                    // 카메라를 향함
        Vector3 desiredTextUp = Vector3.ProjectOnPlane(cam.transform.up, desiredFaceUp);
        if (desiredTextUp.sqrMagnitude < 1e-6f)               // 카메라가 면 정면을 정확히 보면 발생 (드뭄)
            desiredTextUp = Vector3.ProjectOnPlane(cam.transform.right, desiredFaceUp);
        desiredTextUp.Normalize();

        // (글씨 up, 면 법선) 페어를 (원하는 글씨 up, 원하는 면 법선)로 매핑하는 회전
        Quaternion currentFrame = Quaternion.LookRotation(currentTextUp, currentFaceUp);
        Quaternion desiredFrame = Quaternion.LookRotation(desiredTextUp, desiredFaceUp);
        targetRotate = desiredFrame * Quaternion.Inverse(currentFrame) * transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (click.IsPressed())
        {
            Vector2 value = mouseDelta.ReadValue<Vector2>();
            (value.x, value.y) = (value.y, -value.x);
            transform.Rotate(value, Space.World);
            return;
        }

        if (click.WasReleasedThisFrame() && !forceFaceChanged)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 targetDir = -cam.transform.forward;

            int selectedFace = -1;
            float maxDot = float.MinValue;

            for (int i = 0; i < diceObject.faces.Count; i++)
            {
                GameObject face = diceObject.faces[i];
                float dot = Vector3.Dot(face.transform.up, targetDir);

                if (dot > maxDot)
                {
                    selectedFace = i;
                    maxDot = dot;
                }
            }

            if (selectedFace != -1)
            {
                SetFaceShown(selectedFace);
            }
        }

        forceFaceChanged = false;

        if (selectedFace != -1)
        {
            this.transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotate,Time.deltaTime*alignSpeed);
        }
    }

}
