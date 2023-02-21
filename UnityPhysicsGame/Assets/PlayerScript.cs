using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : TankScript
{

    [SerializeField]
    private GameObject marker;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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
}
