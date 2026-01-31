using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Mouse Settings")]
    [Tooltip("Sensitivity of the mouse look.")]
    public float mouseSensitivity = 10f;

    [Tooltip("Maximum angle to look up (in degrees).")]
    public float maxLookUpAngle = 90f;

    [Tooltip("Maximum angle to look down (in degrees).")]
    public float maxLookDownAngle = 90f;

    [Header("References")]
    [Tooltip("The parent object to rotate horizontally (e.g., the Player body or Head). If empty, will attempt to use the direct parent.")]
    public Transform parentBody;

    private float xRotation = 0f;
    // Multiplier to make the sensitivity value feel more normalized (e.g. 1-10 range)
    private const float sensitivityMultiplier = 0.1f;

    void Start()
    {
        // Lock and hide the cursor for FPS controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // If no parentBody is assigned, try to rely on the Transform's parent
        if (parentBody == null)
        {
            parentBody = transform.parent;
        }

        if (parentBody == null)
        {
            Debug.LogWarning("CameraController: No parent body found! Please assign one or child this camera to a player object.");
        }
    }

    void Update()
    {
        // Ensure mouse is present
        if (Mouse.current == null) return;

        // Read mouse delta from the Input System
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Calculate rotation step
        // We do not multiply by Time.deltaTime because mouse delta is already per-frame displacement
        float mouseX = mouseDelta.x * mouseSensitivity * sensitivityMultiplier;
        float mouseY = mouseDelta.y * mouseSensitivity * sensitivityMultiplier;

        // Pitch (Vertical Rotation) -> Rotates the Camera up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookDownAngle, maxLookUpAngle);

        // Apply pitch to the Camera (this object)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yaw (Horizontal Rotation) -> Rotates the Parent Body (Head/Player)
        if (parentBody != null)
        {
            parentBody.Rotate(Vector3.up * mouseX);
        }

        if (Math.Abs(parentBody.rotation.w) < 0.4f)
        {
            Game.instance.monster.monsterInSight = true;
        }
        else
        {
            Game.instance.monster.monsterInSight = false;
        }
    }
}
