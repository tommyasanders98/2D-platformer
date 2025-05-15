using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IHittable
{
    private Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;
    private float knockBackTimer = 0f;

    public int damage = 1;
    public int maxHealth = 3;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (knockBackTimer > 0)
        {
            knockBackTimer -= Time.deltaTime;
            return;
        }

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << player.gameObject.layer);

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocityY);

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer);

            if (!groundInFront.collider && !gapAhead.collider)
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && !platformAbove.collider)
            {
                shouldJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 jumpDirection = direction * jumpForce;
            rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
        }
    }

    public void Hit(Vector2 hitDirection, float damage)
    {
        TakeDamage((int)damage, hitDirection.normalized * 10f); // Use appropriate knockback scale if needed
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        currentHealth -= damage;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);
        knockBackTimer = 0.2f;
        StartCoroutine(FlashWhite());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        foreach (LootItem lootItem in lootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.itemPrefab);
            }
            break;
        }
        GameController gc = GameObject.FindAnyObjectByType<GameController>();
        if (gc != null && gc.xpManager != null)
        {
            GetComponent<XPDropper>()?.DropOrbs(Random.Range(3, 11), gc.xpManager); // Pass XPManager
        }
        Destroy(gameObject);
    }

    void InstantiateLoot(GameObject loot)
    {
        if (loot)
        {
            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);
            droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class Enemy : MonoBehaviour
//{
//    private Transform player;
//    public float chaseSpeed = 2f;
//    public float jumpForce = 2f;
//    public LayerMask groundLayer;

//    private Rigidbody2D rb;
//    private bool isGrounded;
//    private bool shouldJump;
//    private float knockBackTimer = 0f;

//    public int damage = 1;
//    public int maxHealth = 3;
//    private int currentHealth;
//    private SpriteRenderer spriteRenderer;  //allows us to change enemy color when they get hit
//    private Color originalColor;

//    //loot table
//    [Header("Loot")]
//    public List<LootItem> lootTable = new List<LootItem>();

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();   //calls this off of the object it is placed on
//        player = GameObject.FindWithTag("Player").GetComponent < Transform>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        currentHealth = maxHealth;
//        originalColor = spriteRenderer.color;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (knockBackTimer > 0)
//        {
//            knockBackTimer -= Time.deltaTime;
//            return; // skip movement during knockback
//        }
//        //Is gournded?
//        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);  //checks if character is grounded
//        //Player direction
//        float direction = Mathf.Sign(player.position.x - transform.position.x);

//        //Player above detection
//        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << player.gameObject.layer);

//        if (isGrounded)
//        {
//            //chase player
//            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocityY);

//            //jump if there's a gap ahead && no ground infront
//            //else if there's a player above and platform above

//            //if ground
//            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);

//            //if gap
//            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
//            //if platform above
//            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer);

//            if (!groundInFront.collider && !gapAhead.collider)
//            {
//                shouldJump = true;
//            }
//            else if (isPlayerAbove && !platformAbove.collider)
//            {
//                shouldJump = true;
//            }
//        }
//    }

//    private void FixedUpdate()
//    {
//        if(isGrounded && shouldJump)
//        {
//            shouldJump = false;
//            Vector2 direction = (player.position - transform.position).normalized;

//            Vector2 jumpDirection = direction * jumpForce;

//            rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
//        }
//    }

//    public void TakeDamage(int damage, Vector2 knockback)
//    {
//        currentHealth -= damage;
//        rb.linearVelocity = Vector2.zero; // Optional: clear current velocity
//        rb.AddForce(knockback, ForceMode2D.Impulse); // Apply knockback force

//        knockBackTimer = 0.2f; // Prevent movement logic from overriding knockback

//        StartCoroutine(FlashWhite());

//        if(currentHealth <= 0)
//        {
//            Die();
//        }
//    }

//    private IEnumerator FlashWhite()
//    {

//        spriteRenderer.color = Color.white;
//        yield return new WaitForSeconds(0.2f);
//        spriteRenderer.color = originalColor;
//    }

//    void Die()
//    {
//        //go around loot table
//        foreach(LootItem lootItem in lootTable)
//        {
//            if(Random.Range(0f, 100f) <= lootItem.dropChance)
//            {
//                InstantiateLoot(lootItem.itemPrefab);
//            }
//            break;
//        }
//        //spawn item
//        Destroy(gameObject);
//    }

//    void InstantiateLoot (GameObject loot)
//    {
//        if(loot)
//        {
//            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);
//            droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
//        }
//    }
//}
