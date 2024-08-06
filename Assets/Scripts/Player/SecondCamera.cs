using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondCamera : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float rotationSpeed = 2f;

    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;

    [SerializeField] Vector2 framingOffset;


    float rotationX;
    float rotationY;

    public bool InvertX, InvertY;
    float intervalXVal, intervalYVal;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        intervalXVal = (InvertX ? -1 : 1);
        intervalYVal = (InvertY ? -1 : -1);

        rotationX += Input.GetAxis("Camera Y") * intervalYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Camera X") * intervalXVal * rotationSpeed;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);        

        
        transform.rotation = targetRotation;
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}