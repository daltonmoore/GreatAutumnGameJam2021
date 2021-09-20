using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Monster
{
    [SerializeField] GameObject TownTravelRadius;
    [SerializeField] List<ParticleSystem> particleSystems;
    [SerializeField] float m_soulsOnDeath;

    Coroutine moveTowardsCoroutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        TownTravelRadius = GameObject.Find("TownTravel Radius");
        state = State.wandering;
    }

    // Update is called once per frame
    protected override void Update()
    {
        switch (state)
        {
            case State.none:
                break;
            case State.wandering:
                GetRandomPointToMoveTo();
                break;
            case State.attackingPlayer:
                if (m_seesPlayer)
                    MoveTowardsLocation(m_player.transform.position);
                else
                    state = State.wandering;
                break;
            case State.attackingWellHeart:
                MoveTowardsLocation(m_wellHeartLoc);
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Constants.Tags.Player)
        {
            m_seesPlayer = true;
            m_player = collision.gameObject;
            state = State.attackingPlayer;
        }
    }

    Vector2 randomPoint;
    void GetRandomPointToMoveTo()
    {
        if (moveTowardsCoroutine == null)
        {
            Bounds townBounds = TownTravelRadius.GetComponent<CircleCollider2D>().bounds;
            randomPoint = Random.insideUnitCircle * (Vector2)townBounds.extents + (Vector2)townBounds.center;
            // debug random point visual
            //GameObject go = Instantiate(Resources.Load<GameObject>("Inside Circle_"), randomPoint, Quaternion.identity);
            Collider2D collider2D = Physics2D.OverlapBox(randomPoint, new Vector2(5, 5), 0);
            if (collider2D)
            {
                //Debug.Log(collider2D.name);
                //Destroy(go);
                return;
            }
            else
            {
                //Debug.Log("Did not overlap anything");
                //Destroy(go, 8);
            }
            moveTowardsCoroutine = StartCoroutine(MoveTowardsRandomPoint(randomPoint));
        }
        //RaycastHit2D hit = Physics2D.BoxCast(randomPoint, new Vector2(1, 1), 0, Vector2.down);
        //if (hit)
        //{
        //    Debug.Log("Hit this point: " + hit.point);
        //    Debug.Log("Collider name: " + hit.collider.name);
        //    GameObject go2 = Instantiate(Resources.Load<GameObject>("Inside Circle_"), hit.point, Quaternion.identity);
        //    go2.GetComponent<SpriteRenderer>().color = Color.white;
        //}
        //Debug.Log("TownBounds center: "+townBounds.center);
        //Debug.Log("Random Point: "+ randomPoint);
        //Debug.Log("Is inside circle? " + townBounds.Contains(randomPoint));
    }

    IEnumerator MoveTowardsRandomPoint(Vector2 randomPoint)
    {
        while (Vector2.Distance(transform.position, randomPoint) > 1)
        {
            MoveTowardsLocation(randomPoint);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log(name + ": made it destination");
        moveTowardsCoroutine = null;
    }

    protected override void Death()
    {
        base.Death();
        foreach (ParticleSystem ps in particleSystems)
        {
            Util.CreateParticleSystem(ps, transform.position);
        }
        GameManager.Instance.AddSouls(m_soulsOnDeath);
    }

    public void OnDrawGizmosSelected()
    {
        if (!isActiveAndEnabled) return;

        Gizmos.DrawWireSphere(new Vector3(randomPoint.x, randomPoint.y, 0),1);
    }
}
