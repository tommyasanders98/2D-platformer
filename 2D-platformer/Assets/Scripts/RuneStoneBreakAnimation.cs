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
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f); // Or use animation event instead
        GetComponent<XPDropper>()?.DropOrbs(Random.Range(3, 11));
    }

    public void DestroySelf() => Destroy(gameObject); // Called from animation event
}
