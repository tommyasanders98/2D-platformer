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
        if (playerMovement != null && playerMovement.jumpSoundQueued)
        {
            SoundEffectManager.Play("Jump");
            playerMovement.jumpSoundQueued = false;
        }
    }

    public void EnableMeleeHitbox()
    {
        if (playerMovement != null)
            playerMovement.EnableMeleeHitbox();
    }

    public void DisableMeleeHitbox()
    {
        if (playerMovement != null)
            playerMovement.DisableMeleeHitbox();
    }

    public void EnableRunAfterAttack()
    {
        if (playerMovement != null)
            playerMovement.EnableRunAfterAttack();
    }

    public void DisableRun()
    {
        if (playerMovement != null)
            playerMovement.DisableRun();
    }
}