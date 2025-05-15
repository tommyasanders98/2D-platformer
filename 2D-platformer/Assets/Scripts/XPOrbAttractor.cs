using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody2D))]
public class XPOrbAttractor : MonoBehaviour
{
    public float detectionRadius = 3f;
    public float moveSpeed = 5f;
    public string playerTag = "Player";

    private Rigidbody2D rb;
    private Transform target;
    private XPOrb orb;
    [SerializeField] private XPManager xpManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        orb = GetComponent<XPOrb>();
    }

    void Update()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null && Vector2.Distance(transform.position, player.transform.position) <= detectionRadius)
            {
                target = player.transform;
                rb.gravityScale = 0f; // Disable gravity for smooth attraction
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
        //Debug.Log($"[Trigger] Orb touched: {other.name}");
        if (target != null && other.CompareTag(playerTag))
        {
            if (orb != null && xpManager != null)
            {
                xpManager.AddXP(orb.value);
            }

            SoundEffectManager.Play("XP Orb");
            Destroy(gameObject);
        }
    }

    public void SetValue(int value)
    {
        if (orb == null) orb = GetComponent<XPOrb>();
        if (orb != null) orb.value = value;
    }

    public void SetXPManager(XPManager manager)
    {
        xpManager = manager;
    }
}