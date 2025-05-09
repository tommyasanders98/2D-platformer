using UnityEngine;

public class RuneStoneBreakAnimation : MonoBehaviour
{
    private Animator anim;
    private bool broken = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!broken && other.CompareTag("Melee Hit Box"))
        {
            broken = true;
            anim.Play("RuneStoneBreak");

            // Optional: disable collider after hit
            GetComponent<Collider2D>().enabled = false;

            // Optional: destroy after animation
            Destroy(gameObject, 1f); // adjust time based on animation length
        }
    }
}
