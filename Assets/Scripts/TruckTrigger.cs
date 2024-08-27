using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().inRangeTruck = true;
            Debug.Log("In Truck range! ");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().inRangeTruck = false;
            Debug.Log("not in range! ");
        }

    }
}
