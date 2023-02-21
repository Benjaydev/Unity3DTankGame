using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    [System.Serializable]
    public struct ResetableObject
    {
        public Rigidbody obj;
        public Vector3 originalPos;
        public Quaternion originalRot;
    }

    [SerializeField]
    private ResetableObject[] resetableObjects;

    [SerializeField]
    private float resetCooldown = 30f;


    private void Awake()
    {
        StartCoroutine(ResetBodies());
    }

    IEnumerator ResetBodies()
    {
        yield return new WaitForSeconds(resetCooldown);

        for(int i = 0; i < resetableObjects.Length; i++)
        {
            resetableObjects[i].obj.transform.position = resetableObjects[i].originalPos;
            resetableObjects[i].obj.transform.rotation = resetableObjects[i].originalRot;
            resetableObjects[i].obj.velocity = Vector3.zero;
        }

        StartCoroutine(ResetBodies());
    }



    [ContextMenu("Get Rigidbodies")]
    void GetRigidbodies()
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        resetableObjects = new ResetableObject[bodies.Length];
        for (int i = 0; i < bodies.Length; i++)
        {
            resetableObjects[i].obj = bodies[i];
            resetableObjects[i].originalPos = bodies[i].transform.position;
            resetableObjects[i].originalRot = bodies[i].transform.rotation;
        }
    }
}
