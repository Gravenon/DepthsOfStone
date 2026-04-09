using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class PlayerHealth : MonoBehaviour
{ 
    [SerializeField] private bool isInvulnerable;
    
    public TMP_Text healthText;
    public Animator healthTextAnim;

    public static event Action Died;

    public bool isDead { get; private set; }

    private void Start()
    {
        healthText.text = "HP: " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        if(isDead || isInvulnerable) return;

        StatsManager.Instance.currentHealth += amount;
        StatsManager.Instance.currentHealth = Mathf.Clamp(StatsManager.Instance.currentHealth, 0, StatsManager.Instance.maxHealth);

        healthTextAnim.Play("TextUpdate");
        healthText.text = "HP: " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;

        if (StatsManager.Instance.currentHealth <= 0)
        {
            Die();   
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Died?.Invoke();
    }

    public void Revive()
    {
        isDead = false;

        healthTextAnim.Play("TextUpdate");
        healthText.text = "HP: " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
    }

    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
    }

    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }


}
