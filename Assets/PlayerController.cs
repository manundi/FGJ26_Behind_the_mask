
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    Vector2 playerTargetPos;
    Rigidbody rb;
    private InputAction moveAction;
    private Vector2 lastInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize target position to current position to avoid snapping to (0,0)
        playerTargetPos = new Vector2(transform.position.x, transform.position.z);
        rb = GetComponent<Rigidbody>();
        // Get the PlayerInput component and the "Move" action
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
        }
        else
        {
            Debug.LogError("PlayerInput component is missing on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAction != null)
        {
            Vector2 currentInput = moveAction.ReadValue<Vector2>();

            // Handle discrete movement logic
            // We check if the input crossed the threshold this frame

            // X Axis (A/D or Left/Right)
            if (currentInput.x > 0.5f && lastInput.x <= 0.5f)
            {
                playerTargetPos.x -= 1f;
            }
            else if (currentInput.x < -0.5f && lastInput.x >= -0.5f)
            {
                playerTargetPos.x += 1f;
            }


            else if (currentInput.y < -0.5f && lastInput.y >= -0.5f)
            {
                playerTargetPos.y += 1f;
            }

            lastInput = currentInput;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(playerTargetPos.x, transform.position.y, playerTargetPos.y), Time.deltaTime * 10f);
        // Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f);
        // if (!hit.transform.gameObject.CompareTag("Ground"))
        // {
        //     rb.useGravity = true;
        // }
        // if (transform.position.y < -5f)
        // {
        //     Game.instance.RestartGame();
        // }
    }



    void Move(Vector2 direction)
    {

    }
}
