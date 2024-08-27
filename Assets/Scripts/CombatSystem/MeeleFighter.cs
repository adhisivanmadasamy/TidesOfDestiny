using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AttackState {  Idle, BeginAttack, InAttack, EndAttack }
public class MeeleFighter : MonoBehaviour
{
    [field: SerializeField] public float Health { get; private set; } = 25f;

    [SerializeField] List<AttackData> MeleeAttacks;

    [SerializeField] List<AttackData> BoxAttacks;

    [SerializeField] List<AttackData> LongRangeAttacks;

    [SerializeField] float longRangeDistance = 1.5f;

    public bool isTakingHit = false;
    public Animator animator;

    [SerializeField] float rotationSpeed = 500f;

    public GameObject Axe;
    BoxCollider AxeCollider;
    SphereCollider LeftHandCollider, RightHandCollider;
    public bool InAction { get; private set; } = false;

    public AttackState attackState;

    public event Action<MeeleFighter> OnGotHit;

    public event Action OnHiComplete;
    public bool InCounter { get;  set; }

    bool doCombo;
    int ComboCount;

    public void Awake()
    {
        
    }
    private void Start()
    {
        if(this.gameObject.tag == "Player" || this.gameObject.tag == "EnemyBox")
        {
            AxeCollider = Axe.GetComponent<BoxCollider>();
            LeftHandCollider = animator.GetBoneTransform(HumanBodyBones.LeftHand).
                GetComponent<SphereCollider>();
            RightHandCollider = animator.GetBoneTransform(HumanBodyBones.RightHand).
                GetComponent<SphereCollider>();
            DisableHitBoxes();
        } 

        if(this.gameObject.tag == "EnemyMelee")
        {
            AxeCollider = Axe.GetComponent<BoxCollider>();
        }
    }
    public void TryToAttack(MeeleFighter target = null)
    {
        if(gameObject.tag == "Player")
        {
            if(MenuScript.menu.inWheel)
            {
                return;
            }
        }

        if (!InAction)
        {
            if (animator.GetInteger("WeaponState") != 0)
            {
                StartCoroutine(Attack(target));
            }
            else
            {
                animator.SetInteger("WeaponState", 1);
                StartCoroutine(Attack(target));
            }
        }
        else if(attackState == AttackState.InAttack || 
            attackState == AttackState.EndAttack)
        {
            doCombo = true;
        }
    }

    MeeleFighter currTarget;

