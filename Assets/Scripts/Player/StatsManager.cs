using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    public UseItem useItem;
    public StatsUI statsUI;
    public TMP_Text healthText;

    [Header("Combat Stats")]
    public int damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTimre;
    public float stunTime;

    [Header("Movment Stats")]
    public int speed;
    [SerializeField] public Transform playerTransform;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        healthText.text = "HP: " + currentHealth + "/ " + maxHealth;
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;

        healthText.text = "HP: " + currentHealth + "/ " + maxHealth;
    }

    public void UpdateSpeed(int amount)
    {
        speed += amount;
        statsUI.UpdataAllStats();
    }

    public void UpdateDamage(int amount)
    {
        damage += amount;
        statsUI.UpdataAllStats();
    }

    public void ApplyPlayerData(PlayerData data)
    {
        if (data == null) return;

        damage = data.damage;
        weaponRange = data.weaponRange;
        knockbackForce = data.knockbackForce;
        knockbackTimre = data.knockbackTimre;
        stunTime = data.stunTime;

        speed = data.speed;

        playerTransform.position = new Vector3(
           data.position[0],
           data.position[1],
           data.position[2]
       );

        maxHealth = data.maxHealth;
        currentHealth = data.currentHealth;
    }

}
