using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public float spawnCooldown = 20f;

    [System.Serializable]
    public struct PowerupData
    {
        public GameObject powerupPrefab;
        public float chance;
    }
    [SerializeField]
    private PowerupData[] powerupPrefabs = new PowerupData[2];

    private void Start()
    {
        StartCoroutine(SpawnPowerup());
    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(spawnCooldown);

        float total = 0;
        float chance = Random.Range(0f, 1f);
        foreach(PowerupData p in powerupPrefabs)
        {
            total += p.chance;
            if(total >= chance)
            {
                GameObject powerup = Instantiate(p.powerupPrefab);
                powerup.transform.position = GetRandomPointInBounds();
                break;
            }
            
        }


        StartCoroutine(SpawnPowerup());
    }

    private Vector3 GetRandomPointInBounds()
    {
        Vector3 pos = transform.position;
        pos.x += Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
        pos.y += Random.Range(-transform.localScale.y / 2, transform.localScale.y / 2);
        pos.z += Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2);
        return pos;
    }
}
