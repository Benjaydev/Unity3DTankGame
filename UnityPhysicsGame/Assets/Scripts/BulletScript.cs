using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Rigidbody rb;
    public Collider col;
    
    public TrailRenderer trailRenderer;

    public static Dictionary<string, List<BulletScript>> inactiveBullets = new Dictionary<string, List<BulletScript>>();
    public static Dictionary<string, List<BulletScript>> activeBullets = new Dictionary<string, List<BulletScript>>();


    public float destroyCooldown = 5f;
    public float destroyCooldownCount = 0f;

    public string bulletType = "Player";

    public TankScript owner;

    [SerializeField]
    public TankScript trackingTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

    }

    public void DestroyBullet()
    {
        AddToPool(this);
        trailRenderer.Clear();
        trackingTarget = null;
    }
    private void Update()
    {
        if(trackingTarget != null)
        {
            Vector3 diff = (trackingTarget.transform.position - transform.position);
            float mag = diff.magnitude;
            Vector3 norm = (diff / mag);
            
            if(mag <= 10)
            {
                rb.velocity = norm * 100;
            }
            else
            {
                rb.velocity += norm * Time.deltaTime * 500;
            }
        }


        destroyCooldownCount += Time.deltaTime;
        if(destroyCooldownCount >= destroyCooldown)
        {
            DestroyBullet();

        }
    }

    public void SetTrackingTarget(TankScript target)
    {
        trackingTarget = target;
        //StartCoroutine(SetTrackingTargetEn(target));
    }
    IEnumerator SetTrackingTargetEn(TankScript target)
    {
        yield return new WaitForSeconds(0.25f);
        trackingTarget = target;
    }


    private void OnCollisionEnter(Collision collision)
    {
        TankScript tank = collision.gameObject.GetComponent<TankScript>();
        if(tank != null)
        {
            tank.Death();
            DestroyBullet();
            owner.BulletHitCallback(this, true);
        }
        else
        {
            owner.BulletHitCallback(this, false);
        }
        
    }

    public static void AddToPool(BulletScript bullet)
    {   
        activeBullets[bullet.bulletType].Remove(bullet);
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
