using System;
using UnityEngine;

public class Boss_Health : MonoBehaviour
{
    public int health = 500;
    
    // public GameObject deathEffect;
    
    public bool isInvulneable = false;
    
    public BossHealthBar healthBar;

    private void Start()
    {
        healthBar.SetMaxHealth(health);
    }

    public void ChangeHealth(float amount)
    {
        if(isInvulneable)
            return;
        
        health += (int)amount;
        
        healthBar.SetHealth(health);

        if (health <= 250)
        {
            GetComponent<Animator>().SetBool("InRage", true);
        }
        
        if (health <= 0)
        {
            health = 0;
            healthBar.SetHealth(health);
            
            Destroy(GetComponent<Collider2D>());
            
            Die();
        }
    }
    
    void Die()
    {
        isInvulneable = true;
        GetComponent<Animator>().SetBool("IsDead", true);
    }
}
