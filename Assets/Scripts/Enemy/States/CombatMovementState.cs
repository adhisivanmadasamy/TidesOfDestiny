using UnityEngine;

public enum AICombatStates { Idle, Chase, Circling }
public class CombatMovementState : State<EnemyController>
{
    [Header("CombatMovement Controls")]
    [SerializeField] float distanceToStand = 2f;
    [SerializeField] float adjustDistanceThreshol = 1f;
    [SerializeField] Vector2 idleTimeRange = new Vector2(2, 5);
    [SerializeField] Vector2 circlingTimeRange = new Vector2(3, 6);
    [SerializeField] float CirclingSpeed = 20f;
    AICombatStates state;

    EnemyController enemy;

    float timer = 0f;
    int circlingDir = 1;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.navAgent.stoppingDistance = distanceToStand;
        enemy.CombatMovementTimer = 0f;
        enemy.animator.SetBool("CombatMode", true);
    }

    public override void Execute()
    {
        if(enemy.Target.Health <= 0)
        {
            enemy.Target = null;
            enemy.ChangeState(EnemyStates.Idle);
            return ;
        }

        if (Vector3.Distance(enemy.Target.transform.position,
            enemy.transform.position) > distanceToStand + adjustDistanceThreshol) 
        {
            StartChase();
        }

        if(state == AICombatStates.Idle)
        {
            if (timer <= 0f)
            {
                if(Random.Range(0,2)==0)
                {
                    StartIdle();
                }
                else
                {
                    StartCircling();
                }
            }
        }
        else if(state == AICombatStates.Chase)
        {
            if (Vector3.Distance(enemy.Target.transform.position,
            enemy.transform.position) <= distanceToStand + 0.03f)
            {
                StartIdle();
                return;
            }

            enemy.navAgent.SetDestination(enemy.Target.transform.position);
        }
        else if(state == AICombatStates.Circling)
        {
            if(timer <= 0f)
            {
                StartIdle();
                return ;
            }
            
            var vecToTarget = enemy.transform.position - enemy.Target.transform.position;

            var rotatedPos =  Quaternion.Euler(0, CirclingSpeed * circlingDir * 
                Time.deltaTime,0) * vecToTarget;

            enemy.navAgent.Move(rotatedPos - vecToTarget);
            enemy.transform.rotation = Quaternion.LookRotation(-rotatedPos);
        }

        if(timer > 0f)
        {
            timer -= Time.deltaTime;
        }

        enemy.CombatMovementTimer += Time.deltaTime;
        
    }

    void StartChase()
    {
        state = AICombatStates.Chase;        
    }

    void StartIdle()
    {
        state = AICombatStates.Idle; 
        timer = Random.Range(idleTimeRange.x, idleTimeRange.y);
    }

    void StartCircling()
    {
        state = AICombatStates.Circling;
        enemy.navAgent.ResetPath();
        timer = Random.Range(circlingTimeRange.x, circlingTimeRange.y);
        circlingDir = Random.Range(0, 2) == 0? 1 : -1;
    }
    public override void Exit()
    {
        Debug.Log("Exiting Chase state");
        enemy.CombatMovementTimer = 0f;
    }
}

