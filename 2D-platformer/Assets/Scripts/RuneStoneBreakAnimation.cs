using UnityEngine;

public class RuneStoneBreakAnimation : MonoBehaviour, IHittable
{
    private Animator anim;
    private bool broken = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Hit(Vector2 hitDirection, float damage)
    {
        if (broken) return;

        broken = true;
        anim.SetTrigger("Break");

        // Disable collider immediately so it doesn't get hit again
        GetComponent<Collider2D>().enabled = false;
    }

    //  Called via Animation Event
    public void BreakAndDropXP()
    {
        SoundEffectManager.Play("Rune Stone Break");

        GameController gc = GameObject.FindAnyObjectByType<GameController>();
        if (gc != null && gc.xpManager != null)
        {
            GetComponent<XPDropper>()?.DropOrbs(Random.Range(3, 11), gc.xpManager);
        }
    }

    //  Optional: Called at the end of the break animation
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}