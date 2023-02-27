using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

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

    private Quaternion targetLocation;

    public bool isDead = false;

    // Update is called once per frame
    virtual protected void Update()
    {
        shootCooldownCount += Time.deltaTime;


        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, 100, markerLayers))
        {
            targetLocation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }

        bodyParent.transform.rotation = Quaternion.Lerp(bodyParent.transform.rotation, targetLocation, Time.deltaTime);
    }

    protected void Shoot()
    {
        if(shootCooldownCount >= shootCooldown)
        {
            shootCooldownCount = 0;
            BulletScript bullet = BulletScript.SpawnBullet(bulletType, bulletPrefab);
            bullet.transform.position = barrelParent.transform.position;
            bullet.rb.velocity = barrel.transform.forward * 100f;
        }

    }


    public virtual void Death()
    {

    }
}
