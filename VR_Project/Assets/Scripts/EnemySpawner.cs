using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn = null;
    public int amountToSpawn = 10;
    public bool spawnEnemy = true;
    public void Start()
    {
        if (spawnEnemy)
            for (int i = 0; i < amountToSpawn; i++)
                Instantiate(enemyToSpawn, new Vector3(0, 1, 0), new Quaternion(0,0,0,0));
    }
    
}
