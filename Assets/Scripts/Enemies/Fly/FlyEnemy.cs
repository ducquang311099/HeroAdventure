using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlyEnemy : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject dmgPopupParent;
    [SerializeField] private TextMeshPro dmgPopupText;
    [SerializeField] private Transform playerPos;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private float maxHealth = 70f;

    private int currentWaypointNumber = 0;
    private float nextAttackTime = 0f;
    private float currentHealth = 0f;
    private bool isMove = true;
    private bool isDead = false;
    private SpriteRenderer spriteRender;
    private GameObject currentBullet;
    private Rigidbody2D rb;
    private Vector3 currentPlayerPos;
    private Vector2 attackAreaCenter;
    private Vector2 sizeArea;


    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sizeArea = new Vector2(20, 8);
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        FlyEnemyMove();
        BulletMove();
        CheckFire();
    }

    void FlyEnemyMove()
    {
        if (!isMove)
            return;
        if (Vector2.Distance(waypoints[currentWaypointNumber].transform.position, transform.position) < .1f)
        {
            currentWaypointNumber++;
            if (currentWaypointNumber >= waypoints.Length)
                currentWaypointNumber = 0;
            spriteRender.flipX = !spriteRender.flipX;
        }
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointNumber].transform.position, Time.deltaTime * moveSpeed);
    }

    void Attack()
    {
        if (isDead)
            return;
        if (Time.time >= nextAttackTime)
        {
            // Animation
            anim.SetTrigger("Attack");
            // Attack effect        
            Invoke("AttackEffect", 1.05f);

            Invoke("DelayTime", .1f);
            //Cooldown attack
            nextAttackTime = Time.time + 1f / attackRate;
        }
        for (int i = 0; i < 4; i++)
        {
            if (i <= 1)
                playerPos.position = new Vector2(playerPos.position.x - 1, playerPos.position.y);
            else
                playerPos.position = new Vector2(playerPos.position.x + 1, playerPos.position.y);
        }
    }

    void AttackEffect()
    {
        currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.Euler(new Vector3(0, 0, 90)));
        currentPlayerPos = playerPos.position;
    }

    void BulletMove()
    {
        if (currentBullet != null)
        {
            currentBullet.GetComponent<Rigidbody2D>().AddForce(currentPlayerPos - currentBullet.transform.position);
        }
    }

    public bool TakeDamage(float currentDmg, float basicDmg)
    {
        //anim.SetTrigger("Hurt");
        currentHealth -= currentDmg;
        GenerateDmgPopup(currentDmg, basicDmg);
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    void Die()
    {
        anim.SetTrigger("IsDead");
        isDead = true;
        Destroy(gameObject, 1.1f);

        //foreach (GameObject item in dropItem)
        //{
        //    Instantiate(item, new Vector2(transform.position.x, transform.position.y + 1.5f), Quaternion.identity);
        //}
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
        GameObject temp = Instantiate(dmgPopupParent, new Vector2(transform.position.x, transform.position.y + .5f), Quaternion.identity);
        Destroy(temp, 1f);
    }

    void CheckFire()
    {
        attackAreaCenter = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 3);
        if (Physics2D.BoxCast(attackAreaCenter, sizeArea, 0f, Vector2.down, .1f, playerLayer))
        {
            isMove = false;
            if (playerPos.position.x - transform.position.x > 0)
                spriteRender.flipX = true;
            else
                spriteRender.flipX = false;
            if (spriteRender.flipX)
                attackPoint.position = new Vector2(transform.position.x + .5f, transform.position.y - .7f);
            else
                attackPoint.position = new Vector2(transform.position.x - .5f, transform.position.y - .7f);
            Attack();
        }
        else
        {
            isMove = true;
            if (currentWaypointNumber == 0)
                spriteRender.flipX = false;
            else
                spriteRender.flipX = true;
            anim.SetInteger("State", 0);
        }
    }

    void DelayTime()
    {
    }

    private void OnDrawGizmos()
    {
        if (attackAreaCenter == null)
            return;
        Gizmos.color = new Color32(77, 77, 77, 150);
        Gizmos.DrawCube(attackAreaCenter, new Vector3(20, 8, 0));

    }
}
