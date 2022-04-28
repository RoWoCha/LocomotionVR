using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Rendering.PostProcessing;

public class VRController : MonoBehaviour
{
    [Header("General Data")]
    public float minHeight = 1.0f;
    public float maxHeight = 2.0f;
    public float gravityStrength = 350.0f;

    CharacterController characterController = null;
    Transform cameraRig = null;
    Transform head = null;

    [Header("Movement")]
    public float maxSpeed = 2.0f;
    public float joystickSensitivity = 0.1f;
    public SteamVR_Action_Vector2 joystickDir = null;

    [HideInInspector]
    public float currentSpeed = 0.0f;

    [Header("Snap Turn")]
    public float snapTurnRotation = 45.0f;
    public SteamVR_Action_Boolean snapTurnPressedLeft = null;
    public SteamVR_Action_Boolean snapTurnPressedRight = null;

    [Header("Vignette Data")]
    public BlindersVR blindersVR = null;

    private void Awake()
    {
        // Initialize blinders data
        blindersVR.playerController = this;
    }

    void Start()
    {
        // Initialize
        characterController = GetComponent<CharacterController>();
        cameraRig = SteamVR_Render.Top().origin;
        head = SteamVR_Render.Top().head;
    }

    void Update()
    {
        UpdateHeight();
        UpdateMovement();
        CheckSnapRotation();
    }

    void UpdateHeight()
    {
        // Updating height
        float headHeight = Mathf.Clamp(head.localPosition.y, minHeight, maxHeight);
        characterController.height = headHeight;

        // Updating collider's center
        Vector3 newCenter = head.localPosition;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;
        characterController.center = newCenter;
    }

    void UpdateMovement()
    {
        // Get movement direction
        Quaternion movementDir = CalculateMovementDir();

        // If joystick is not used, stop movement
        if (joystickDir.GetAxis(SteamVR_Input_Sources.LeftHand).magnitude == 0)
            currentSpeed = 0.0f;

        // Calculate speed
        currentSpeed += joystickDir.GetAxis(SteamVR_Input_Sources.LeftHand).magnitude * joystickSensitivity;
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Create velocity vector
        Vector3 movementVec = movementDir * (currentSpeed * Vector3.forward);

        // Apply gravity
        movementVec.y -= gravityStrength * Time.deltaTime;

        // Apply movement
        characterController.Move(movementVec * Time.deltaTime);
    }

    Quaternion CalculateMovementDir()
    {
        // Get joystick's direction
        float rotation = Mathf.Atan2(joystickDir.GetAxis(SteamVR_Input_Sources.LeftHand).x, joystickDir.GetAxis(SteamVR_Input_Sources.LeftHand).y);
        rotation *= Mathf.Rad2Deg;

        // Find player's movement direction
        Vector3 moveDirEuler = new Vector3(0.0f, head.eulerAngles.y + rotation, 0.0f);
        return (Quaternion.Euler(moveDirEuler));
    }

    void CheckSnapRotation()
    {
        float snapValue = 0.0f;

        // Get input
        if (snapTurnPressedLeft.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            snapValue = -Mathf.Abs(snapTurnRotation);
            blindersVR.OnSnapTurn();
        }
        else if (snapTurnPressedRight.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            snapValue = Mathf.Abs(snapTurnRotation);
            blindersVR.OnSnapTurn();
        }
        // Rotate
        transform.RotateAround(head.position, Vector3.up, snapValue);
    }
}
