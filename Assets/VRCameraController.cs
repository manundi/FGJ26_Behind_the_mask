using System;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VRCameraController : MonoBehaviour
{
    public AudioSource breathAudioSource;

    [Header("References")]
    [Tooltip("In VR this reference might not rotate the body, but is kept for compatibility.")]
    public Transform parentBody;

    private bool monsterWasSight = false;
    private float monsterSightTimer = 0.0f;
    public AudioSource audioSource;
    public List<AudioClip> monsterSounds = new List<AudioClip>();
    public List<AudioClip> scaredSounds = new List<AudioClip>();
    private float scaredTimer = 0.0f;
    private float randomScaredTimer = 0.0f;
    private bool scaredWaiting = false;


    private float playerGuideTimer = 0.0f;
    private float playerGuideTarget = 0.0f;
    private float playerGuideTimerDelta = 0.0f;

    // Kept private to match original, likely assigned in editor in original if at all, but code shows it private unassigned.
    [SerializeField]
    private TMP_Text playerGuideObject;

    // How strict the check is (1 = exact, 0 = 90 degrees)
    [Range(-1f, 1f)]
    public float dotThreshold = 0.8f;

    void Start()
    {
        if (parentBody == null)
        {
            parentBody = transform.parent;
        }
    }

    void Update()
    {
        // Mouse Look removed for VR. 
        // Rotation is controlled by the HMD/XR System.

        bool monsterInSightNow;

        Vector3 facing = transform.forward;
        Vector3 targetDirection = Vector3.back;

        float dot = Vector3.Dot(facing.normalized, targetDirection.normalized);

        // Check if camera is facing roughly "back"
        if (dot > dotThreshold)
        {
            monsterInSightNow = true;
            if (Game.instance != null && Game.instance.monster != null)
            {
                Game.instance.monster.monsterInSight = true;
            }
            // Debug.Log("Facing Vector3.back");
        }
        else
        {
            monsterInSightNow = false;
            if (Game.instance != null && Game.instance.monster != null)
            {
                Game.instance.monster.monsterInSight = false;
            }
        }

        if (monsterInSightNow != monsterWasSight)
        {
            // Debug.Log("Monster sight changed: " + monsterInSightNow + " (was " + monsterWasSight + ")");

            monsterSightTimer += Time.deltaTime;
            if (monsterSightTimer >= 0.1f)
            {
                // Debug.Log("Monster timer: " + monsterSightTimer);

                monsterWasSight = monsterInSightNow;
                if (monsterInSightNow == false)
                {
                    // Debug.Log("Monster sight now: " + monsterInSightNow);
                    if (audioSource != null && monsterSounds.Count > 0)
                    {
                        audioSource.PlayOneShot(monsterSounds[UnityEngine.Random.Range(0, monsterSounds.Count)]);
                        audioSource.volume = 0.5f;
                    }
                }
                else
                {
                    if (audioSource != null)
                        audioSource.volume = Math.Max(0.0f, audioSource.volume - Time.deltaTime * 10.0f);
                    // Debug.Log("Monster sight now: " + monsterInSightNow);
                }
            }

            if (monsterInSightNow == false)
            {
                //breathing sounds
                scaredWaiting = true;
                scaredTimer = 0.0f;
                randomScaredTimer = 0.7f + UnityEngine.Random.Range(0.0f, 0.3f);

                playerGuideTimer = 0.0f;
                playerGuideTarget = 0.0f;
                playerGuideTimerDelta = 1.0f;
            }
            else
            {
                playerGuideTimer = 0.0f;
                playerGuideTarget = 1.0f;
                playerGuideTimerDelta = 5.0f;
            }
        }
        else
        {
            monsterSightTimer = 0.0f;
        }

        if (monsterInSightNow)
        {
            scaredWaiting = false;
        }

        if (scaredWaiting)
        {
            scaredTimer += Time.deltaTime;
            if (scaredTimer >= randomScaredTimer)
            {
                if (breathAudioSource != null && scaredSounds.Count > 0)
                {
                    breathAudioSource.PlayOneShot(scaredSounds[UnityEngine.Random.Range(0, scaredSounds.Count)]);
                }
                scaredTimer = 0.0f;
                randomScaredTimer = 2.2f + UnityEngine.Random.Range(0.0f, 1.0f);
            }
        }

        if (playerGuideTimer < 1.0f)
        {
            playerGuideTimer += Time.deltaTime * playerGuideTimerDelta;
            float guideValue = Mathf.Lerp(0.0f, 1.0f, playerGuideTimer);

            if (playerGuideObject)
            {
                float alpha = Mathf.Lerp(playerGuideObject.alpha, playerGuideTarget, guideValue);
                playerGuideObject.alpha = alpha;
            }
        }
    }
}
