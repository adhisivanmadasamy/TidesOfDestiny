using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().inRangeHeli = true;
            Debug.Log("In Heli range! ");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().inRangeHeli = false;
            Debug.Log("not in Heli range! ");
        }

    }
}
