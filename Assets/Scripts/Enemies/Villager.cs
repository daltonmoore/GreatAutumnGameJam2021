using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Monster
{
    [SerializeField] GameObject TownTravelRadius;

    [SerializeField] protected State state;
    protected enum State { none, wanderingTown, attackingPlayer, attackingWellHeart }

    Coroutine moveTowardsCoroutine;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //ChangeState(State.wanderingTown);
    }

    // Update is called once per frame
    protected override void Update()
    {
        switch (state)
        {
            case State.none:
                break;
            case State.wanderingTown:
                GetRandomPointToMoveTo();
                break;
            case State.attackingPlayer:
                break;
            case State.attackingWellHeart:
                MoveTowardsWellHeart();
                break;
            default:
                break;
        }
    }

    void ChangeState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.none:
                break;
            case State.wanderingTown:

                break;
            case State.attackingPlayer:
                break;
            case State.attackingWellHeart:
                break;
            default:
                break;
        }
    }

    void GetRandomPointToMoveTo()
    {
        if (moveTowardsCoroutine == null)
        {
            Bounds townBounds = TownTravelRadius.GetComponent<CircleCollider2D>().bounds;
            Vector2 randomPoint = Random.insideUnitCircle * (Vector2)townBounds.extents + (Vector2)townBounds.center;
            // debug random point visual
            GameObject go = Instantiate(Resources.Load<GameObject>("Inside Circle_"), randomPoint, Quaternion.identity);
            Collider2D collider2D = Physics2D.OverlapBox(randomPoint, new Vector2(1, 1), 0);
            if (collider2D)
            {
                Debug.Log(collider2D.name);
                Destroy(go);
                return;
            }
            else
            {
                Debug.Log("Did not overlap anything");
                Destroy(go, 8);
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
        moveTowardsCoroutine = null;
    }
}
