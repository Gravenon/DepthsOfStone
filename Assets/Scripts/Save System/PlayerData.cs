using UnityEngine;

[System.Serializable]
public class PlayerData
{

    [Header("Scene")]
    public string sceneName;

    [Header("Position")]
    public float[] position;

    [Header("Stats")]
    public int maxHealth;
    public int currentHealth;

    public int speed;

    public int damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTimre;
    public float stunTime;

    public PlayerData(StatsManager stats, Transform playerTransform)
    {
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        position = new float[3];
        position[0] = playerTransform.position.x;
        position[1] = playerTransform.position.y;
        position[2] = playerTransform.position.z;

        maxHealth = stats.maxHealth;
        currentHealth = stats.currentHealth;

        speed = stats.speed;

        damage = stats.damage;
        weaponRange = stats.weaponRange;
        knockbackForce = stats.knockbackForce;
        knockbackTimre = stats.knockbackTimre;
        stunTime = stats.stunTime;
    }

}
