using System;
using System.Collections;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // === COMPONENT REFERENCES ===
    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem smokeFX;
    public ParticleSystem speedFX;
    public TrailRenderer trailRenderer;
    public BoxCollider2D playerCollider;
    public Transform visualTransform;

    // === PAUSE MENU ===
    private bool suppressInput = false;
    public InputActionReference attackAction;

    // === COMBAT ===
    [Header("Combat")]
    public Weapon currentWeapon;
    public GameObject meleeHitbox;
    public float attackCooldown = 0.5f;
    public bool isAttacking = false;
    public float lastAttackTime = -999f;

    // === MOVEMENT CONFIGURATION ===
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeedMultiplier = 1.5f;
    public float movementSmoothTime = 0.05f;
    public float attackMoveDamp = 0.2f;
    public Vector3 positionTracker;
    public bool isFacingRight = true;
    private float horizontalMovement;
    private float speedMultiplier = 1f;
    private float currentSpeed;
    private Vector2 currentVelocity;
    private bool isRunning;

    // === JUMPING ===
    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    public int jumpsRemaining;
    public bool isJumping;
    public bool jumpSoundQueued = false;

    // === DASH ===
    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public bool isDashing;
    public bool canDash = true;

    // === GROUND & WALL CHECK ===
    [Header("Ground & Wall Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;
    public bool isGrounded;
    public bool isOnPlatform;

    public Transform wallCheckPos;
    public Vector2 wallCheckSize;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;
    public bool isWallSliding;

    // === WALL JUMP ===
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpTime = 0.5f;
    private float wallJumpTimer;

    // === COYOTE TIME ===
    public float coyoteTimer = 0.1f;
    private float coyoteTimeCounter;

    void Start()
    {
        SpeedItem.OnSpeedCollected += StartSpeedBoost;
    }

    void Update()
    {
        if (suppressInput) return;

        positionTracker = transform.position;
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);

        animator.SetBool("isRunning", isRunning && Mathf.Abs(horizontalMovement) > 0.1f);

        if (isDashing) return;

        Gravity();
        WallSlide();
        HandleWallJumpTimer();

        if (isGrounded) coyoteTimeCounter = coyoteTimer;
        else coyoteTimeCounter -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        groundCheck();
        Flip();
        currentSpeed = isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;
        float movementFactor = isAttacking ? attackMoveDamp : 1f;

        if (!isWallJumping)
        {
            Vector2 targetVelocity = new Vector2(horizontalMovement * currentSpeed * speedMultiplier * movementFactor, rb.linearVelocity.y);
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, movementSmoothTime);
        }

        animator.SetBool("isGrounded", isGrounded);
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = 2f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -18f));
        }
        else rb.gravityScale = 2f;
    }

    private void WallSlide()
    {
        if (wallCheck() && !isGrounded && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else isWallSliding = false;
    }

    private void HandleWallJumpTimer()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = (wallCheckPos.localPosition.x > 0) ? -1 : 1;
            wallJumpTimer = wallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f) wallJumpTimer -= Time.deltaTime;
    }

    private void CancelWallJump() => isWallJumping = false;

    private void groundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else isGrounded = false;
    }

    private bool wallCheck() => Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);

    private void Flip()
    {
        if ((isFacingRight && horizontalMovement < 0) || (!isFacingRight && horizontalMovement > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = visualTransform.localScale; ls.x *= -1f; visualTransform.localScale = ls;
            Vector3 hitboxPos = meleeHitbox.transform.localPosition; hitboxPos.x *= -1f; meleeHitbox.transform.localPosition = hitboxPos;
            Vector3 wallCheckPosLocal = wallCheckPos.localPosition; wallCheckPosLocal.x *= -1f; wallCheckPos.localPosition = wallCheckPosLocal;
            speedFX.transform.rotation = Quaternion.Euler(0f, isFacingRight ? 0f : 180f, 0f);
            if (rb.linearVelocity.y == 0) smokeFX.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPos != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        }

        if (wallCheckPos != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (PauseMenuController.GameIsPaused || suppressInput || !context.performed) return;

        if (!isAttacking && Time.time >= lastAttackTime + currentWeapon.hitCooldown)
        {
            isAttacking = true;
            animator.SetTrigger("attack");
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            lastAttackTime = Time.time;
            StartCoroutine(ResetAttackCooldown());
        }
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private IEnumerator speedBoostCoroutine(float multiplier)
    {
        speedMultiplier = multiplier;
        speedFX.Play();
        yield return new WaitForSeconds(2f);
        speedMultiplier = 1f;
        speedFX.Stop();
    }

    void StartSpeedBoost(float multiplier) => StartCoroutine(speedBoostCoroutine(multiplier));

    public void Move(InputAction.CallbackContext context) => horizontalMovement = context.ReadValue<Vector2>().x;

    public void Run(InputAction.CallbackContext context) => isRunning = context.performed;

    public void Dash(InputAction.CallbackContext context)
    {
        if (PauseMenuController.GameIsPaused || suppressInput || !context.performed) return;
        if (context.performed && canDash) StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(3, 8, true);
        canDash = false; isDashing = true; trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false; trailRenderer.emitting = false;
        Physics2D.IgnoreLayerCollision(3, 8, false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (PauseMenuController.GameIsPaused || suppressInput) return;

        // Handle full jump
        if (context.performed)
        {
            if (wallJumpTimer > 0)
            {
                isJumping = true;
                isWallJumping = true;
                rb.linearVelocity = new Vector2(wallJumpDirection * 5f, 10f);
                wallJumpTimer = 0;
                jumpSoundQueued = true; // Queue jump sound
                jumpFX();

                if ((isFacingRight && wallJumpDirection < 0) || (!isFacingRight && wallJumpDirection > 0))
                    Flip();

                Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
                return;
            }

            bool canUseCoyote = coyoteTimeCounter > 0f && jumpsRemaining == maxJumps;
            bool canJump = isGrounded || canUseCoyote || jumpsRemaining > 0;
            if (canJump)
            {
                isJumping = true;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                coyoteTimeCounter = 0f;
                jumpSoundQueued = true; // Only set here
                jumpFX();
            }
        }

        // Handle half-jump (early release) — DO NOT queue sound
        else if (context.canceled && rb.linearVelocity.y > 0)
        {
            bool canUseCoyote = coyoteTimeCounter > 0f && jumpsRemaining == maxJumps;
            bool canJump = isGrounded || canUseCoyote || jumpsRemaining > 0;
            if (canJump)
            {
                isJumping = true;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
                coyoteTimeCounter = 0f;
                jumpFX(); // Still play VFX, but no sound queue
            }
        }
    }


    public void Drop(InputAction.CallbackContext context)
    {
        if (PauseMenuController.GameIsPaused || suppressInput || !context.performed) return;

        if (isGrounded && isOnPlatform && playerCollider.enabled)
        {
            StartCoroutine(DisablePlayerCollider(0.25f));
        }
    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled = true;
    }

    // === Animation Sound Events ===
    public void PlayFootstep() => SoundEffectManager.Play("FootstepGrass");
    public void PlaySwordSwingSound() => SoundEffectManager.Play("SwordSwing");
    public void PlayJumpSound() => SoundEffectManager.Play("Jump");

    private void jumpFX()
    {
        animator.SetTrigger("jump");
        smokeFX.Play();
    }

    public void SuppressInputForOneFrame()
    {
        StartCoroutine(SuppressInputCoroutine());
    }

    private IEnumerator SuppressInputCoroutine()
    {
        suppressInput = true;
        attackAction.action.Disable();  // Temporarily disable

        yield return null;
        yield return null;
        yield return null;

        suppressInput = false;
        attackAction.action.Enable();   // Re-enable after input is flushed
    }


    // Called by Animation Events
    public void EnableMeleeHitbox()
    {
        if (meleeHitbox != null)
            meleeHitbox.SetActive(true);
    }

    public void DisableMeleeHitbox()
    {
        if (meleeHitbox != null)
            meleeHitbox.SetActive(false);
    }

}