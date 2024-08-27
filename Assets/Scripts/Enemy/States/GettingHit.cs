using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettingHit : State<EnemyController>
{
    [SerializeField] float stunTime = 0.5f;
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        StopAllCoroutines();
        enemy = owner;
        enemy.Fighter.OnHiComplete += () => StartCoroutine(GoToCombatMovement());
    }

    IEnumerator GoToCombatMovement()
    {
        yield return new WaitForSeconds(stunTime);
        if(!enemy.IsInState(EnemyStates.Dead)) 
            enemy.ChangeState(EnemyStates.CombatMovement);
    }
}
