using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyScript : TankScript
{
    [SerializeField]
    [Range(0f, 1f)]
    private float barrelRotateAcceptance = 0.1f;

    [SerializeField]
    private float shootRange = 25f;

    [Header("Ragdoll")]
    [SerializeField]
    private GameObject ragdoll;
    [SerializeField]
    private Animator ragdollAnimator;
    [SerializeField]
    private Rigidbody[] ragdollBodies = new Rigidbody[0];
    [SerializeField]
    private Collider[] ragdollColliders = new Collider[0];
    [SerializeField]
    private float ragdollForce = 50f;

    protected static PlayerScript player;

    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private ParticleSystem explosionParticles;

    private void Awake()
    {
        if(player == null)
        {
            player = FindObjectOfType<PlayerScript>();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Set AI destination
        navMeshAgent.destination = player.transform.position;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        // Get direction to next position in path
        Vector3 nextPosDir = (navMeshAgent.nextPosition - transform.position).normalized;
        // Get desired rotation to face next position
        Quaternion targetRotation = Quaternion.LookRotation(nextPosDir, Vector3.up);

        // Rotate to desired rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
        float angle = Vector3.Dot(transform.forward, nextPosDir);

        if(angle >= 0.99f)
        {
            controller.Move(nextPosDir * Time.deltaTime * speed + new Vector3(0, -9.8f * Time.deltaTime, 0));
        }

        Vector3 dirToPlayer = player.transform.position - transform.position;
        Vector3 dirToPlayerNorm = (player.transform.position - transform.position).normalized;
        Quaternion targetBarrelRotation = Quaternion.LookRotation(dirToPlayerNorm, Vector3.up);

        barrelParent.transform.rotation = Quaternion.Lerp(barrelParent.transform.rotation, targetBarrelRotation, Time.deltaTime * barrelTurnSpeed);

        float barrelAngle = Vector3.Dot(barrelParent.transform.forward, dirToPlayerNorm);
        if (barrelAngle > 1 - barrelRotateAcceptance && dirToPlayer.sqrMagnitude <= shootRange* shootRange)
        {
            Shoot();
        }
    }

    public override void Death()
    {
        if (isDead)
        {
            return;
        }

        player.AddPoints();

        isDead = true;
        // Detach ragdoll from tank
        ragdoll.transform.parent = null;
        ActivateRigidbody();

        // Play explosion
        explosionParticles.transform.parent = null;
        explosionParticles.Play();

        // Disable tank
        enabled = false;
        transform.position -= new Vector3(0, 100, 0);
        StartCoroutine(DestroyTank());


    }

    IEnumerator DestroyTank()
    {
        yield return new WaitForSeconds(10);
        Destroy(ragdoll);
        Destroy(explosionParticles.gameObject);
        Destroy(this);

        EnemySpawner.instance.enemiesInScene--;
    }

    public void ActivateRigidbody()
    {
        ragdollAnimator.enabled = false;
        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            ragdollBodies[i].isKinematic = false;
            // Get force direction
            Vector3 diff =  ragdoll.transform.position - transform.position;
            // Go upwards
            diff.y += 1;
            // Add force to ragdoll parts
            ragdollBodies[i].AddForce(diff*ragdollForce, ForceMode.Impulse);
            ragdollBodies[i].AddTorque(diff * ragdollForce, ForceMode.Impulse);
            ragdollColliders[i].enabled = true;
        }
    }
}
