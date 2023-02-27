using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : TankScript
{



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

    private float airstrikeRadius = 10f;

    public int points = 0;

    public float airstrikeProgress = 0;




    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalCameraPosition = airstrikeCamera.transform.position;
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
                airstrikeExplosion.transform.position = airHit.point;
                airstrikeExplosion.Play();
                // Collide with tanks inside explosion radius
                RaycastHit[] hits = Physics.SphereCastAll(airHit.point, airstrikeRadius, Vector3.down, 1);
                for(int i = 0; i < hits.Length; i++)
                {
                    Rigidbody rb = hits[i].collider.gameObject.GetComponent<Rigidbody>();
                    if(rb != null)
                    {
                        rb.AddForce((rb.transform.position-airHit.point)*100, ForceMode.Impulse);
                        rb.AddTorque((rb.transform.position-airHit.point)*100, ForceMode.Impulse);
                    }


                    TankScript tank = hits[i].collider.gameObject.GetComponent<TankScript>();
                    if (tank != null)
                    {
                        tank.Death();
                    }
                }

                EndAirstrike();
            }

            return;
        }
        marker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        float vAxis = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float hAxis = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
        float rAxis = Input.GetAxis("Rotate") * barrelTurnSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y");

        //Debug.Log(rAxis);
        controller.Move(freeBody.transform.forward * vAxis + new Vector3(0, -9.8f * Time.deltaTime, 0));
        freeBody.transform.Rotate(new Vector3(0, hAxis, 0));


        barrelParent.transform.RotateAround(barrelParent.transform.position, transform.up, rAxis);


        barrelParent.transform.rotation = Quaternion.Euler(barrelParent.transform.rotation.eulerAngles.x - mouseY, barrelParent.transform.rotation.eulerAngles.y, barrelParent.transform.rotation.eulerAngles.z);

        float normAngle = barrelParent.transform.rotation.eulerAngles.x;
        if(normAngle >= 180f)
        {
            normAngle -= 360f;
        }

        float rot = barrelParent.transform.rotation.eulerAngles.x;
        if(normAngle >= 0)
        {
            rot = 0f;
        }
        else if(normAngle <= -lookRangeMax)
        {
            rot = -lookRangeMax;
        }

        barrelParent.transform.rotation = Quaternion.Euler(rot, barrelParent.transform.rotation.eulerAngles.y, barrelParent.transform.rotation.eulerAngles.z);


        RaycastHit hit;
        Debug.DrawRay(barrel.transform.position, barrelParent.transform.forward, Color.red, 10);
        if (Physics.Raycast(barrel.transform.position, barrel.transform.forward, out hit, 1000f, markerLayers))
        {
            marker.transform.position = hit.point;
        }
        else
        {
            marker.transform.localPosition = Vector3.zero;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

    }

    public void AddPoints()
    {
        points += (int)(points*1.01);
        airstrikeProgress += 0.1f;
    }

    public void StartAirstrike()
    {
        airstrike = true;
        airstrikeProgress = 0;

        Cursor.lockState = CursorLockMode.Confined;
        mainCamera.gameObject.SetActive(false);
        airstrikeCamera.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    public void EndAirstrike()
    {
        airstrike = false;

        Cursor.lockState = CursorLockMode.Locked;
        mainCamera.gameObject.SetActive(true);
        airstrikeCamera.gameObject.SetActive(false);
        Time.timeScale = 1f;


        airstrikeCamera.transform.position = originalCameraPosition;
    }
}
