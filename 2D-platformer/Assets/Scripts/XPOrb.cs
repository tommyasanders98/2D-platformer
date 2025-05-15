using UnityEngine;

public class XPOrb : MonoBehaviour, IXPorb
{
    public int value = 1;
    private XPManager xpManager;

    public void SetValue(int newValue)
    {
        value = newValue;
    }

    public void SetXPManager(XPManager manager)
    {
        xpManager = manager;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && xpManager != null)
        {
            xpManager.AddXP(value);
            SoundEffectManager.Play("XP Orb");
            Destroy(gameObject);
        }
    }
}
