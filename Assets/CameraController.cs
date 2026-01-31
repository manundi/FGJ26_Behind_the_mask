using System;
using Unity.Mathematics;
using System.Collections.Generic;
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

    private bool monsterWasSight = false;
    private float monsterSightTimer = 0.0f;
    public AudioSource audioSource;
    public List<AudioClip> monsterSounds = new List<AudioClip>();
          // How strict the check is (1 = exact, 0 = 90 degrees)
    [Range(-1f, 1f)]
    public float dotThreshold = 0.8f;

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

        bool monsterInSightNow;



   
        Vector3 facing = transform.forward;
        Vector3 targetDirection = Vector3.back;

        float dot = Vector3.Dot(facing.normalized, targetDirection.normalized);

        if (dot > dotThreshold)
        {
             monsterInSightNow = true;
            Game.instance.monster.monsterInSight = true;
            Debug.Log("Facing Vector3.back");
        }
       
        else
        {
            monsterInSightNow = false;
            Game.instance.monster.monsterInSight = false;
        }

        if (monsterInSightNow != monsterWasSight)
        {
            Debug.Log("Monster sight changed: " + monsterInSightNow + " (was " + monsterWasSight + ")");

            monsterSightTimer += Time.deltaTime;
            if (monsterSightTimer >= 0.1f)
            {
                Debug.Log("Monster timer: " + monsterSightTimer);

                monsterWasSight = monsterInSightNow;
                if (monsterInSightNow == false)
                {
                    Debug.Log("Monster sight now: " + monsterInSightNow);
                    if (audioSource != null)
                    {
                        audioSource.PlayOneShot(monsterSounds[UnityEngine.Random.Range(0, monsterSounds.Count)]);
                        audioSource.volume = 1.0f;
                    }
                }
                else
                {
                    audioSource.volume = Math.Max(0.0f, audioSource.volume - Time.deltaTime * 10.0f);
                    Debug.Log("Monster sight now: " + monsterInSightNow);
                }
            }
        }
        else
        {
            monsterSightTimer = 0.0f;
        }
    }
}
