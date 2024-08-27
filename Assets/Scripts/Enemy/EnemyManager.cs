using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    //This class is to manage all the enemies in the scene
    //Adding & removing enemies, from the list getting the nearest enemy, 
    //attacking enemy, and directions 
    //These are used in places, like to make the player face the enemy when got hit
    //move the player close to the player when attacking and more

    [SerializeField] Vector2 timeRangeBetweenAttacks = new Vector2(1, 4);
    public static EnemyManager i { get; private set; }
    public List<EnemyController> enemiesInRange = new List<EnemyController>();
    float notAttackingTimer = 2;
    [SerializeField] CombatController CombatController;
    [field: SerializeField] public LayerMask EnemyLayer {  get; private set; }

    private void Awake()
    {
        i = this;
    }

    //Adding the enemy to list when triggered
    public void AddEnemyInRange(EnemyController enemy)
    {
        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    //Removing the enemy from list when dead
    public void RemoveEnemyInRange(EnemyController enemy)
    {
        enemiesInRange.Remove(enemy);

        if(enemy == CombatController.TargetEnemy)
        {
            enemy.skinnedMeshHighlighter?.HighlightMesh(false);
            CombatController.TargetEnemy = GetClosestEnemyToDirection
                (CombatController.GetTargetingDir());
            CombatController.TargetEnemy?.skinnedMeshHighlighter.HighlightMesh(true);
        }
    }

    float timer = 0f;
    private void Update()
    {
        if(enemiesInRange.Count == 0) return;

        if(!enemiesInRange.Any(e => e.IsInState(EnemyStates.Attack)))
        {
            if(notAttackingTimer > 0) 
                notAttackingTimer -= Time.deltaTime;
                
            if(notAttackingTimer <= 0)
            {
                var attackingEnemy = SelectEnemyForAttack();

                if(attackingEnemy != null)
                {
                    attackingEnemy.ChangeState(EnemyStates.Attack);
                    notAttackingTimer = Random.Range(timeRangeBetweenAttacks.x,
                        timeRangeBetweenAttacks.y);
                }
            }
        }

        if(timer >= 0.1f)
        {
            timer = 0f;
            var closestEnemy = GetClosestEnemyToDirection
                (CombatController.GetTargetingDir());
            if(closestEnemy != null && closestEnemy != CombatController.TargetEnemy)
            {
                var prevEnemy = CombatController.TargetEnemy;
                CombatController.TargetEnemy = closestEnemy;

                CombatController?.TargetEnemy?.skinnedMeshHighlighter.HighlightMesh(true);
                prevEnemy?.skinnedMeshHighlighter?.HighlightMesh(false);
            }
        }
        timer += Time.deltaTime;
    }

    EnemyController SelectEnemyForAttack()
    {
        return enemiesInRange.OrderByDescending
            (e => e.CombatMovementTimer).FirstOrDefault(e => e.Target != null
            && e.IsInState(EnemyStates.CombatMovement));
    }

    public EnemyController GetAttackingEnemy()
    {
        return enemiesInRange.FirstOrDefault(e => e.IsInState(EnemyStates.Attack));
    }

    public EnemyController GetClosestEnemyToDirection(Vector3 direction)
    {
        float minDistance = Mathf.Infinity;
        EnemyController closestEnemy = null;

        foreach(var enemy in enemiesInRange)
        {
            var vecToEnemy = enemy.transform.position - CombatController.transform.position;
            vecToEnemy.y = 0f;

            float angle = Vector3.Angle(direction, vecToEnemy);
            float distance = vecToEnemy.magnitude * Mathf.Sin(angle * Mathf.Deg2Rad);

            if(distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }
}
