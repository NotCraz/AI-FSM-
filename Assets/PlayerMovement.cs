using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float crouchSpeed = 2f;
    public float groundDrag = 5f;

    [Header("Crouch Settings")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float standHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchTransitionSpeed = 6f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;

    private bool isCrouching = false;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private Rigidbody rb;
    private CapsuleCollider capsule;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        capsule = GetComponent<CapsuleCollider>();
        if (capsule)
        {
            capsule.height = standHeight;
            capsule.center = new Vector3(0, standHeight / 2f, 0);
        }
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        HandleInput();
        HandleDrag();
        HandleCrouch();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(crouchKey))
            isCrouching = true;

        if (Input.GetKeyUp(crouchKey))
            isCrouching = false;
    }

    private void HandleCrouch()
    {
        if (!capsule) return;

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        capsule.height = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        capsule.center = new Vector3(0, capsule.height / 2f, 0);
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
    }

    private void HandleDrag()
    {
        rb.linearDamping = grounded ? groundDrag : 0f;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }
}
