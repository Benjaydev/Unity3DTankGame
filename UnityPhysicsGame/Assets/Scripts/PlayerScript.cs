using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScript : TankScript
{
    public static PlayerScript instance;

    [SerializeField]
    private GameObject marker;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Camera airstrikeCamera;
    private Vector3 originalCameraPosition;
    private bool airstrike = false;
    [SerializeField]
    private ParticleSystem airstrikeExplosion;
    [SerializeField]
    private float airstrikeRadius = 10f;

    private int points = 0;
    private float killMultiplier = 1f;

    private float airstrikeProgress = 0;

    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private UIScript uiScript;

    public bool explosiveShots = false;
    public float explosiveShotsDuration = 20f;

    public bool trackingShots = false;
    public float trackingShotsDuration = 10f;
    public LayerMask trackingLayers;


    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        originalCameraPosition = airstrikeCamera.transform.position;

        uiScript.UpdatePoints(points);
        uiScript.UpdateAirstrikeProgress(airstrikeProgress);

        instance = this;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (airstrikeProgress >= 1 && Input.GetKeyDown(KeyCode.T))
        {
            StartAirstrike();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            EndAirstrike();
        }

        // If player is in airstrike mode
        if (airstrike)
        {
            airstrikeCamera.transform.position -= new Vector3(0, 30 * Time.unscaledDeltaTime, 0);

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = airstrikeCamera.transform.position.y;
            Vector3 worldPosition = airstrikeCamera.ScreenToWorldPoint(mousePos);

            RaycastHit airHit;
            Physics.Raycast(airstrikeCamera.transform.position, worldPosition - airstrikeCamera.transform.position, out airHit);
            Debug.DrawLine(airstrikeCamera.transform.position, airstrikeCamera.transform.position + (worldPosition - airstrikeCamera.transform.position) * 1000, Color.red);
            marker.transform.position = airHit.point;
            marker.transform.localScale = new Vector3(10, 10, 10);

            if (Input.GetMouseButtonDown(0))
            {
                SpawnExplosion(airHit.point);

                EndAirstrike();
            }

            return;
        }
        // Reset marker size
        marker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        float vAxis = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float hAxis = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
        float rAxis = Input.GetAxis("Rotate") * barrelTurnSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y");

        controller.Move(freeBody.transform.forward * vAxis + new Vector3(0, -9.8f * Time.deltaTime, 0));
        freeBody.transform.Rotate(new Vector3(0, hAxis, 0));


        barrelParent.transform.RotateAround(barrelParent.transform.position, transform.up, rAxis);
        barrelParent.transform.rotation = Quaternion.Euler(barrelParent.transform.rotation.eulerAngles.x - mouseY, barrelParent.transform.rotation.eulerAngles.y, barrelParent.transform.rotation.eulerAngles.z);

        // Rotate barrel to mouse
        float normAngle = barrelParent.transform.rotation.eulerAngles.x;
        if(normAngle >= 180f)
        {
            normAngle -= 360f;
        }

        float rot = barrelParent.transform.rotation.eulerAngles.x;
        if(normAngle >= -lookRangeMin)
        {
            rot = -lookRangeMin;
        }
        else if(normAngle <= -lookRangeMax)
        {
            rot = -lookRangeMax;
        }
        barrelParent.transform.rotation = Quaternion.Euler(rot, barrelParent.transform.rotation.eulerAngles.y, barrelParent.transform.rotation.eulerAngles.z);

        // Set position of aim marker in world
        RaycastHit hit;
        if (Physics.Raycast(barrel.transform.position, barrel.transform.forward, out hit, 1000f, markerLayers))
        {
            marker.transform.position = hit.point;
        }
        else
        {
            marker.transform.localPosition = Vector3.zero;
        }



        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            // Shoot bullet
            BulletScript bullet = Shoot();

            // Tracking shot
            if(bullet != null && trackingShots && EnemyScript.activeEnemies.Count > 0)
            {
                // get point where player is looking
                RaycastHit bulletHit;
                Physics.Raycast(barrel.transform.position, barrelParent.transform.forward, out bulletHit, 1000f, trackingLayers);

                Debug.DrawLine(bulletHit.point, bulletHit.point+Vector3.up*20, Color.red);

                // Find the closest enemy to that looking point
                EnemyScript closestEnemy = EnemyScript.activeEnemies[0];
                float closestDist = (closestEnemy.transform.position - bulletHit.point).sqrMagnitude;
                for(int i = 1; i < EnemyScript.activeEnemies.Count; i++)
                {
                    float dist = (EnemyScript.activeEnemies[i].transform.position - bulletHit.point).sqrMagnitude;
                    if (dist < closestDist)
                    {
                        closestEnemy = EnemyScript.activeEnemies[i];
                        closestDist = dist;
                    }
                }
                // Set target to closest
                bullet.SetTrackingTarget(closestEnemy);
            }
        }

    }

    // Can perform any actions after a shot bullet hit something
    public override void BulletHitCallback(BulletScript bullet, bool hasHitTank)
    {
        base.BulletHitCallback(bullet, hasHitTank);

        // Spawn explosion on bullet hit
        if (explosiveShots)
        {
            SpawnExplosion(bullet.transform.position);
            bullet.DestroyBullet();
        }
    }

    public void SpawnExplosion(Vector3 position)
    {
        // Spawn explosion effect
        GameObject explosion = Instantiate(explosionEffect);
        explosion.transform.position = position;
        ParticleSystem particles = explosion.GetComponent<ParticleSystem>();
        particles.Play();

        // Destroy explosion effect
        Destroy(explosion, 5);
        // Explode at right time
        StartCoroutine(Explode(particles.main.duration*2, position));
        
    }

    IEnumerator Explode(float time, Vector3 position)
    {
        yield return new WaitForSeconds(time);
        // Collide with tanks inside explosion radius
        RaycastHit[] hits = Physics.SphereCastAll(position, airstrikeRadius, Vector3.down, 1);
        for (int i = 0; i < hits.Length; i++)
        {
            Rigidbody rb = hits[i].collider.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce((rb.transform.position - position) * 100, ForceMode.Impulse);
                rb.AddTorque((rb.transform.position - position) * 100, ForceMode.Impulse);
            }


            TankScript tank = hits[i].collider.gameObject.GetComponent<TankScript>();
            if (tank != null)
            {
                tank.Death();
            }
        }
    }

    public void TriggerPowerup(string type)
    {
        if(type == "Explosive")
        {
            StartCoroutine(ActivateExplosiveShots());
        }
    }

    IEnumerator ActivateExplosiveShots()
    {
        explosiveShots = true;
        uiScript.SetIcon("Explosive", true);
        yield return new WaitForSeconds(explosiveShotsDuration);
        explosiveShots = false;
        uiScript.SetIcon("Explosive", false);
    }


    public void AddPoints()
    {
        killMultiplier += 0.1f;
        points += (int)(1 * killMultiplier);
        airstrikeProgress = Mathf.Min(airstrikeProgress + 0.1f, 1);
        uiScript.UpdatePoints(points);
        uiScript.UpdateAirstrikeProgress(airstrikeProgress);
    }

    public void StartAirstrike()
    {
        airstrike = true;
        airstrikeProgress = 0;

        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        mainCamera.gameObject.SetActive(false);
        airstrikeCamera.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    public void EndAirstrike()
    {
        airstrike = false;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        mainCamera.gameObject.SetActive(true);
        airstrikeCamera.gameObject.SetActive(false);
        Time.timeScale = 1f;


        airstrikeCamera.transform.position = originalCameraPosition;
    }
}
