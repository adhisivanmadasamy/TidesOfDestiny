using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoat : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().inRangeBoat = true;
            Debug.Log("In Boat range! ");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().inRangeBoat = false;
            Debug.Log("not in Boat range! ");
        }

    }
}
