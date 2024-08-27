using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisionSensor : MonoBehaviour //To detect the player
{
    [SerializeField] EnemyController enemy;
    private void OnTriggerEnter(Collider other)
    {
        var fighter = other.GetComponent<MeeleFighter>();
        if(fighter != null )
        {
            enemy.TargetsInRange.Add( fighter );
            EnemyManager.i.AddEnemyInRange(enemy);
        } 
    }

    private void Awake()
    {
        enemy.visionSensor = this;
    }

    private void OnTriggerExit(Collider other)
    {
        var fighter = other.GetComponent<MeeleFighter>();
        if (fighter != null)
        {
            enemy.TargetsInRange.Remove(fighter);
            EnemyManager.i.RemoveEnemyInRange(enemy);
        }
    }
}
