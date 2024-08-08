using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBody : MonoBehaviour
{
    public Collider waterCollider;

    private void Start()
    {
        waterCollider = GetComponent<Collider>();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().WaterTag = this.gameObject.tag;
            other.gameObject.GetComponent<PlayerController>().EnterWater();
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().WaterTag = this.gameObject.tag;
            other.gameObject.GetComponent<PlayerController>().ExitWater();
        }
    }
}
