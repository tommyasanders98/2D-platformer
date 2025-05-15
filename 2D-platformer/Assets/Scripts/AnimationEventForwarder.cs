using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    private PlayerController playerMovement;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerController>();
    }

    public void PlayFootstep()
    {
        if (playerMovement != null)
        {
            playerMovement.PlayFootstep();
        }
    }

    public void PlaySwordSwingSound()
    {
        if (playerMovement != null)
        {
            playerMovement.PlaySwordSwingSound();
        }
    }

    public void PlayJumpSound()
    {
        if (playerMovement != null)
        {
            playerMovement.PlayJumpSound();
        }
    }

    //public void PlayXPSound()
    //{
    //    if
    //}
}