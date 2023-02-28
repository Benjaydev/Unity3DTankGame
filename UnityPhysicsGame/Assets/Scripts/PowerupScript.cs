using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupScript : MonoBehaviour
{
    public string type;


    [SerializeField]
    protected float rotationSpeed = 10f;


    // Update is called once per frame
    protected virtual void Update()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + (rotationSpeed * Time.deltaTime), 0);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == PlayerScript.instance.gameObject)
        {
            PlayerScript.instance.TriggerPowerup(type);
            Destroy(gameObject);
        }

    }


}
