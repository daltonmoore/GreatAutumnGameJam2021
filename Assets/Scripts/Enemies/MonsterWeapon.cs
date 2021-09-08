using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWeapon : MonoBehaviour
{
    public int damage = 10;

    float damageCooldown = .5f;
    float lastHitTime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckHitObject(collision.tag);
    }

    void CheckHitObject(string objectTag)
    {
        switch (objectTag)
        {
            case Constants.Tags.Player:
                if (lastHitTime + damageCooldown < Time.time)
                {
                    lastHitTime = Time.time;
                    PlayerController.instance.ReceiveDamage(damage);
                }
                break;
            case Constants.Tags.WellHeart:
                break;
        }
    }
}
