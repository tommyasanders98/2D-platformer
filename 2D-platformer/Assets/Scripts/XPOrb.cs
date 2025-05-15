using UnityEngine;

public class XPOrb : MonoBehaviour, IXPorb
{
    public int value = 1;

    public void SetValue(int newValue)
    {
        value = newValue;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO: Add XP to player later
            Destroy(gameObject);
        }
    }
}
