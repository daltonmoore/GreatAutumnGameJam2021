using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SensorToolkit;
using System;

public class Monster : MonoBehaviour
{
    public event EventHandler Died;

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
    [SerializeField] SteeringRig2D steeringRig;

    protected enum Drops { none, seed }
    [SerializeField] protected State state;
    protected enum State { none, wandering, attackingPlayer, attackingWellHeart, dead }
    protected Vector2 m_wellHeartLoc;
    protected GameObject m_player;
    protected Rigidbody2D m_rigid;
    protected bool m_seesPlayer;

    Animator m_animator;
    AudioSource m_audioSource;
    int m_currentHealth = 30;

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

    protected virtual void Start()
    {
        m_currentHealth = m_healthMax;
        m_healthBar.value = m_healthMax;
        m_healthBar.maxValue = m_healthMax;
        m_animator = GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        m_rigid = GetComponent<Rigidbody2D>();
        m_wellHeartLoc = GetWellHeartLocation();
        steeringRig.IgnoreList.AddRange(new List<GameObject> { PlayerController.instance.gameObject, SpawnManager.instance.WellHearts[0].gameObject });

    }

    protected virtual void Update()
    {
        switch (state)
        {
            case State.none:
                break;
            case State.wandering:
                break;
            case State.attackingPlayer:
                if (m_seesPlayer)
                    MoveTowardsLocation(m_player.transform.position);
                else
                    state = State.attackingWellHeart;
                break;
            case State.attackingWellHeart:
                MoveTowardsLocation(m_wellHeartLoc);
                break;
            default:
                break;
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
    protected Vector2 GetWellHeartLocation()
    {
        if (SpawnManager.instance.WellHearts.Count <= 0)
            return Vector2.negativeInfinity;

        return SpawnManager.instance.WellHearts[0].transform.position;
    }

    protected void MoveTowardsLocation(Vector2 loc)
    {
        if (loc == Vector2.negativeInfinity)
            return;

        Vector2 dir = loc - (Vector2)transform.position;
        dir = steeringRig.GetSteeredDirection(dir);
        m_animator.SetFloat("horizontal", dir.x);
        m_animator.SetFloat("vertical", dir.y);

        m_rigid.velocity = dir.normalized * m_speed;
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
        Died?.Invoke(this, null);
        StopAllCoroutines();
        // create and play death particle system
        var ps = Instantiate(PS_Death, transform.position, Quaternion.identity);
        ps.Play();
        DropLoot();

        Destroy(ps.gameObject, PS_Death.main.duration);
        Destroy(gameObject);
    }
}
