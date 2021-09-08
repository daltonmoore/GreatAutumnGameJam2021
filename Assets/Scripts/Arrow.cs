using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    int damage = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            Monster monster = collision.GetComponent<Monster>();
            BoxCollider2D boxCollider2D = (BoxCollider2D)collision;
            if (monster != null && boxCollider2D != null)
            {
                monster.ReceiveDamage(damage);
                Destroy(gameObject);
            }
        }
        catch { }
    }
}
