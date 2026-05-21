using UnityEngine;

public class ItemSOLibrary : MonoBehaviour
{
    public static ItemSOLibrary Instance { get; private set; }

    public ItemSO[] itemSOs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Another library already exists (from a previous scene) — destroy this duplicate.
            Destroy(gameObject);
        }
    }
}
