using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToLoadLevel : MonoBehaviour
{
    public float holdDuration = 1f; //how long you have to hold before action
    public Image fillCircle;    //image of circle loader

    private float holdTimer = 0;
    private bool isHolding = false;

    public static event Action OnHoldComplete;

    // Update is called once per frame
    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDuration;
            if(holdTimer >= holdDuration)
            {
                //Load next level
                OnHoldComplete.Invoke();    //this will leave the loading of the next level to the game controller
                ResetHold();
            }
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;   //when action button is held
        }
        else if (context.canceled)  //button released
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        isHolding = false;
        holdTimer = 0;
        fillCircle.fillAmount = 0;
    }
}
