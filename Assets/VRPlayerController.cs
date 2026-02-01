using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRPlayerController : MonoBehaviour
{
    public GameObject bloodEffectPrefab;
    public Transform bloodSpawnPoint;
    Vector2 playerTargetPos;
    Rigidbody rb;

    [Header("Input Settings")]
    [Tooltip("Drag the Left or Right Controller Move Action here")]
    public InputActionReference moveActionReference;
    
    // In case user wants to use PlayerInput component instead
    [Tooltip("Optional: If using PlayerInput, the action name to read.")]
    public string playerInputActionName = "Move";

    private Vector2 lastInput;

    public AudioSource audioSource;
    public List<AudioClip> moveSounds = new List<AudioClip>();
    public List<AudioClip> dropSounds = new List<AudioClip>();
    public List<AudioClip> deathSounds = new List<AudioClip>();

    void Start()
    {
        // Initialize target position to current position to avoid snapping to (0,0)
        playerTargetPos = new Vector2(transform.position.x, transform.position.z);
        rb = GetComponent<Rigidbody>();
        
        if (moveActionReference != null)
        {
            moveActionReference.action.Enable();
        }
    }

    void OnDestroy()
    {
        if (moveActionReference != null)
        {
            moveActionReference.action.Disable();
        }
    }

    void Update()
    {
        bool moved = false;
        
        // Check monster sight requirement
        if (Game.instance != null && Game.instance.monster != null)
        {
            if (!Game.instance.monster.monsterInSight) return;
        }

        Vector2 currentInput = Vector2.zero;

        // Try reading from Reference first
        if (moveActionReference != null)
        {
            currentInput = moveActionReference.action.ReadValue<Vector2>();
        }
        // Fallback to PlayerInput component search if reference missing
        else
        {
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null && !string.IsNullOrEmpty(playerInputActionName))
            {
                currentInput = playerInput.actions[playerInputActionName].ReadValue<Vector2>();
            }
        }

        // Discrete movement logic
        // X Axis
        if (currentInput.x > 0.5f && lastInput.x <= 0.5f)
        {
            moved = true;
            playerTargetPos.x -= 1f; // Inverted: Right Input -> Left Move
        }
        else if (currentInput.x < -0.5f && lastInput.x >= -0.5f)
        {
            moved = true;
            playerTargetPos.x += 1f; // Inverted: Left Input -> Right Move
        }
        // Y Axis
        else if (currentInput.y < -0.5f && lastInput.y >= -0.5f)
        {
            if (audioSource != null && moveSounds.Count > 0)
            {
                audioSource.PlayOneShot(moveSounds[UnityEngine.Random.Range(0, moveSounds.Count)]);
            }
            moved = true;
            playerTargetPos.y += 1f; // Down Input -> Positive Z (Forward/Back)

            if (Game.instance != null && Game.instance.levelCreator != null)
                Game.instance.levelCreator.UpdatePlayerPosition((int)Math.Floor(playerTargetPos.y));
        }

        lastInput = currentInput;

        if (moved)
        {
            if (Game.instance != null && Game.instance.levelCreator != null)
            {
                if (Game.instance.levelCreator.occupied.Contains(new Vector2Int((int)Math.Floor(playerTargetPos.x), (int)Math.Floor(playerTargetPos.y))))
                {
                    if (audioSource != null && dropSounds.Count > 0)
                    {
                        audioSource.PlayOneShot(dropSounds[UnityEngine.Random.Range(0, dropSounds.Count)]);
                    }
                }
            }
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(playerTargetPos.x, transform.position.y, playerTargetPos.y), Time.deltaTime * 10f);

        if (transform.position.y < -4f)
        {
            if (Game.instance != null)
            {
                Game.instance.InvokeDie();
                // Disable this controller
                
                // If we also had a reference to Game.instance.playerController, we might need to update that if Game assumes a singleton
            }

            if (bloodEffectPrefab != null && bloodSpawnPoint != null)
                Instantiate(bloodEffectPrefab, bloodSpawnPoint.position, quaternion.identity);
            
            this.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Monster")
        {
            if (audioSource != null && deathSounds.Count > 0)
            {
                audioSource.PlayOneShot(deathSounds[UnityEngine.Random.Range(0, deathSounds.Count)]);
            }

            if (Game.instance != null)
                Game.instance.InvokeDie();
        }
    }
}
