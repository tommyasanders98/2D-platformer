using UnityEngine;
using UnityEngine.InputSystem;                  //allows input actions to be written to

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;                      //variable for game object rigid body assets
    private float logTimer = 0f;                //log message timer 
    public bool isFacingRight = true;

    [Header("Movement")]                        //helps track movement speed, must be placed above a serialized to add a label above the field in the inspector
    public float moveSpeed = 5f;                //variable for movement speed
    float horizontalMovement;                   //variable for horizontal movement

    [Header("Jumping")]                         //helps track jumping
    public float jumpPower = 10f;               //jump power
    public int maxJumps = 2;                    //maximum jumps
    public int jumpsRemaining;                  //remaining jumps

    [Header("GroundCheck")]                                     //helps track jumping
    public Transform groundCheckPos;                            //Checks position
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);   //Checks size
    public LayerMask groundLayer;                               //Checks layer
    bool isGrounded;                                            //bool for touching ground

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

    // Update is called once per frame
    void Update()
    {
       
        groundCheck();
        Gravity();
        WallSlide();
        WallJump();
        if (!isWallJumping) //character cannot move while wall jumping
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocityY); //updates the movement of the object on the x-axis while maintaining current y-axis velocity
            Flip();
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

            //force a character flip
            if(transform.localScale.x != 0)
            {
                // Flip the character's facing direction
                isFacingRight = !isFacingRight;
                // Get the current local scale of the character
                Vector3 ls = transform.localScale;
                // Invert the x-scale to flip the character visually
                ls.x *= -1f;
                // Apply the flipped scale back to the character
                transform.localScale = ls;
            }

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
        if (jumpsRemaining > 0)
        {
            if (context.performed)                                  //if the button was fully pressed
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);
                jumpsRemaining--;
            }

            else if (context.canceled)                               //if the jump button was not pressed all the way
            {
                if (rb.linearVelocityY > 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);     //half powered jump based on current vertical height
                    jumpsRemaining--;
                }
            }
        }

        //Wall jumping
        if(context.performed && wallJumpTimer > 0)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);  //jump away from the wall
            wallJumpTimer = 0;

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);        //wall jump will last 0.5 seconds, player can jump again after 0.6 seconds

        }
    }
    private void groundCheck()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))  //if the overlap box goes over a ground layer
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
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
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);  //Return if player is on a wall
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
