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
    protected CharacterController controller;

    [SerializeField]
    protected GameObject barrelParent;
    [SerializeField]
    protected GameObject barrel;


    [SerializeField]
    protected GameObject bulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void Shoot()
    {
        BulletScript bullet = Instantiate(bulletPrefab).GetComponent<BulletScript>();
        bullet.transform.position = barrelParent.transform.position;
        bullet.rb.velocity = barrel.transform.forward * 100f;
    }
}
