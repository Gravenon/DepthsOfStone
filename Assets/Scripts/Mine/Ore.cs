using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Ore : MonoBehaviour
{

    public int durability = 3;

    public ItemSO oreItem;
    public GameObject orePrefab;



    public void Mine()
    {
        durability--;
        if (durability <= 0)
        {
            DropResource();
            Destroy(gameObject);
        }
    }

    private void DropResource()
    {
        if(oreItem == null)
        {
            Debug.LogWarning($"Problem {name}");
        }

        GameObject oreDrop = Instantiate(orePrefab, transform.position, Quaternion.identity);
        Loot loot = oreDrop.GetComponent<Loot>();
        loot.itemSO = oreItem;
        loot.quantity = Random.Range(1, 4);
        
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