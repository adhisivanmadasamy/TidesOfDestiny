using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponChange : MonoBehaviour 
{

    [SerializeField] Animator animator;
    [SerializeField] GameObject AxeObj, GunObj;

    //Functions for button to change between weapon modes
    //Used in weapon wheel buttons
    public void TakeFist()
    {
        AxeObj.SetActive(false);
        GunObj.SetActive(false);        
        animator.SetInteger("WeaponState", 1);
    }

    public void TakeMelee()
    {
        AxeObj.SetActive(true);
        GunObj.SetActive(false);        
        animator.SetInteger("WeaponState", 2);
    }

    public void TakeGun()
    {
        AxeObj.SetActive(false);
        GunObj.SetActive(true);
        animator.SetInteger("WeaponState", 3);
    }

    public void CasualMode()
    {
        AxeObj.SetActive(false);
        GunObj.SetActive(false);
        animator.SetInteger("WeaponState", 0);
    }

}
