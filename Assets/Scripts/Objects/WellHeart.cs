using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WellHeart : MonoBehaviour
{
    public event EventHandler WellHeartDeath;
    public int health = 100;
    public Slider healthBar;

    Coroutine receiveDamageCooldown;

    public void ReceiveDamage()
    {
        //if (receiveDamageCooldown == null)
        //{
        //    receiveDamageCooldown = StartCoroutine(Damage());
        //}
        health -= 5;
        healthBar.value = health;
        HUD.Instance.UpdateWellheartHealthBar(health);
        if (health <= 0)
        {
            WellHeartDeath?.Invoke(this, null);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        healthBar.value = healthBar.maxValue = health;
    }

    private IEnumerator Damage()
    {
        health -= 5;
        healthBar.value = health;
        if(health <= 0)
        {
            StopAllCoroutines();
            WellHeartDeath?.Invoke(this, null);
            Destroy(gameObject);
        }
        yield return new WaitForSeconds(1);
        receiveDamageCooldown = null;
    }
}
