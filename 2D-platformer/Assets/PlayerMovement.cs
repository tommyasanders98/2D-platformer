using UnityEngine;
using UnityEngine.InputSystem;                  //allows input actions to be written to

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;                      //variable for game object rigid body assets
    [Header("Movement")]                        //helps track movement speed, must be placed above a serialized to add a label above the field in the inspector
    public float moveSpeed = 5f;                //variable for movement speed
    float horizontalMovement;                   //variable for horizontal movement

    [Header("Jumping")]                         //helps track jumping
    public float jumpPower = 10f;                //jump power
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocityY); //updates the movement of the object on the x-axis while maintaining current y-axis velocity
    }

    public void Move(InputAction.CallbackContext context) 
    {
        horizontalMovement = context.ReadValue<Vector2>().x;    //ties horizontal movement variable to the actual x-value of the character
    }

    public void Jump(InputAction.CallbackContext context)       
    {
        if (context.performed)                                  //if the button was fully pressed
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);
        }
  
    }
}
