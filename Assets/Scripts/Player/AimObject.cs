using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimObject : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private LayerMask layerMask;

    //To move the aimobject to the hitpoint of raycast
    //This aim object is used to make the player and gun point exactly at the 
    //shooting point using dynamic rig
    private void Update()
    {        
        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward,
            out RaycastHit hitInfo, 900f, layerMask))
        {
            transform.position = hitInfo.point;
        }
    }
}
