using System;                                   // Basic .NET namespace
using System.Collections;                       // For coroutines
using UnityEngine;                              // Core Unity engine
using UnityEngine.InputSystem;                  // Input System package for handling player inputs

public class PlayerMovement : MonoBehaviour
{
    // === COMPONENT REFERENCES ===
    public Rigidbody2D rb; // Rigidbody for physics-based movement
    public bool isFacingRight = true; // Track direction character is facing
    public Animator animator; // Animator for handling animations
    public ParticleSystem smokeFX; // Particle system for smoke (e.g. landing or flipping)
    public ParticleSystem speedFX; // Particle system for speed effects during boost
    BoxCollider2D playerCollider; // Used to detect and disable collider on platforms
    public Transform visualTransform; //need to assign visual renderer since it was broken out from the main character

    // === INVENTORY CONFIGURATION ===
    [Header("Equipped Weapon")]
    public Weapon currentWeapon;
    public float lastAttackTime = -999f;

    // === MOVEMENT CONFIGURATION ===
    [Header("Movement")]
    public float moveSpeed = 5f; // Base walking speed
    float horizontalMovement; // Value for horizontal movement input
    public float runSpeedMultiplier = 1.5f; // Speed multiplier when running
    public bool isRunning; // Is the player running?
    public float currentSpeed; // Actual calculated movement speed
    public Vector3 positionTracker; // Position tracker independent of flipping
    float speedMultiplier = 1f; // Temporary multiplier (e.g. speed boosts)

    // === JUMP CONFIGURATION ===
    [Header("Jumping")]
    public float jumpPower = 10f; // Force applied on jump
    public int maxJumps = 2; // Max number of jumps (e.g. double jump)
    public int jumpsRemaining; // Jumps currently available
    public bool isJumping; // Track if player is jumping

