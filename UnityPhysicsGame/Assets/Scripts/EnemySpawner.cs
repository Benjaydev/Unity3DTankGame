using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    public static EnemySpawner instance;

    [System.NonSerialized]
    public int enemiesInScene = 0;
    [SerializeField]
    private int maxEnemiesInScene = 5;

    [SerializeField]
    private Transform[] spawnPoints = new Transform[1];

    [SerializeField]
    private LayerMask spawnBlockers;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        if(enemiesInScene < maxEnemiesInScene)
        {
            Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].position;

            // Check that no other enemy is at this spawn point
            if (!Physics.Raycast(pos+new Vector3(0,10,0),Vector3.down, 15, spawnBlockers))
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.transform.position = pos;

                enemiesInScene++;
            }

        }
    }

}
