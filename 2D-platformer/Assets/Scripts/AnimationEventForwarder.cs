using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
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
}