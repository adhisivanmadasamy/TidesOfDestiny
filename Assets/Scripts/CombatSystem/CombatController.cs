using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    EnemyController targetEnemy;
    public EnemyController TargetEnemy
    {
        get => targetEnemy;
        set
        {
            targetEnemy = value;

            if(targetEnemy == null) 
                combatMode = false;
        }
    }

    public MeeleFighter meeleFighter;
    public GameObject Parent;
    Animator animator;
    CameraController cam;

    bool combatMode = false;
    public bool CombatMode
    {
        get => combatMode; 
        set
        {
            combatMode = value;

            if(TargetEnemy == null)
            {
                combatMode = false;
            }

            animator.SetBool("CombatMode", combatMode);
        }
    }

    private void Awake()
    {
        
        animator = GetComponent<Animator>();
        cam = Camera.main.GetComponent<CameraController>(); 
    }

    private void Start()
    {
        meeleFighter.OnGotHit += (MeeleFighter attacker) =>
        {
            if(CombatMode && attacker != TargetEnemy.Fighter)
                TargetEnemy = attacker.GetComponent<EnemyController>();
        };
    }
 
    void Update()
    {
        if(Input.GetButtonDown("Attack") && !meeleFighter.isTakingHit)
        {
            var enemy = EnemyManager.i.GetAttackingEnemy();
            if(enemy != null && enemy.Fighter.IsCounterable && !meeleFighter.InAction)
            {
                StartCoroutine(meeleFighter.PerformCounter(enemy));
            }
            else
            {
                var enemyToAttack = EnemyManager.i.GetClosestEnemyToDirection
                    (PlayerController.i.GetIntentDir());
                         
                meeleFighter.TryToAttack(enemyToAttack?.Fighter);

                //combatMode = true;
            }            
        }

        //Disabled LockOn mode - will use in future - Maybe
         
        //if(Input.GetButtonDown("LockOn") || JoystickHelper.i.GettAxisDown("LockOnTrigger"))
        //{
        //    combatMode = !combatMode;
        //}
    }

    private void OnAnimatorMove()
    {
        Vector3 deltaPosition = animator.deltaPosition;
        Quaternion deltaRotation = animator.deltaRotation;

        // Debugging output to check root motion values
        Debug.Log("Delta Position: " + deltaPosition);
        Debug.Log("Delta Rotation: " + deltaRotation);

        if (!meeleFighter.InCounter)
            Parent.transform.position += animator.deltaPosition;

        if (meeleFighter.InAction)
            Parent.transform.position += animator.deltaPosition;

        
        Parent.transform.rotation *= animator.deltaRotation;
    }

    public Vector3 GetTargetingDir()
    {
        if(!combatMode)
        {
            var vecFromCam = transform.position - cam.transform.position;
            vecFromCam.y = 0;
            return vecFromCam.normalized;
        }
        else
        {
            return transform.forward;
        }       

    }
}
