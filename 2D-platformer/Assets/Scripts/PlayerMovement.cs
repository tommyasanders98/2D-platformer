using UnityEngine;
using UnityEngine.InputSystem;                  //allows input actions to be written to

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;                      //variable for game object rigid body assets
    private float logTimer = 0f;                //log message timer 
    public bool isFacingRight = true;
    public Animator animator;
    public ParticleSystem smokeFX;              //particle system for smoke effects, ran when animator is called

    [Header("Movement")]                        //helps track movement speed, must be placed above a serialized to add a label above the field in the inspector
    public float moveSpeed = 5f;                //variable for movement speed
    float horizontalMovement;                   //variable for horizontal movement
    public float runSpeedMultiplier = 1.5f;     //variable for running speed of character
    public bool isRunning;
    public float currentSpeed;                  //tracks current speed for running vs walking

    [Header("Jumping")]                         //helps track jumping
    public float jumpPower = 10f;               //jump power
    public int maxJumps = 2;                    //maximum jumps
    public int jumpsRemaining;                  //remaining jumps
    public bool isJumping;                      //tracking if the character is wall or regular jumping

    [Header("GroundCheck")]                                     //helps track jumping
    public Transform groundCheckPos;                            //Checks position
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);   //Checks size
    public LayerMask groundLayer;                               //Checks layer
    public bool isGrounded;                                            //bool for touching ground

    [Header("WallCheck")]                                     
    public Transform wallCheckPos;                            //Checks position
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.5f);   //Checks size
    public LayerMask wallLayer;                               //Checks layer

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallSlide")]
    public float wallSlideSpeed = 2f;
    public bool isWallSliding;

    //Wall Jumping
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
    void Update()   // Update is called once per frame
    {

        Gravity();
        WallSlide();
        WallJump();
        if (!isWallJumping) //character cannot move while wall jumping
        {
            rb.linearVelocity = new Vector2(horizontalMovement * currentSpeed, rb.linearVelocityY); //updates the movement of the object on the x-axis while maintaining current y-axis velocity
            Flip();
        }

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

        // Increase timer by the time passed since the last frame
        //logTimer += Time.deltaTime;

        //if (logTimer >= 0.1f)
        //{
        //    if (wallCheck())
        //    {
        //        Debug.Log("update");
        //    }
        //    // Reset the timer
        //    logTimer = 0f;
        //}



    }
    
    private void FixedUpdate()  //This runs at a fixed rate based on Unity's physics engine, not based on FPS
    {
        groundCheck();  //I found that running this constantly helps properly reset the jumps remaining counter
        currentSpeed = isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;  //this is a compact "if else" statement -> if isRunning true ... else moveSpeed
                                                                                //this is used to calculate the running speed based on if the run action is pressed or not
        
        if (isGrounded)
        {
            isJumping = false;  //reset tracking variable
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }
    }
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
    private void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;    //makes the character jump in the opposite direction
            wallJumpTimer = wallJumpTime;   //reset wall jump timer

            CancelInvoke(nameof(CancelInvoke));     //as soon as we wall slide, we can jump again
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime; //decrease timer
        }
    }
    private void CancelWallJump()
    {
        isWallJumping = false;
    }
    public void Move(InputAction.CallbackContext context) 
    {
        horizontalMovement = context.ReadValue<Vector2>().x;    //ties horizontal movement variable to the actual x-value of the character
    }
    public void Run(InputAction.CallbackContext context)    //this only tells when the run action is pressed, not if the player is actually running
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
        animator.SetTrigger("jump");    //plays jump animation
        smokeFX.Play();     //plays smoke animation
    }
    private void Gravity()
    {
        if (rb.linearVelocityY < 0) 
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;    //fall increasingly faster
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -maxFallSpeed));  //This limits the fall speed to the maximum fall speed
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        
            //Wall Jump
            if (context.performed && wallJumpTimer > 0)
            {
                isJumping = true;       //just for tracking on inspector
                isWallJumping = true;                                                                   //bool for state of wall jumping
                rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);  //jump away from the wall
                wallJumpTimer = 0;
                jumpFX();                                                            //pass jumping action to animator

                //force a character flip
                if (transform.localScale.x != 0)
                {
                    isFacingRight = !isFacingRight;
                    Vector3 ls = transform.localScale;
                    ls.x *= -1f;
                    transform.localScale = ls;
                }

                Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);        //wall jump will last 0.5 seconds, player can jump again after 0.6 seconds
                return; //skip the rest of the logic

            }
        if (jumpsRemaining > 0)                   //performs a jump if there are any jumps remaining
        {
            //Regular Jump
            if (context.performed)                                  //if the button was fully pressed
            {
                isJumping = true;       //just for tracking on inspector
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);
                jumpsRemaining--;
                jumpFX(); //pass jumping action to animator
            }

            //Half Jump
            else if (context.canceled)                               //if the jump button was not pressed all the way
            {
                if (rb.linearVelocityY > 0)
                {
                    isJumping = true;       //just for tracking on inspector
                    rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);     //half powered jump based on current vertical height
                    //jumpsRemaining--;
                    jumpFX(); //pass jumping action to animator
                }
            }
        }

        
    }
    private void groundCheck()
    {

        // Perform a raycast downward from the player's position to check if there's ground beneath them
        //RaycastHit2D hit = Physics2D.Raycast(groundCheckPos.position, Vector2.down, groundCheckSize.y, groundLayer);

        //if (hit.collider != null) // If something is hit, the player is on the ground
        //{
        //    isGrounded = true;
        //    if (jumpsRemaining < maxJumps)
        //    {
        //        jumpsRemaining = maxJumps; // Reset jumps when on the ground
        //    }
        //}
        //else
        //{
        //    isGrounded = false;
        //}

        // For debugging, draw the raycast in the scene view
        //Debug.DrawRay(groundCheckPos.position, Vector2.down * groundCheckSize.y, Color.red);

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
        // Check if character's current facing direction doesn't match the movement direction:
        // - If facing right but moving left (horizontalMovement < 0)
        // - OR if facing left but moving right (horizontalMovement > 0)

        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            // Flip the character's facing direction
            isFacingRight = !isFacingRight;
            // Get the current local scale of the character
            Vector3 ls = transform.localScale;
            // Invert the x-scale to flip the character visually
            ls.x *= -1f;
            // Apply the flipped scale back to the character
            transform.localScale = ls;

            if (rb.linearVelocityY == 0)
            {
                smokeFX.Play();     //play smoke effects
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
}
