using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<EnemyController>
{
    public override void Enter(EnemyController owner)
    {
        owner.visionSensor.gameObject.SetActive(false);
        EnemyManager.i.RemoveEnemyInRange(owner);
        owner.navAgent.enabled = false;
        owner.characterController.enabled = false;
    }
}
