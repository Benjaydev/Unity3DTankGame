using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Rigidbody rb;
    public Collider col;
    

    public static Dictionary<string, List<BulletScript>> inactiveBullets = new Dictionary<string, List<BulletScript>>();
    public static Dictionary<string, List<BulletScript>> activeBullets = new Dictionary<string, List<BulletScript>>();


    public float destroyCooldown = 5f;
    public float destroyCooldownCount = 0f;

    public string bulletType = "Player";
    private int poolPosition = 0;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

    }

    public void DestroyBullet()
    {
        AddToPool(this);
    }
    private void Update()
    {
        destroyCooldownCount += Time.deltaTime;
        if(destroyCooldownCount >= destroyCooldown)
        {
            DestroyBullet();

        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        TankScript tank = collision.gameObject.GetComponent<TankScript>();
        if(tank != null)
        {
            tank.Death();
        }
    }

    public static void AddToPool(BulletScript bullet)
    {
        activeBullets[bullet.bulletType].RemoveAt(bullet.poolPosition);
        inactiveBullets[bullet.bulletType].Add(bullet);
        bullet.gameObject.SetActive(false);
    }

    public static BulletScript SpawnBullet(string type, GameObject prefab)
    {
        BulletScript bullet;
        // If pool of type doesn't exist yet
        if (!activeBullets.ContainsKey(type))
        {
            activeBullets.Add(type, new List<BulletScript>());
            inactiveBullets.Add(type, new List<BulletScript>());
        }

        if (inactiveBullets[type].Count > 0)
        {
            bullet = inactiveBullets[type][0];
            inactiveBullets[type].RemoveAt(0);
            bullet.gameObject.SetActive(true);

        }
        else
        {
            bullet = Instantiate(prefab).GetComponent<BulletScript>();
        }

        activeBullets[type].Add(bullet);
        bullet.destroyCooldownCount = 0f;

        return bullet;
    }


}
