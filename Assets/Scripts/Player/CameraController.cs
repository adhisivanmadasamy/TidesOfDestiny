using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Controls")]
    [SerializeField] Transform followTarget;
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float distance = 5;

    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;

    [SerializeField] Vector2 framingOffset;
    public bool InvertX, InvertY;

    //private variables
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
        //Cam movement
        intervalXVal = (InvertX ? -1 : 1);
        intervalYVal = (InvertY ? -1 : -1);

        rotationX += Input.GetAxis("Camera Y") * intervalYVal* rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Camera X") * intervalXVal * rotationSpeed;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

        transform.position = focusPosition - targetRotation * new Vector3(0,0,distance);
        transform.rotation = targetRotation;
    }

    //To give jus the Y rotation to the player to align in the direction of Cam
    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}
