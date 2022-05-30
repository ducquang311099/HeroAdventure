using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EnemyBanditController : MonoBehaviour
{
    #region ==== Fields ====
    [SerializeField] private Animator anim;
    [SerializeField] private Transform playerPos;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private TextMeshPro dmgPopupText;
    [SerializeField] private GameObject dmgPopupParent;
    [SerializeField] private GameObject[] dropItem;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private float attackRange = .5f;
    [SerializeField] private float attackDame = 40f;

    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private float currentHealth;
    private float nextAttackTime = 0f;
    private enum MovementState { idle, run, combatIdle }
    #endregion

    #region ==== Unity LifeCirle ====
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if (anim.GetBool("IsDead"))
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            else
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !anim.GetBool("IsDead"))
        {
            if (Vector2.Distance(collision.transform.position, transform.position) <= 2.2f)
                Attack();
            else
                FlipEnemy();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            anim.SetInteger("State", (int)MovementState.idle);
    }
    #endregion

    #region ==== Methods ====
    public bool TakeDamage(float dame, float basicDmg)
    {
        anim.SetTrigger("Hurt");
        currentHealth -= dame;
        GenerateDmgPopup(dame, basicDmg);
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    void GenerateDmgPopup(float dame, float basicDmg)
    {
        // Set color
        if (dame > basicDmg * 1.2f)
            dmgPopupText.color = new Color32(255, 0, 0, 255);
        else if (dame > basicDmg * 1f)
            dmgPopupText.color = new Color32(255, 40, 40, 255);
        else
            dmgPopupText.color = new Color32(255, 255, 255, 255);

        // Set Text & Generate
        dmgPopupText.text = dame.ToString();
        GameObject temp = Instantiate(dmgPopupParent, new Vector2(transform.position.x, transform.position.y + 1.5f), Quaternion.identity);
        Destroy(temp, 1f);
    }

    void Die()
    {
        anim.SetBool("IsDead", true);
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 3.5f);

        foreach (GameObject item in dropItem)
        {
            Instantiate(item, new Vector2(transform.position.x, transform.position.y + 1.5f), Quaternion.identity);
        }
    }

    void FlipEnemy()
    {
        if (playerPos.position.x - transform.position.x > 0)
        {
            sprite.flipX = true;
            attackPoint.position = new Vector2(transform.position.x + .5f, transform.position.y + 1.7f);
        }
        else
        {
            sprite.flipX = false;
            attackPoint.position = new Vector2(transform.position.x - .5f, transform.position.y + 1.7f);
        }
        anim.SetInteger("State", (int)MovementState.run);
        // Move Toward Player Only Pos.x
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerPos.position.x, transform.position.y, transform.position.z), Time.deltaTime * moveSpeed);
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            // Animation
            anim.SetTrigger("Attack");
            // Attack effect
            Invoke("AttackEffect", 0.2f);
            //Cooldown attack
            nextAttackTime = Time.time + 1f / attackRate;
        }
        // Update Player Position 
        for (int i = 0; i < 4; i++)
        {
            if (i <= 1)
                playerPos.position = new Vector2(playerPos.position.x - 1, playerPos.position.y);
            else
                playerPos.position = new Vector2(playerPos.position.x + 1, playerPos.position.y);
        }
        anim.SetInteger("State", (int)MovementState.combatIdle);
    }

    void AttackEffect()
    {
        // Detect Enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damaged enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<PlayerController>().TakeDamage(RandomDmg(20));

        }
    }

    float RandomDmg(float range)
    {
        System.Random random = new System.Random();
        double currentDmg = (random.NextDouble() * range + attackDame - range / 2);
        return (float)currentDmg;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion

}
