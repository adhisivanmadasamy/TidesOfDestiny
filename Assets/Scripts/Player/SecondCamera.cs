using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondCamera : MonoBehaviour //aim camera, similar to main Cam
{
    [Header("Camera Controls")]
    [SerializeField] Transform followTarget;
    [SerializeField] float rotationSpeed = 2f;

    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;

    [SerializeField] Vector2 framingOffset;
    public bool InvertX, InvertY;

    //Other private variables
    float rotationX;
    float rotationY;    
    float intervalXVal, intervalYVal;

    private void Start()
    {
        //Hiding the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        //AimCam movements
        intervalXVal = (InvertX ? -1 : 1);
        intervalYVal = (InvertY ? -1 : -1);

        rotationX += Input.GetAxis("Camera Y") * intervalYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Camera X") * intervalXVal * rotationSpeed;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);        

        
        transform.rotation = targetRotation;
    }

    //To give jus the Y rotation to the player to align in the direction of AimCam
    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}