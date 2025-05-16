using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class XPOrb : MonoBehaviour, IXPorb
{
    [Header("XP Settings")]
    public int value = 1;
    private XPManager xpManager;

    [Header("Attraction Settings")]
    public float detectionRadius = 3f;
    public float moveSpeed = 5f;
    public string playerTag = "Player Damage Hit Box";

    private Rigidbody2D rb;
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null && Vector2.Distance(transform.position, player.transform.position) <= detectionRadius)
            {
                target = player.transform;
                rb.gravityScale = 0f; // Disable gravity for attraction
            }
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && xpManager != null)
        {
            xpManager.AddXP(value);
            SoundEffectManager.Play("XP Orb");
            Destroy(gameObject);
        }
    }

    // IXPorb interface
    public void SetValue(int newValue)
    {
        value = newValue;
    }

    public void SetXPManager(XPManager manager)
    {
        xpManager = manager;
    }
}