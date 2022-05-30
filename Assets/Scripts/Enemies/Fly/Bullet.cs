using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float dmgBullet = 10f;

    private void Start()
    {
        Destroy(gameObject, 1.7f);
    }

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeDamage(RandomDmg(10));
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("HigherGround") || collision.gameObject.CompareTag("LowestGround"))
        {
            Destroy(gameObject);
        }
    }

    float RandomDmg(float range)
    {
        System.Random random = new System.Random();
        double currentDmg = (random.NextDouble() * range + dmgBullet - range / 2);
        return (float)currentDmg;
    }

}
