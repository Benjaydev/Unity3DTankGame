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

    public static List<EnemyScript> activeEnemies = new List<EnemyScript>();


    private int currentPathPos = 0;

    private void Awake()
    {
        if(player == null)
        {
            player = FindObjectOfType<PlayerScript>();
        }
        activeEnemies.Add(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Set AI destination
        navMeshAgent.destination = player.transform.position;
        // Get direction to next position in path
        Vector3 nextPosDir = currentPathPos < navMeshAgent.path.corners.Length - 1 ? (navMeshAgent.path.corners[currentPathPos + 1] - transform.position) : Vector3.zero;
        Vector3 nextPosDirNorm = nextPosDir.normalized;
        // Get desired rotation to face next position
        Quaternion targetRotation = Quaternion.LookRotation(nextPosDirNorm, Vector3.up);

        // Rotate to desired rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
        float angle = Vector3.Dot(transform.forward, nextPosDirNorm);

        // If enemy is facing the next path point, move
        if (angle >= 0.99f)
        {
            controller.Move(nextPosDirNorm * Time.deltaTime * speed + new Vector3(0, -9.8f * Time.deltaTime, 0));
        }
        // If has reached next path point
        if (nextPosDir.sqrMagnitude <= 1)
        {
            if (currentPathPos < navMeshAgent.path.corners.Length - 1)
            {
                currentPathPos++;
            }

        }


        // Get direction to player
        Vector3 dirToPlayer = player.transform.position - transform.position;
        Vector3 dirToPlayerNorm = (player.transform.position - transform.position).normalized;
        // Get target direction to face player 
        Quaternion targetBarrelRotation = Quaternion.LookRotation(dirToPlayerNorm, Vector3.up);

        // Lerp barrel rotation towards target
        barrelParent.transform.rotation = Quaternion.Lerp(barrelParent.transform.rotation, targetBarrelRotation, Time.deltaTime * barrelTurnSpeed);

        // If angle is within acceptance range of player and enemy is close enough, shoot
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
        activeEnemies.Remove(this);


        player.AddPoints();

        isDead = true;
        // Detach ragdoll from tank
        ragdoll.transform.parent = null;
        ActivateRigidbody();

        // Play explosion
        explosionParticles.transform.parent = null;
        explosionParticles.Play();

        // Disable tank
        navMeshAgent.updatePosition = false;
        enabled = false;
        transform.position -= new Vector3(0, 100, 0);
        StartCoroutine(DestroyTank());

    }

    IEnumerator DestroyTank()
    {
        yield return new WaitForSeconds(10);
        Destroy(ragdoll);
        Destroy(explosionParticles.gameObject);
        Destroy(gameObject);

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
