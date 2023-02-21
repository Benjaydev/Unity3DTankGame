using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBuilder : MonoBehaviour
{
    [Tooltip("a prefab that we clone several times as children of this object")]
    public PhysicsBuilderPart prefab;

    [Tooltip("How many prefabs to clone")]
    public int count;
    [Tooltip("Offset in local space of this object for positioning each child")]
    public Vector3 offset;

    public bool fixStart;
    public bool fixEnd;

    public float breakingForce = 10;

    [ContextMenu("Build")]
    void Build()
    {
        // delete any old ones
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            DestroyObj(rb.gameObject);

        if (prefab == null)
            return;

        PhysicsBuilderPart previous = null;

        for (int i = 0; i < count; i++)
        {
            // clone the prefab and make it a child of us
            PhysicsBuilderPart instance = Instantiate(prefab, transform);

            instance.transform.localPosition = i * offset;
            instance.transform.localRotation = prefab.transform.localRotation;
            instance.name = name + "_" + i;

            Rigidbody rb = instance.GetComponent<Rigidbody>();

            rb.isKinematic = ((i == 0 && fixStart) || (i == count - 1 && fixEnd));

            // attach the previous body to this one
            if (previous)
                foreach (Joint joint in previous.forwardJoints)
                    joint.connectedBody = rb;
            previous = instance;
        }
    }

    [ContextMenu("Set Break Force")]
    public void SetBreakingForce()
    {
        if (breakingForce != 0)
            foreach (Joint joint in GetComponentsInChildren<Joint>())
                joint.breakForce = breakingForce;
    }


    void DestroyObj(Object obj)
    {
        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }
}
