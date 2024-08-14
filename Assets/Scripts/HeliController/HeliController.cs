using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeliController : MonoBehaviour
{
    Rigidbody rb;

    float heightVal;
    public float UpSpeed = 10f;
    public float UpMovement;

    float MoveVal;
    public float MoveSpeed = 10f;
    public float FrontMovement;

    float TurnVal;
    public float TurnSpeed = 10f;
    public float TurnMovement;
    
    public CameraController cameraController;

    public float stabilizationSpeed = 2f;

    public bool inHeli;

    public ControllerManager controllerManager;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (inHeli)
        {
            rb.useGravity = false;

            HandleInput();

            rb.AddForce(Vector3.up * UpMovement * UpSpeed, ForceMode.Impulse);
            rb.AddForce(transform.forward * FrontMovement * MoveSpeed, ForceMode.Impulse);

            rb.AddTorque(transform.up * TurnMovement * TurnSpeed, ForceMode.Impulse);

            StabilizeRotation();
        }
        
    }

    private void Update()
    {
        if (inHeli)
        {

            if (Input.GetButtonDown("EnterVehicle"))
            {
                controllerManager.ExitHeli();
                rb.useGravity = true;
                Debug.Log("Pressed F");
            }
        }

        
    }

    public void HandleInput()
    {
        
        heightVal = Input.GetAxis("UpDown");
        if(heightVal != 0f)
        {
            UpMovement = heightVal * Time.deltaTime * 5f;
        }
        else
        {
            UpMovement = 0f;
        }

        

        MoveVal = Input.GetAxis("Vertical");
        if(MoveVal != 0f)
        {
            FrontMovement = MoveVal * Time.deltaTime;
        }
        else
        {
            FrontMovement = 0;
        }



        TurnVal = Input.GetAxis("Horizontal");
        if (TurnVal != 0f)
        {
            TurnMovement = TurnVal * Time.deltaTime;
        }
        else
        {
            TurnMovement = 0;
        }


        

    }

    private void StabilizeRotation()
    {
        // Get the current rotation
        Quaternion currentRotation = transform.rotation;

        // Calculate the target rotation (keep Y axis as is, stabilize X and Z axes)
        Quaternion targetRotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, 0f);

        // Lerp towards the target rotation for stabilization
        transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, stabilizationSpeed * Time.deltaTime);
    }
}
