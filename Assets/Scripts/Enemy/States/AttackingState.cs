using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : State<EnemyController>
{
    [SerializeField] float attackDistance = 1.5f;
    EnemyController enemy;

    bool isAttacking;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.navAgent.stoppingDistance = attackDistance;
    }

    public override void Execute()
    {
        if (isAttacking) return;

        enemy.navAgent.SetDestination(enemy.Target.transform.position);

        if(Vector3.Distance(enemy.Target.transform.position, 
            enemy.transform.position)<= attackDistance + 0.03f)
        {
            StartCoroutine(Attack(Random.Range(0,3)));
        }
    }

    IEnumerator Attack(int comboCount = 1)
    {
        isAttacking = true;
        enemy.animator.applyRootMotion = true;

        enemy.Fighter.TryToAttack(enemy.Target);

        for(int i = 0; i < comboCount; i++)
        {
            yield return new WaitUntil(() => enemy.Fighter.attackState
                == AttackState.EndAttack);
            enemy.Fighter.TryToAttack(enemy.Target);
        }

        yield return new WaitUntil(() => enemy.Fighter.attackState 
            == AttackState.Idle);

        enemy.animator.applyRootMotion = false;
        isAttacking = false;

        if(enemy.IsInState(EnemyStates.Attack))
        {
            enemy.ChangeState(EnemyStates.Retreat);
        }
    }

    public override void Exit()
    {
        enemy.navAgent.ResetPath();
    }
}
