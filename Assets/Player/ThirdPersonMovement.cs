using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    private PlayerInputActions playerInputActions;
    private Vector2 movement;
    
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    private Vector3 velocity;
    private bool isGrounded;
    // equation to calculate jump height
    // v = sqrt(h * -2 * g)
    //h-height g-gravity

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    private void Awake() {
        playerInputActions = new PlayerInputActions();

        // Movement Tracking
        playerInputActions.Player.Move.performed += context => movement = context.ReadValue<Vector2>();
        playerInputActions.Player.Move.canceled += context => movement = Vector2.zero;

        // Jump Tracking
        playerInputActions.Player.Jump.performed += executeJump;

    }

    private void executeJump(InputAction.CallbackContext context) {
        Debug.Log(context.ReadValue<float>());
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight*-2f*gravity);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //making sure gravity doesn't keep counting once we touched the ground
        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        float horizontal = movement.x;
        float vertical = movement.y;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Debug.Log("X" + movement.x);
        Debug.Log("Y" + movement.y);

        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        //applying gravity
        // y = (1/2)gravity * time^2
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnEnable() {
        playerInputActions.Player.Enable();
    }
    void OnDisable() {
        playerInputActions.Player.Disable();
    }
}