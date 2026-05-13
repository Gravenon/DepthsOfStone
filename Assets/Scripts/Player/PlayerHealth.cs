using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{ 
    [SerializeField] private bool isInvulnerable;
    [SerializeField] private float damageInvulnerabilityDuration = 0.5f;

    public TMP_Text healthText;
    public Slider healthBar;
    public Animator healthTextAnim;
    

    public static event Action Died;

    public bool isDead { get; private set; }

    private void Start()
    {
        RefreshHealthBar();
    }

    private void OnEnable()
    {
        StatsManager.OnHealthChanged += RefreshHealthBar;
        // Subscribe here (not Awake) so it works for both regular and DontDestroyOnLoad objects.
        // For DDOL objects Start() is only called once — this handler fires on every scene load.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        StatsManager.OnHealthChanged -= RefreshHealthBar;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Resets stale flags on every scene load.
    // Required for DontDestroyOnLoad objects where Start() is only called once.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isDead = false;
        isInvulnerable = false;
        StopAllCoroutines();
        Time.timeScale = 1f;
    }

    private void RefreshHealthBar()
    {
        if (StatsManager.Instance == null) return;
        healthBar.maxValue = StatsManager.Instance.maxHealth;
        healthBar.value    = StatsManager.Instance.currentHealth;
        healthText.text    = "HP: " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
    }

    public void ChangeHealth(float amount)
    {
        if (isDead || isInvulnerable) return;

        StatsManager.Instance.currentHealth = Mathf.Clamp(
            StatsManager.Instance.currentHealth + (int)amount,
            0, StatsManager.Instance.maxHealth);

        healthTextAnim.Play("TextUpdate");
        RefreshHealthBar();

        if (amount < 0)
            StartCoroutine(InvulnerabilityCoroutine(damageInvulnerabilityDuration));

        if (StatsManager.Instance.currentHealth <= 0)
            Die();
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
        isInvulnerable = false;
        StopAllCoroutines();
        healthTextAnim.Play("TextUpdate");
        RefreshHealthBar();
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
