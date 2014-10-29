﻿using UnityEngine;

[AddComponentMenu("RPG/Character/Player/Motor")]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(RPGAnimator))]
public class RPGPlayerMotor : MonoBehaviour
{

    public float walkSpeed = 6.0f;
    public float runSpeed = 11.0f;
    public float BackwardSpeed = 2f;
    public float SwimmingSpeed = 4;
    public bool limitDiagonalSpeed = true;
    public bool toggleRun = true;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float fallingDamageThreshold = 10.0f;
    public bool slideWhenOverSlopeLimit = true;
    public bool slideOnTaggedObjects = true;
    public float slideSpeed = 3.0f;
    public int SlideSlope = 45;
    public bool airControl = false;
    public float antiBumpFactor = .75f;
    public int antiBunnyHopFactor = 1;

    public string ForwardAxis = "Vertical";
    public string StrafeAxis = "Horizontal";
    public string RunAxis = "Run";
    //public string JumpAxis = "Jump";
    public string SlidingTag = "Slide";

    public bool IsSwimming { get; set; }
    public bool InCliffRange { get; set; }
    public bool InCliffAnimation { get; set; }
    public bool DoCliffAnimation { get; set; }
    public Vector3 MoveDirection { get; set; }

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;
    private int jumpTimer;
    private bool running;

    public static RPGPlayerMotor Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = SlideSlope;
        jumpTimer = antiBunnyHopFactor;
    }

    private void FixedUpdate()
    {
        float inputY = Input.GetAxis(ForwardAxis);
        float inputX = Input.GetAxis(StrafeAxis);
        // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

        if (grounded || IsSwimming)
        {
            bool sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }

            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (falling)
            {
                falling = false;
                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                    FallingDamageAlert(fallStartLevel - myTransform.position.y);
            }

            // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
            if (!toggleRun)
                speed = Input.GetButton(RunAxis) ? runSpeed : walkSpeed;

            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ((sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == SlidingTag))
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else
            {
                moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                playerControl = true;
            }

            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!Input.GetKeyDown(PlayerInput.Instance.Jump))
                jumpTimer++;
            else if (jumpTimer >= antiBunnyHopFactor && !DoCliffAnimation)
            {
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
            }
        }
        else
        {
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!falling && !IsSwimming && !InCliffAnimation)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }

            // If air control is allowed, check movement but don't touch the y component
            if (airControl && playerControl && !InCliffAnimation)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }

        // Apply gravity
        if (!IsSwimming && !InCliffAnimation)
        {
            moveDirection.y -= gravity * Time.deltaTime;

            // Move the controller, and set grounded true or false depending on whether we're standing on something

            grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }

        else if (!InCliffAnimation && IsSwimming)
        {
            moveDirection.y = 0;
            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    void Update()
    {
        // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
        // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
        speed = running ? runSpeed : walkSpeed;
        if (toggleRun && grounded && Input.GetButtonDown(RunAxis))
        {
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);
            running = !running;
        }
        if (Input.GetAxis(ForwardAxis) < 0)
            speed = BackwardSpeed;
        if (IsSwimming)
            speed = SwimmingSpeed;
        RPGAnimator.Instance.WalkSpeed = Input.GetAxis(ForwardAxis) * speed;
        RPGAnimator.Instance.StrafeSpeed = Input.GetAxis(StrafeAxis);
        MoveDirection = moveDirection;
    }

    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
    }

    // If falling damage occurred, this is the place to do something about it. You can make the player
    // have hit points and remove some of them based on the distance fallen, add sound effects, etc.
    static void FallingDamageAlert(float fallDistance)
    {
        print("Ouch! Fell " + fallDistance + " units!");
    }

    public void ResetMoveVector()
    {
        moveDirection = Vector3.zero;
        if (IsSwimming)
            speed = SwimmingSpeed;
    }
}