    IEnumerator Attack(MeeleFighter target = null)
    {
        InAction = true;
        currTarget = target;
        attackState = AttackState.BeginAttack;
        var attack = MeleeAttacks[ComboCount];

        if (animator.GetInteger("WeaponState")==0)
        {
            attack = BoxAttacks[ComboCount];
        }
        else if(animator.GetInteger("WeaponState") == 1)
        {
            attack = BoxAttacks[ComboCount];
        }
        else if(animator.GetInteger("WeaponState") == 2)
        {
            attack = MeleeAttacks[ComboCount];
        }
        else
        {
            //nothing
        }              

        var attackDir = transform.forward;

        Vector3 startPos = transform.position;
        Vector3 targetPos = Vector3.zero;

        if(target != null)
        {
            var vecToTarget= target.transform.position - transform.position;
            vecToTarget.y = 0;

            attackDir = vecToTarget.normalized;
            float distance = vecToTarget.magnitude - attack.DistanceFromTarget;

            if(distance >= longRangeDistance && LongRangeAttacks.Count > 0 )
            {
                attack = LongRangeAttacks[0];
            }

            if(attack.MoveToTarget)
            {
                if (distance <= attack.MaxMoveDistance)
                {
                    targetPos = target.transform.position - attackDir * attack.DistanceFromTarget;
                }
                else
                {
                    targetPos = startPos + attackDir * attack.MaxMoveDistance;
                }
            }      
        }
                
        int CurrentWeaponState;

        if(animator.GetInteger("WeaponState")==1)
        {
            CurrentWeaponState = 1;
            animator.CrossFade(attack.AnimName, 0.2f);
        }
        else if(animator.GetInteger("WeaponState")==2)
        {
            CurrentWeaponState = 2;
            animator.CrossFade(attack.AnimName, 0.2f);
        }
        else if(animator.GetInteger("WeaponState")==3)
        {
            CurrentWeaponState = 3;
            //shoot anim 
        }
        else
        {
            //Just to avoid error of unassigned local variable
            CurrentWeaponState = 1;
        }
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float StartTime;
        float EndTime;
        int AttackCount;

        if(CurrentWeaponState==3)
        {
            StartTime = attack.AttackStartTime;
            EndTime = attack.AttackEndTime;
            AttackCount = BoxAttacks.Count;
        }
        else if (CurrentWeaponState == 2) 
        {
            StartTime = attack.AttackStartTime;
            EndTime = attack.AttackEndTime;
            AttackCount = MeleeAttacks.Count;
        }
        else
        {
            StartTime = attack.AttackStartTime;
            EndTime = attack.AttackEndTime;
            AttackCount = BoxAttacks.Count;
        }

        float timer = 0f;
        while (timer <= animState.length) 
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;


            if(target!= null && attack.MoveToTarget)
            {
                float percTime = (normalizedTime - attack.AttackStartTime) /
                    (attack.AttackEndTime - attack.AttackStartTime); 
                transform.position =  Vector3.Lerp(startPos, targetPos, percTime);
            }

            if(attackDir != null)
            {
               transform.rotation =  Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(attackDir), rotationSpeed
                    * Time.deltaTime);
            }
            else
            {
                Debug.Log("Attack Dir is null");
            }

