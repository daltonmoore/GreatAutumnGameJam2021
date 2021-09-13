using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWeapon : MonoBehaviour
{
    public int damage = 10;

    float weaponCooldown = 1;
    Coroutine damageCoroutine;
    CircleCollider2D weaponCollider;

    private void Start()
    {
        weaponCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        //weaponCollider.
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CheckHitObject(collision.tag))
        {
            weaponCollider.enabled = false;
            //weaponCollider.is
        }
    }

    bool CheckHitObject(string objectTag)
    {
        if (damageCoroutine == null)
        {
            switch (objectTag)
            {
                case Constants.Tags.Player:
                    PlayerController.instance.ReceiveDamage(damage);
                    damageCoroutine = StartCoroutine(DamageCooldown());
                    return true;
                case Constants.Tags.WellHeart:
                    SpawnManager.instance.WellHearts[0].ReceiveDamage();
                    damageCoroutine = StartCoroutine(DamageCooldown());
                    return true;
            }
        }
        return false;
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(weaponCooldown);
        damageCoroutine = null;
        weaponCollider.enabled = true;
    }
}
