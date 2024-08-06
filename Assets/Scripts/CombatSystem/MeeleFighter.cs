using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleFighter : MonoBehaviour
{
    public Animator animator;

    public bool InAction { get; private set; } = false;

    public void Awake()
    {
        
    }
    public void TryToAttack()
    {
        if(!InAction)
        {
            if (animator.GetInteger("WeaponState") != 0)
            {
                StartCoroutine(Attack());
            }

            
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        if(animator.GetInteger("WeaponState")==1)
        {
            int now = Random.Range(1, 3);            
            animator.CrossFade("Punch"+now.ToString(), 0.2f);
        }
        else if(animator.GetInteger("WeaponState")==2)
        {
            int now = Random.Range(1, 3);
            animator.CrossFade("Meele"+now.ToString(), 0.2f);
        }
        else if(animator.GetInteger("WeaponState")==3)
        {
            //shoot anim 
        }
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        yield return new WaitForSeconds(animState.length);
        InAction = false;
    }
}
