using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetractState : State<EnemyController>
{
    [SerializeField] float backwardSpeed = 1.5f;

    [SerializeField] float distanceToRetreat = 3f;

    EnemyController enemy;

    Vector3 targetPos;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        targetPos = enemy.Target.transform.position;
    }

    public override void Execute()
    {
        if(Vector3.Distance(enemy.transform.position, targetPos)
            >= distanceToRetreat)
        {
            enemy.ChangeState(EnemyStates.CombatMovement);
            return;
        }

        var vecToTarget = enemy.Target.transform.position - enemy.transform.position;
        enemy.navAgent.Move(-vecToTarget.normalized * backwardSpeed * Time.deltaTime);

        vecToTarget.y = 0f;
        transform.rotation =  Quaternion.RotateTowards(transform.rotation,  
            Quaternion.LookRotation(vecToTarget), 500 * Time.deltaTime);
    }
}
