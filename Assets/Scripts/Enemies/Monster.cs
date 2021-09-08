using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [SerializeField] float m_speed;
    [SerializeField] float m_collisionForce = 1;
    [SerializeField] int m_healthMax = 30;
    [SerializeField] ParticleSystem PS_Death;
    [SerializeField] Slider m_healthBar;
    [SerializeField] List<GameObject> DropPrefabLists = new List<GameObject>();
    [SerializeField] Drops drop;
    enum Drops { none, seed }

    Animator m_animator;
    bool m_seesPlayer;
    int m_currentHealth = 30;
    Rigidbody2D m_rigid;
    Vector2 m_playerLocation;

    public void ReceiveDamage(int damage)
    {
        m_currentHealth -= damage;
        UpdateHealthBar();
        if(m_currentHealth <= 0)
        {
            Death();
        }
    }

    void Start()
    {
        m_currentHealth = m_healthMax;
        m_healthBar.value = m_healthMax;
        m_healthBar.maxValue = m_healthMax;
        m_animator = GetComponent<Animator>();
        m_rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (m_seesPlayer)
        {
            MoveTowardsPlayer();
        }
        else
        {
            MoveTowardsWellHeart();
        }
        //MoveTowardsWellHeart();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == Constants.Tags.Player)
        {
            m_seesPlayer = true;
            m_playerLocation = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Constants.Tags.Player)
        {
            m_seesPlayer = false;
            m_rigid.velocity = Vector2.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == Constants.Tags.Player)
        {
            Vector2 distance = collision.collider.transform.position - transform.position;
            Vector2 direction = distance.normalized;
            collision.rigidbody.velocity = Vector2.zero;
            collision.rigidbody.AddForce(m_collisionForce * direction, ForceMode2D.Impulse);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 dir = m_playerLocation - (Vector2)transform.position;
        Debug.Log(dir.x);
        m_animator.SetFloat("horizontal", dir.x);
        m_animator.SetFloat("vertical", dir.y);
        m_rigid.velocity = dir.normalized * m_speed;
    }

    void MoveTowardsWellHeart()
    {
        Vector2 dir = (Vector2)SpawnManager.instance.WellHearts[0].transform.position - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
        if (hit && hit.collider.tag == Constants.Tags.Collision)
        {
            
        }
        else
        {
            m_rigid.velocity = dir.normalized * m_speed;
        }
    }

    void UpdateHealthBar()
    {
        m_healthBar.value = m_currentHealth;
    }

    void DropLoot()
    {
        switch (drop)
        {
            case Drops.none:
                break;
            case Drops.seed:
                break;
        }
    }

    void Death()
    {
        // create and play death particle system
        var ps = Instantiate(PS_Death, transform.position, Quaternion.identity);
        ps.Play();
        DropLoot();

        Destroy(ps.gameObject, PS_Death.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}
