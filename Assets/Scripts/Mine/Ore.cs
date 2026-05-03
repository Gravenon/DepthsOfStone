using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Ore : MonoBehaviour
{
    [Header("Drop Settings")]
    public ItemSO oreItem;
    public GameObject orePrefab;
    public int minDropQuantity = 1;
    public int maxDropQuantity = 4;

    public void Mine()
    {
        DropResource();
        Destroy(gameObject);
    }

    private void DropResource()
    {
        if(oreItem == null)
        {
            Debug.LogWarning($"Ore item is null on {name}");
            return;
        }

        if(orePrefab == null)
        {
            Debug.LogWarning($"Ore prefab is null on {name}");
            return;
        }

        GameObject oreDrop = Instantiate(orePrefab, transform.position, Quaternion.identity);
        Loot loot = oreDrop.GetComponent<Loot>();
        loot.itemSO = oreItem;
        loot.quantity = Random.Range(minDropQuantity, maxDropQuantity + 1);
        
    }

    public enum OreType
    {
        Copper,
        Iron,
        Gold,
        Silver,
        Diamond
    }
}