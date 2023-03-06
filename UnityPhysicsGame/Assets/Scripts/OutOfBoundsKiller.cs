using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class OutOfBoundsKiller : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        TankScript tank = other.gameObject.GetComponent<TankScript>();
        if (tank != null)
        {
            tank.Death();
        }
    }

}
