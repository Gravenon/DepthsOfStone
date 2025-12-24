using UnityEngine;

public class TriggerSave : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SaveSystem.SavePlayer(StatsManager.Instance, other.transform);
        }

        Destroy(gameObject);
    }
}
