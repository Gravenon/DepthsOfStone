using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MineGeneration : MonoBehaviour
{
    public MineConfig mineConfig;

    private void Start()
    {
        SpawnOre();
    }

    //add function what will spawn ore and mobe more if player go mine in nigtht time


    // add spawn enemy

    private void SpawnOre()
    {
        foreach (var oreData in mineConfig.ores)
        {
            int count = Random.Range(oreData.minAmount, oreData.maxAmount + 1);

            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    Random.Range(mineConfig.spawnAreaMin.x, mineConfig.spawnAreaMax.x),
                    Random.Range(mineConfig.spawnAreaMin.y, mineConfig.spawnAreaMax.y));

                if (pos != Vector2.zero)
                {
                    GameObject oreObject = Instantiate(oreData.orePrefab, pos, Quaternion.identity);


                }
            }
        }
    }
}
