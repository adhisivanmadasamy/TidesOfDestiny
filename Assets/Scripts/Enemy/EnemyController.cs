using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//States of enemy AI
public enum EnemyStates {  Idle, CombatMovement, Attack, Retreat, Dead, GettingHit}
public class EnemyController : MonoBehaviour
{
    //This class is used for all enemies - Box, Melee, and Gun.
    //Controls the movement,Detection, Target, States, and Alerting other enemies
    //Everything a single enemy needs to do
    [SerializeField] public float Fov { get; private set; } = 180f;
    public List<MeeleFighter> TargetsInRange { get;  set; } = 
        new List<MeeleFighter>();

    //Alert Radius
    [field: SerializeField] public float AlertRange { get; set; } = 20f;

    //Properties
    public MeeleFighter Target { get; set; }
    public StateMachine<EnemyController> stateMachine {  get; private set; }

    Dictionary<EnemyStates, State<EnemyController>> stateDict;

    public NavMeshAgent navAgent { get; private set; }

    public Animator animator {  get; private set; }

    public float CombatMovementTimer { get;  set; } = 0f;

    public MeeleFighter Fighter { get; private set; }

    public VisionSensor visionSensor { get;  set; }

    public CharacterController characterController { get; set; }

    public SkinnedMeshHighlighter skinnedMeshHighlighter { get; set; }

    private void Start()
    {
        //initialising
        animator = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        Fighter = GetComponent<MeeleFighter>();
        characterController = GetComponent<CharacterController>();
        skinnedMeshHighlighter = GetComponent<SkinnedMeshHighlighter>();

        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();
        stateDict[EnemyStates.Idle] = GetComponent<IdleState>();
        stateDict[EnemyStates.CombatMovement] = GetComponent<CombatMovementState>();
        stateDict[EnemyStates.Attack] = GetComponent<AttackingState>();
        stateDict[EnemyStates.Retreat] = GetComponent<RetractState>();
        stateDict[EnemyStates.Dead] = GetComponent<DeadState>();
        stateDict[EnemyStates.GettingHit] = GetComponent<GettingHit>();

        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(stateDict[EnemyStates.Idle]);

        // Handles the fighter response when getting hit
        // updating target and state accordingly
        Fighter.OnGotHit += (MeeleFighter attacker) =>
        {
            if(Fighter.Health >0)
            {
                if(Target == null)
                {
                    Target = attacker;
                    AlertNearbyEnemies();
                }
                ChangeState(EnemyStates.GettingHit);
            }                
            else
                ChangeState(EnemyStates.Dead);
        };
    }

    //Used in multiple areas - to change state of Enemy
    public void ChangeState(EnemyStates state)
    {
        stateMachine.ChangeState(stateDict[state]);
    }

    //To check if the enemy is in a state - Used in multiple places, just to keep it DRY
    public bool IsInState(EnemyStates state)
    {
        return stateMachine.CurrentState == stateDict[state];   
    }

    Vector3 prevPos;

    private void Update()
    {
        stateMachine.Execute();

        //Enemy movements
        var deltaPos = animator.applyRootMotion? 
            Vector3.zero : transform.position - prevPos;
        var velocity = deltaPos / Time.deltaTime;

        float forwardSpeed =  Vector3.Dot(velocity, transform.forward);

        animator.SetFloat("forwardSpeed",
            forwardSpeed / navAgent.speed, 0.2f, Time.deltaTime);
                
        float angle = Vector3.SignedAngle(transform.forward, velocity,
            Vector3.up);
        float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
        animator.SetFloat("strafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);

        //Remove from list of enemies
        if(Target?.Health <= 0)
        {
            TargetsInRange.Remove(Target);
            EnemyManager.i.RemoveEnemyInRange(this);
        }
        prevPos = transform.position;
    }

    //Targeting player
    public MeeleFighter FindTarget()
    {
        foreach (var target in TargetsInRange)
        {
            var vecToTarget = target.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, vecToTarget);

            if (angle <= Fov / 2)
            {
                return target;
            }
        }

        return null;
    }

    //Triggering all enemies in a radius
    public void AlertNearbyEnemies()
    {
        var colliders =  Physics.OverlapBox(transform.position, new Vector3(AlertRange / 2f, 1f, AlertRange / 2f),
            Quaternion.identity, EnemyManager.i.EnemyLayer);

        foreach (var collider in colliders)
        {
            if(collider.gameObject == gameObject) continue;

            var NearbyEnemy = collider.GetComponent<EnemyController>();
            if(NearbyEnemy != null && NearbyEnemy.Target == null)
            {
                NearbyEnemy.Target = Target;
                NearbyEnemy.ChangeState(EnemyStates.CombatMovement);
            }
        }
    }
}
