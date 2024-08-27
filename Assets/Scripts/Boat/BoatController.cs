using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 10f;                
    public float turnSpeed = 5f;             
    public float acceleration = 2f;         
    public float deceleration = 2f;          
    public float maxSpeed = 20f;            

    private float currentSpeed = 0f;

    public bool inBoat;

    public ControllerManager controllerManager;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (inBoat)
        {            

            if (Input.GetButtonDown("EnterVehicle"))
            {
                controllerManager.ExitBoat();                
            }
        }
    }
    private void FixedUpdate()
    {
        if (inBoat) 
        {
            float turnInput = Input.GetAxis("Horizontal");
            float moveInput = Input.GetAxis("Vertical");

            if (moveInput > 0)
            {
                currentSpeed += acceleration * Time.deltaTime;
            }
            else if (moveInput < 0)
            {
                currentSpeed -= acceleration * Time.deltaTime;
            }
            else
            {
                if (currentSpeed > 0)
                    currentSpeed -= deceleration * Time.deltaTime;
                else if (currentSpeed < 0)
                    currentSpeed += deceleration * Time.deltaTime;
            }


            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);


            Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);

            float turn = turnInput * turnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}

