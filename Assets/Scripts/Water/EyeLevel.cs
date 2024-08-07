using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeLevel : MonoBehaviour
{
    public PlayerController playerController;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Water")
        {
            playerController.isFloating = false;
            
        }
    }
}