            if (attackState == AttackState.BeginAttack)
            {
                if (InCounter) break;
                
                if (normalizedTime >= StartTime)
                {
                    attackState = AttackState.InAttack;
                    EnableHitBox(CurrentStateAttack(CurrentWeaponState));
                }
            }
            else if (attackState == AttackState.InAttack)
            {
                if (normalizedTime >= EndTime)
                {
                    attackState = AttackState.EndAttack;
                    DisableHitBoxes();
                }
            }
            else if (attackState == AttackState.EndAttack)
            {
                if(doCombo)
                {
                    doCombo = false;
                    ComboCount = (ComboCount + 1) % AttackCount;

                    StartCoroutine(Attack(target));
                    yield break;
                    
                }
            }
            yield return null;
        }

        attackState = AttackState.Idle;
        ComboCount = 0;
        InAction = false;
        currTarget = null;
    }

    
    public void OnTriggerEnter(Collider other)
    {
        
        if ((other.gameObject.tag == "PlayerHand" || other.gameObject.tag=="PlayerMelee"
            || other.gameObject.tag == "EnemyHand" || other.gameObject.tag == "EnemyMelee")
            && !isTakingHit && !InCounter)
        {
            var attacker = other.GetComponentInParent<MeeleFighter>();
            if(attacker.currTarget != this)
                return;

            if (animator.GetInteger("WeaponState") != 0)
            {
                TakeDamage(WeaponDamage(other.gameObject.tag));
                OnGotHit?.Invoke(attacker);
                if (Health > 0)
                    StartCoroutine(GetHit(attacker));
                else
                    PlayDeathAnim(attacker);
            }
            else
            {
                animator.SetInteger("WeaponState", 1);
                TakeDamage(WeaponDamage(other.gameObject.tag));
                OnGotHit?.Invoke(attacker);
                if (Health > 0)
                    StartCoroutine(GetHit(attacker));
                else
                    PlayDeathAnim(attacker);
            }
        }
    }

    public float WeaponDamage(string ObjTag)
    {
        float Damage;
        if (ObjTag == "PlayerHand")
        {
            Damage = 5f;
        }
        else if(ObjTag == "PlayerMelee")
        {
            Damage = 10f;
        }
        else if(ObjTag == "EnemyHand")
        {
            Damage = 5f;
        }
        else if(ObjTag == "EnemyMelee")
        {
            Damage = 10f;
        }
        else
        {
            Damage = 5f;
        }
        
        return Damage;
    }

    void TakeDamage(float damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, Health);
    }

    void PlayDeathAnim(MeeleFighter attacker)
    {
        animator.CrossFade("Death", 0.2f);
        
    }
    IEnumerator GetHit(MeeleFighter attacker)
    {
        InAction = true;
        isTakingHit = true;
        var dispVec = attacker.transform.position - transform.position;
        dispVec.y = 0;
        transform.rotation = Quaternion.LookRotation(dispVec);

        if (animator.GetInteger("WeaponState") == 1)
        {            
            animator.CrossFade("BoxHit", 0.2f);
        }
        else if (animator.GetInteger("WeaponState") == 2)
        {            
            animator.CrossFade("MeeleHit", 0.2f);
        }
        else if (animator.GetInteger("WeaponState") == 3)
        {
            //gun get hit anim
        }
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        yield return new WaitForSeconds(animState.length * 0.7f);
        
        OnHiComplete?.Invoke();
        isTakingHit = false;
        InAction = false;
    }

    public IEnumerator PerformCounter(EnemyController opponent)
    {
        InAction = true;

        InCounter = true;
        opponent.Fighter.InCounter = true;
        opponent.ChangeState(EnemyStates.Dead);

        var dispVec = opponent.transform.position - transform.position;

        dispVec.y = 0;

        transform.rotation = Quaternion.LookRotation(dispVec);
        opponent.transform.rotation = Quaternion.LookRotation(-dispVec);

        var targetPos = Vector3.zero;

        //targetPos = opponent.transform.position - dispVec.normalized * 0.6f;
        //targetPos.x = targetPos.x + 0.2f;
                
        if (animator.GetInteger("WeaponState") == 1)
        {
            targetPos = opponent.transform.position - dispVec.normalized * 0.6f;
            targetPos.x = targetPos.x + 0.2f;
            animator.CrossFade("BoxCounter", 0.2f);
            opponent.animator.CrossFade("BoxCounterVictim", 0.2f);
        }
        else if (animator.GetInteger("WeaponState") == 2)
        {
            targetPos = opponent.transform.position - dispVec.normalized * 1f;
            
            animator.CrossFade("CounterAttack", 0.2f);
            opponent.animator.CrossFade("CounterVictim", 0.2f);
        }
        else if (animator.GetInteger("WeaponState") == 3)
        {
            //gun get hit anim
        }
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= animState.length)
        {
            if (isTakingHit) break;

            transform.position =  Vector3.MoveTowards(transform.position, 
                targetPos, 5 * Time.deltaTime);
            yield return null;

            timer += Time.deltaTime;
        }

        InCounter = false;
        opponent.Fighter.InCounter = false;

        InAction = false;
    }

    AttackData CurrentStateAttack(int currentState)
    {
        if (currentState == 1)
        {
            return BoxAttacks[ComboCount];
        }
        else if (currentState == 2)
        {
            return MeleeAttacks[ComboCount];
        }
        else
        {
            return BoxAttacks[ComboCount];
        }
    }

    void EnableHitBox(AttackData attack)
    {
        switch (attack.HitBoxToUse)
        {
            case AttackHitBox.LeftHand:
                LeftHandCollider.enabled = true;
                break;
            case AttackHitBox.RightHand:                
                RightHandCollider.enabled = true;
                break;
            case AttackHitBox.Sword:
                AxeCollider.enabled = true;
                break;
            default:
                break;
        }
    }
    void DisableHitBoxes()
    {
        if (AxeCollider != null)
            AxeCollider.enabled = false;
        if (LeftHandCollider != null)
            LeftHandCollider.enabled = false;
        if (RightHandCollider != null)
            RightHandCollider.enabled = false;
    }

    public bool IsCounterable => attackState == AttackState.BeginAttack 
        && ComboCount == 0;
}