    // === GROUND CHECK SETTINGS ===
    [Header("GroundCheck")]
    public Transform groundCheckPos; // Position to check for ground below player
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f); // Size of ground check box
    public LayerMask groundLayer; // Layer used to detect ground
    public bool isGrounded; // Is the player on the ground?
    public bool isOnPlatform; // Is player on a drop-through platform?
    public float platformDisableTime = 0.25f; // Time to disable collider for dropping through

    // === WALL CHECK SETTINGS ===
    [Header("WallCheck")]
    public Transform wallCheckPos; // Position to check for wall beside player
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.5f); // Size of wall check box
    public LayerMask wallLayer; // Layer used to detect walls

    // === GRAVITY SETTINGS ===
    [Header("Gravity")]
    public float baseGravity = 2f; // Normal gravity scale
    public float maxFallSpeed = 18f; // Max fall velocity
    public float fallSpeedMultiplier = 2f; // Gravity multiplier when falling

    // === WALL SLIDE ===
    [Header("WallSlide")]
    public float wallSlideSpeed = 2f; // Vertical speed during wall slide
    public bool isWallSliding; // Is the player sliding against wall?

    // === COYOTE TIME ===
    [Header("Coyote Timer")]
    public float coyoteTimer = 0.1f; // How long after leaving ground player can still jump
    private float coyoteTimeCounter; // Tracks remaining coyote time

    // === DASH SETTINGS ===
    [Header("Dashing")]
    public float dashSpeed = 20f; // Speed during dash
    public float dashDuration = 0.2f; // Duration of dash
    public float dashCooldown = 0.5f; // Time between dashes
    bool isDashing; // Is player currently dashing?
    bool canDash = true; // Can player dash?
    TrailRenderer trailRenderer; // Trail effect during dash

    // === WALL JUMP ===
    bool isWallJumping; // Is wall jump active?
    float wallJumpDirection; // Direction to jump off wall
    float wallJumpTime = 0.5f; // Duration of wall jump phase
    float wallJumpTimer; // Timer for wall jump phase
    public Vector2 wallJumpPower = new Vector2(5f, 10f); // Force applied during wall jump

    Vector2 currentVelocity; // Used internally by SmoothDamp
    [Header("Smoothing")]
    [Range(0f, 0.5f)] public float movementSmoothTime = 0.05f; // Tune in Inspector
    public float attackMoveDamp = 0.2f; // 0 = freeze, 1 = full movement (tweak as needed)

    [Header("Melee Attack")]
    public GameObject meleeHitbox;
    public float attackCooldown = 0.5f;
    public bool isAttacking = false;
    private object visaualTransform;

    // START METHOD
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>(); // Cache trail renderer
        playerCollider = GetComponent<BoxCollider2D>(); // Cache player collider
        SpeedItem.OnSpeedCollected += StartSpeedBoost; // Subscribe to speed item event
    }

    void StartSpeedBoost(float multiplier)
    {
        StartCoroutine(speedBoostCoroutine(multiplier)); // Begin coroutine for timed boost
    }

    private IEnumerator speedBoostCoroutine(float multiplier)
    {
        speedMultiplier = multiplier; // Apply speed multiplier
        speedFX.Play(); // Play speed visual effect
        yield return new WaitForSeconds(2f); // Wait for duration
        speedMultiplier = 1f; // Reset speed
        speedFX.Stop(); // Stop visual effect
    }

    void Update()   // Update is called once per frame
    {
        positionTracker = transform.position;   //always updates with the current position
        animator.SetFloat("yVelocity", rb.linearVelocityY); //passes the y-velocity to the animator
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude); //passes the magnitude to the animator
        animator.SetBool("isWallSliding", isWallSliding);


        if (isRunning && Mathf.Abs(horizontalMovement) <= 0.1f)
        {
            // Not moving, so don't run yet
            animator.SetBool("isRunning", false);
        }
        else if (isRunning && Mathf.Abs(horizontalMovement) > 0.1f)
        {
            // Moving while shift is held, start running
            animator.SetBool("isRunning", true);
        }
        else
        {
            // Shift not held or not moving, no running
            animator.SetBool("isRunning", false);
        }

        if (isDashing)
        {
            return;
        }

        Gravity();
        WallSlide();
        HandleWallJumpTimer();

        if (isGrounded)  //time buffer for jumping
        {
            coyoteTimeCounter = coyoteTimer;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }


        // ATTACKING
        if (Input.GetMouseButtonDown(0) && !isAttacking && Time.time >= lastAttackTime + currentWeapon.hitCooldown)        //this handles the player attacking
        {
            isAttacking = true;
            animator.SetTrigger("attack");

            //horizontalMovement = 0f; // Stop horizontal input
            rb.linearVelocity = new Vector2(0f, rb.linearVelocityY); // Stop horizontal movement immediately

            // Reset hitbox to retrigger OnTriggerEnter
            meleeHitbox.SetActive(false);
            meleeHitbox.SetActive(true);

            lastAttackTime = Time.time;
            StartCoroutine(ResetAttackCooldown());
        }

    }



    private void FixedUpdate()                                                                          //This runs at a fixed rate based on Unity's physics engine, not based on FPS
    {
        groundCheck();                                                                                  //I found that running this constantly helps properly reset the jumps remaining counter
        Flip();
        currentSpeed = isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;                          //this is a compact "if else" statement -> if isRunning true ... else moveSpeed
                                                                                                        //this is used to calculate the running speed based on if the run action is pressed or not
        float movementFactor = isAttacking ? attackMoveDamp : 1f;

        if (!isWallJumping)
        {
            Vector2 targetVelocity = new Vector2(
                horizontalMovement * currentSpeed * speedMultiplier * movementFactor,
                rb.linearVelocityY
            );

            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, movementSmoothTime);
        }

        if (isGrounded)
        {
            isJumping = false;                                                                          //reset tracking variable
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }
        //Debug.Log("horizontalMovement: " + horizontalMovement + " | FacingRight: " + isFacingRight);
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public void EnableMeleeHitbox() => meleeHitbox.SetActive(true);
    public void DisableMeleeHitbox() => meleeHitbox.SetActive(false);

    private void WallSlide()
    {
        //Not touching ground, is touching a wall, and x movement != 0
        if (wallCheck() && !isGrounded && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -wallSlideSpeed));    //caps the fall rate
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void HandleWallJumpTimer()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            
            // Determine which side the wall is on
            wallJumpDirection = (wallCheckPos.localPosition.x > 0) ? -1 : 1;                            //makes the character jump in the opposite direction
            wallJumpTimer = wallJumpTime;                                                               //reset wall jump timer

            CancelInvoke(nameof(CancelInvoke));                                                         //as soon as we wall slide, we can jump again
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;                                                            //decrease timer
        }
    }
    private void CancelWallJump()
    {
        isWallJumping = false;
    }
    public void Move(InputAction.CallbackContext context)
    {
        //if (isAttacking) return;                                                                        //Doesn't allow movement when attacking
        horizontalMovement = context.ReadValue<Vector2>().x;                                            //ties horizontal movement variable to the actual x-value of the character
        
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(3, 8, true);                                                     //ignore collision with enemy

        canDash = false;                                                                                //if we can dash
        isDashing = true;                                                                               //if we are currently dashing

        trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocityY);                 //dash movement

        yield return new WaitForSeconds(dashDuration);                                                  //wait for duration of dash

        rb.linearVelocity = new Vector2(0f, rb.linearVelocityY);                                        //reset horizontal velocity

        isDashing = false;
        trailRenderer.emitting = false;

        Physics2D.IgnoreLayerCollision(3, 8, false);                                                     //can collide with enemy

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Run(InputAction.CallbackContext context)                                                //this only tells when the run action is pressed, not if the player is actually running
    {
        if (context.performed)
        {
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }

    private void jumpFX()
    {
        animator.SetTrigger("jump");                                                                    //plays jump animation
        smokeFX.Play();                                                                                 //plays smoke animation
    }
    private void Gravity()
    {
        if (rb.linearVelocityY < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;                                        //fall increasingly faster
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -maxFallSpeed));  //This limits the fall speed to the maximum fall speed
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && isOnPlatform && playerCollider.enabled)                //only can drop when on the right layer, when the action is pressed and when on the ground
                                                                                                      //playerCollider.enabled means this only runs when we're not currently dropping
        {
            //Coroutine drop
            StartCoroutine(DisablePlayerCollider(platformDisableTime));     //starts the disable platform collision routine based on the time we want to disable the collision for 
        }
    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);           //time delay for falling through platform
        playerCollider.enabled = true;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;


        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {

        //Wall Jump
        if (context.performed && wallJumpTimer > 0)
        {
            isJumping = true;                                                                       //just for tracking on inspector
            isWallJumping = true;                                                                   //bool for state of wall jumping
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);  //jump away from the wall
            wallJumpTimer = 0;
            jumpFX();                                                                               //pass jumping action to animator


            // Flip if facing the wrong direction for the wall jump
            if ((isFacingRight && wallJumpDirection < 0) || (!isFacingRight && wallJumpDirection > 0))
            {
                Flip();
            }
     

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);                                    //wall jump will last 0.5 seconds, player can jump again after 0.6 seconds
            return;                                                                                 //skip the rest of the logic

        }
        // === FULL JUMP ===
        if (context.performed)
        {
            bool canUseCoyote = coyoteTimeCounter > 0f && jumpsRemaining == maxJumps;
            bool canJump = isGrounded || canUseCoyote || jumpsRemaining > 0;

            if (canJump)
            {
                isJumping = true;
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);

                // Use a jump regardless of coyote or grounded
                jumpsRemaining--;
                coyoteTimeCounter = 0f;

                jumpFX();
            }
        }

        // === HALF JUMP ===
        else if (context.canceled && rb.linearVelocityY > 0)
        {
            bool canUseCoyote = coyoteTimeCounter > 0f && jumpsRemaining == maxJumps;
            bool canJump = isGrounded || canUseCoyote || jumpsRemaining > 0;

            if (canJump)
            {
                isJumping = true;
                rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);

                // Still consume a jump on half jump
                jumpsRemaining--;
                coyoteTimeCounter = 0f;

                jumpFX();
            }
        }

    }
    private void groundCheck()
    {

        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))  //if the overlap box goes over a ground layer
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;  //Only reset this here
            //Debug.Log("Ground check true");
        }
        else
        {
            isGrounded = false;
            //Debug.Log("Ground check false");
        }
    }
    private bool wallCheck()
    {
        //returns 1 if player is touching something on the wall layer or 0 if it is not
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }
    private void Flip()
    {
        if ((isFacingRight && horizontalMovement < 0) || (!isFacingRight && horizontalMovement > 0))
        {
            isFacingRight = !isFacingRight;

            // Flip the visual sprite
            Vector3 ls = visualTransform.localScale;
            ls.x *= -1f;
            visualTransform.localScale = ls;

            // Flip melee hitbox local position
            Vector3 hitboxPos = meleeHitbox.transform.localPosition;
            hitboxPos.x *= -1f;
            meleeHitbox.transform.localPosition = hitboxPos;

            // Flip wall check local position
            Vector3 wallCheckPosLocal = wallCheckPos.localPosition;
            wallCheckPosLocal.x *= -1f;
            wallCheckPos.localPosition = wallCheckPosLocal;

            // Flip visual-only FX (like speedFX direction)
            speedFX.transform.rotation = Quaternion.Euler(0f, isFacingRight ? 0f : 180f, 0f);

            if (rb.linearVelocityY == 0)
            {
                smokeFX.Play();
            }
        }

    }
    private void OnDrawGizmosSelected()     //draws a cube that helps visualize the ground and wall check
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }

    //SOUND MANAGEMENT

    public void PlayFootstep()
    {
        SoundEffectManager.Play("FootstepGrass");
    }

    public void PlaySwordSwingSound()
    {
        SoundEffectManager.Play("SwordSwing");
    }

    public void PlayJumpSound()
    {
        SoundEffectManager.Play("Jump");
    }

}
