using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [SerializeField] AudioClip ac_death;
    [SerializeField] AudioClip ac_wounded;
    [SerializeField] AudioClip ac_attack;
    [SerializeField] Drops drop;
    [SerializeField] float m_speed = 1;
    [SerializeField] float m_collisionForce = 54;
    [SerializeField] List<GameObject> DropPrefabLists = new List<GameObject>();
    [SerializeField] int m_healthMax = 30;
    [SerializeField] ParticleSystem PS_Death;
    [SerializeField] Slider m_healthBar;
    protected enum Drops { none, seed }
    [SerializeField] protected State state;
    protected enum State { none, wandering, attackingPlayer, attackingWellHeart, dead }

    Animator m_animator;
    AudioSource m_audioSource;
    bool m_seesPlayer;
    GameObject m_player;
    int m_currentHealth = 30;
    Rigidbody2D m_rigid;

    public void ReceiveDamage(int damage)
    {
        m_currentHealth -= damage;
        UpdateHealthBar();
        if(m_currentHealth <= 0)
        {
            Death();
        }
    }

    public void Devoured()
    {
        Util.SetUpAudioSourcePrefab(ac_death);
        state = State.dead;
        Death();
    }

    private void ChangeAudioClip(AudioClip ac)
    {
        m_audioSource.clip = ac;
    }

    protected virtual void Start()
    {
        m_currentHealth = m_healthMax;
        m_healthBar.value = m_healthMax;
        m_healthBar.maxValue = m_healthMax;
        m_animator = GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        m_rigid = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (m_seesPlayer)
        {
            MoveTowardsPlayer();
        }
        else
        {
            MoveTowardsWellHeart();
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.tag == Constants.Tags.Player)
    //    {
    //        m_seesPlayer = true;
    //        m_playerLocation = collision.transform.position;
    //        state = State.attackingPlayer;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Constants.Tags.Player)
        {
            m_seesPlayer = true;
            m_player = collision.gameObject;
            state = State.attackingPlayer;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == Constants.Tags.Player)
        {
            Vector2 distance = collision.collider.transform.position - transform.position;
            Vector2 direction = distance.normalized;
            collision.rigidbody.velocity = Vector2.zero;
            collision.rigidbody.AddForce(m_collisionForce * direction, ForceMode2D.Impulse);
        }
    }

    protected void MoveTowardsPlayer()
    {
        Vector2 dir = (Vector2)m_player.transform.position - (Vector2)transform.position;
        m_animator.SetFloat("horizontal", dir.x);
        m_animator.SetFloat("vertical", dir.y);
        m_rigid.velocity = dir.normalized * m_speed;
    }

    protected void MoveTowardsLocation(Vector2 loc)
    {
        Vector2 dir = loc - (Vector2)transform.position;
        Debug.Log(dir.x);
        m_animator.SetFloat("horizontal", dir.x);
        m_animator.SetFloat("vertical", dir.y);
        m_rigid.velocity = dir.normalized * m_speed;
    }

    protected void MoveTowardsWellHeart()
    {
        Vector2 wellHeartLoc = SpawnManager.instance.WellHearts[0].transform.position;

        if (Vector2.Distance(transform.position, wellHeartLoc) > 2)
        {
            Vector2 dir = wellHeartLoc - (Vector2)transform.position;
            m_rigid.velocity = dir.normalized * m_speed;
        }
        else
        {
            m_rigid.velocity = Vector2.zero;
        }
    }

    protected void UpdateHealthBar()
    {
        m_healthBar.value = m_currentHealth;
    }

    protected void DropLoot()
    {
        switch (drop)
        {
            case Drops.none:
                break;
            case Drops.seed:
                Instantiate(DropPrefabLists[0], transform.position, Quaternion.identity);
                break;
        }
    }

    protected virtual void Death()
    {
        StopAllCoroutines();
        // create and play death particle system
        var ps = Instantiate(PS_Death, transform.position, Quaternion.identity);
        ps.Play();
        DropLoot();

        Destroy(ps.gameObject, PS_Death.main.duration);
        Destroy(gameObject);
    }
}
