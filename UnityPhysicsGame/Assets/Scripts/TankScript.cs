using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using static UnityEngine.UI.GridLayoutGroup;

public class TankScript : MonoBehaviour
{
    [SerializeField]
    protected float speed = 10f;

    [SerializeField]
    protected float turnSpeed = 10f;

    [SerializeField]
    protected float barrelTurnSpeed = 10f;

    [SerializeField]
    [Range(0, 360)]
    protected float lookRangeMax = 90f;
    [Range(0, -360)]
    protected float lookRangeMin = -30;

    [SerializeField]
    private float shootCooldown = 0.3f;
    private float shootCooldownCount = 0f;



    [SerializeField]
    protected CharacterController controller;

    [SerializeField]
    protected GameObject barrelParent;
    [SerializeField]
    protected GameObject barrel;

    [SerializeField]
    protected GameObject bodyParent;
    [SerializeField]
    protected GameObject freeBody;

    [SerializeField]
    protected GameObject bulletPrefab;
    [SerializeField]
    protected string bulletType = "Player";

    [SerializeField]
    protected LayerMask markerLayers;

    public bool isDead = false;

    public bool isInvulnerable = true;

    protected virtual void Start()
    {
        StartCoroutine(Invulnerable());
    }
    IEnumerator Invulnerable()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(1);
        isInvulnerable = false;
    }


    // Update is called once per frame
    virtual protected void Update()
    {
        shootCooldownCount += Time.deltaTime;


    }

    protected BulletScript Shoot()
    {
        if(shootCooldownCount >= shootCooldown)
        {
            shootCooldownCount = 0;
            BulletScript bullet = BulletScript.SpawnBullet(bulletType, bulletPrefab);
            bullet.transform.position = barrelParent.transform.position;
            bullet.rb.velocity = barrel.transform.forward * 100f;
            bullet.owner = this;
            return bullet;
        }
        return null;

    }


    public virtual void BulletHitCallback(BulletScript bullet, bool hasHitTank)
    {

    }

    public virtual void Death()
    {
        if (!isInvulnerable)
        {
            isDead = true;
        }

    }
}
