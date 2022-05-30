using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region ==== Fields ====
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float attackRange = .5f;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private float attackDame = 10f;
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private LayerMask jumpAbleGround;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Image healthBar;
    [SerializeField] private Slider expBar;

    public int kill = 0;

    private enum MovementState { idle, run, jump, fall }
    private float dirX = 0f;
    private float nextAttackTime = 0f;
    private float currentHealth = 0f;
    private float tempExp = 0f;
    private int level = 1;
    private bool isMoveLowerGround = true;
    private bool isKillEnemy = false;
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private Animator anim;
    private SpriteRenderer sprite;

    #endregion

    #region ==== Unity LifeCircle ====
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
        currentHealth = maxHealth;
        healthBar.fillAmount = 1f;
        expBar.value = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu.activeSelf) /* Cancle all activiies of Player when PauseMenu is Open|Ative */
            return;
        if (coll.isTrigger)
            return;
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        if (Input.GetButtonDown("Jump") && isGround())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, 0);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isMoveLowerGround)
                coll.isTrigger = true;
        }
        UpdateAnimationState();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("LowestGround"))
            isMoveLowerGround = false;
        if (collision.gameObject.CompareTag("HigherGround"))
            coll.isTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("LowestGround"))
            isMoveLowerGround = true;
        if (collision.gameObject.CompareTag("HigherGround"))
            coll.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            StartCoroutine(DameTrap(1f));
            //anim.SetTrigger("Death");
            //rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            ////gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
            //// Disable anim when player die
            //isDead = true;
        }
    }
    #endregion

    #region Methods
    IEnumerator DameTrap(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        TakeDamage(20);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateAnimationState()
    {
        MovementState state;

        // When Player Die => Cancle all anim
        if (anim.GetBool("IsDead"))
            return;

        // Animation Run, Idle
        if (dirX > 0f)
        {
            state = MovementState.run;
            sprite.flipX = false;
            attackPoint.position = new Vector2(transform.position.x + 1f, transform.position.y);
        }
        else if (dirX < 0f)
        {
            state = MovementState.run;
            sprite.flipX = true;
            attackPoint.position = new Vector2(transform.position.x - 1f, transform.position.y);
        }
        else
            state = MovementState.idle;

        // Animation Jump and Fall
        if (rb.velocity.y > .1f)
            state = MovementState.jump;
        else if (rb.velocity.y < -.1f)
            state = MovementState.fall;

        // Set Animation State
        anim.SetInteger("State", (int)state);

        //Set Animation Attack
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= nextAttackTime)
            {
                // Animation
                anim.SetTrigger("Attack");

                // Attack effect
                Invoke("Attack", 0.2f);

                //Cooldown attack
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

    }

    public void TakeDamage(float dame)
    {
        anim.SetTrigger("Hurt");
        currentHealth = Mathf.Max(0, currentHealth - dame);
        healthBar.fillAmount = currentHealth / maxHealth; 
        if (currentHealth == 0)
            Die();

        //currentHealth -= dame;
        //healthBar.fillAmount -= dame / maxHealth;
        //if (currentHealth <= 0)
        //    Die();
    }

    void Die()
    {
        anim.SetBool("IsDead", true);
        gameObject.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        GetComponent<Collider2D>().enabled = false;
    }

    void Attack()
    {
        // Detect Enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damaged enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Bandit"))
            {
                isKillEnemy = enemy.GetComponent<EnemyBanditController>().TakeDamage(RandomDmg(10), attackDame);
                if (isKillEnemy)
                {
                    AddExp(.4f);
                    kill++;
                }                  
            }
            if (enemy.CompareTag("Fly"))
            {
                isKillEnemy = enemy.GetComponent<FlyEnemy>().TakeDamage(RandomDmg(10), attackDame);
                if (isKillEnemy)
                {
                    AddExp(.9f);
                    kill++;
                }
            }
        }
    }

    void AddExp(float exp)
    {
        tempExp += exp;
        while (tempExp >= 1f)
            LevelUp();
        expBar.value = tempExp;
    }

    void LevelUp()
    {
        // Exp raise
        tempExp -= 1f;
        level++;
        expBar.maxValue *= 1.2f;

        // Dmg raise
        attackDame *= 1.1f;
    }

    int RandomDmg(float range)
    {
        System.Random random = new System.Random();
        double currentDmg = (random.NextDouble() * range + attackDame - range / 2);
        return (int)currentDmg;
    }

    bool isGround()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpAbleGround);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    #endregion

}
