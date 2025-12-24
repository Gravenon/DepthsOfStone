using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [Header("Combat Stats")]
    public int damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTimre;
    public float stunTime;

    [Header("Movment Stats")]
    public int speed;
    public float[] position;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;

    public PlayerData(StatsManager stats, Transform playerTransform)
    {
        damage = stats.damage;
        weaponRange = stats.weaponRange;
        knockbackForce = stats.knockbackForce;
        knockbackTimre = stats.knockbackTimre;
        stunTime = stats.stunTime;

        speed = stats.speed;
        position = new float[3];
        position[0] = playerTransform.position.x;
        position[1] = playerTransform.position.y;
        position[2] = playerTransform.position.z;

        maxHealth = stats.maxHealth;
        currentHealth = stats.currentHealth;
    }

}
