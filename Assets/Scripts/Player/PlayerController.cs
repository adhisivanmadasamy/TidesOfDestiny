using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    [Header("Movement settings")]
    public float moveSpeed = 1.5f;
    [SerializeField] float rotationSpeed = 500f;

    [Header("Ground check settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;
    bool isAiming;
    float yspeed;

    Quaternion targetRotaion;

    CameraController cameraController;
    public SecondCamera secondCamera;
    public GameObject firstCamera;

    public Animator animator;
    CharacterController characterController;
    MeeleFighter meeleFighter;

    [SerializeField] Rig aimRig;
    float aimRigWeight;

    bool isSprinting = false;

    public GameObject AxeMesh;
    public GameObject GunMesh;

    public CinemachineVirtualCamera aimVirtualCamera;

    Vector3 moveDir;
    float moveAmount;

    public bool inWater = false, isFloating = false;
    public bool isUnderwater = false;
    private int LastWeaponState = 0;

    public LayerMask waterMask;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        characterController = GetComponent<CharacterController>();
        meeleFighter = GetComponent<MeeleFighter>();
    }

    private void Update()
    {
        Sprint();

        SwapWeapon();

        aimGun();

        if (!inWater)
        {
            GroundMovement();
        }
        else
        {
            WaterMovement();
        }

    }

    public void GroundMovement()
    {

        if (!inWater)
        {
            isFloating = false;
        }

        if (meeleFighter.InAction)
        {
            animator.SetFloat("moveAmount", 0f);
            return;
        }

        //Player Movements
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (!isSprinting)
        {
            moveAmount = Mathf.Clamp((Mathf.Abs(h) + Mathf.Abs(v)), 0, 0.5f);
        }
        else
        {
            moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));
        }

        var moveInput = (new Vector3(h, 0, v)).normalized;

        //Conditions while aiming
        if (isAiming)
        {
            moveDir = secondCamera.transform.forward.normalized;
            moveDir.y = 0;
            moveSpeed = 0;
            moveAmount = 0;
            rotationSpeed = 3000f;

        }
        else
        {
            moveDir = cameraController.PlanarRotation * moveInput;
            rotationSpeed = 500f;
        }

        //Gravity
        GroundCheck();

        if (isGrounded)
        {
            yspeed = -0.5f;
        }
        else
        {
            yspeed += Physics.gravity.y * Time.deltaTime;
        }

        //playerMovement
        var velocity = moveDir * moveSpeed;
        velocity.y = yspeed;

        characterController.Move(velocity * Time.deltaTime);

        //Player orientation
        if (moveAmount > 0 || isAiming)
        {
            targetRotaion = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotaion,
            rotationSpeed * Time.deltaTime);

        animator.SetFloat("moveAmount", moveAmount, 0.1f, Time.deltaTime);
    }

    public void EnterWater()
    {
        inWater = true;
        LastWeaponState = animator.GetInteger("WeaponState");
        animator.SetInteger("WeaponState", 4);

    }

    public void ExitWater()
    {
        inWater = false;
        animator.SetInteger("WeaponState", LastWeaponState);

    }
    void CheckFloat()
    {
        //Check floating
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.8f,
            transform.position.z), Vector3.down, out hit, Mathf.Infinity, waterMask) &&
            inWater == true)
        {
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.8f,
            transform.position.z), Vector3.down, Color.green);
            if (hit.distance < 0.2f)
            {
                //player is floating
                isFloating = true;
                isUnderwater = false;
                Debug.Log("Player Floating");
            }

        }
        else
        {
            isFloating = false;
            isUnderwater = true;
            Debug.Log("Player Under water");
        }
    }
    void WaterMovement()
    {

        //activate water movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float u = Input.GetAxis("UpDown");

        if (!isSprinting)
        {
            moveAmount = Mathf.Clamp(Mathf.Abs(h) + Mathf.Abs(v) + Mathf.Abs(u), 0, 0.5f);
        }
        else
        {
            moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v) + Mathf.Abs(u));
        }

        var moveInput = (new Vector3(h, 0, v)).normalized;

        moveDir = cameraController.PlanarRotation * moveInput;


        var velocity = moveDir * moveSpeed;

                       

        CheckFloat();

        if (inWater && !isFloating && !isUnderwater)
        {
            yspeed += Physics.gravity.y * Time.deltaTime;
            velocity.y = yspeed;
        }


        if (isFloating)
        {
            velocity.y += Mathf.Clamp(u, -1, 0) * Time.deltaTime * moveSpeed * 150f;
        }
        else
        {
            velocity.y += u * Time.deltaTime * moveSpeed * 150f;
        }


        characterController.Move(velocity * Time.deltaTime);


        if (moveAmount > 0 && (moveInput.x != 0 || moveInput.z != 0))
        {
            targetRotaion = Quaternion.LookRotation(moveDir);
        }



        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotaion,
            rotationSpeed * Time.deltaTime);
        if ((u > 0 && isFloating) || (v!=0 && isFloating) || (h!=0 && isFloating))
        {
            if(isSprinting)
            {
                moveAmount = 1;
            }
            else
            {
                moveAmount = 0;
            }
            
        }


        animator.SetFloat("moveAmount", moveAmount, 0.1f, Time.deltaTime);
    }

    void Sprint()
    {
        if(Input.GetButton("Sprint"))
        {
            moveSpeed = 3.5f;
            isSprinting = true;            
        }
        else
        {
            isSprinting= false;
            moveSpeed = 1.5f;            
        }
    }
    void SwapWeapon()
    {
        if ((Input.GetButtonDown("WeaponChange")))
        {
            int currentState = animator.GetInteger("WeaponState");
            if (currentState == 3) //Will change to 3 after adding gun
            {
                currentState = 0;
                AxeMesh.SetActive(false);
                GunMesh.SetActive(false);
            }
            else
            {
                if (currentState == 0)
                {
                    AxeMesh.SetActive(false);
                    GunMesh.SetActive(false);
                }
                else if (currentState == 1)
                {
                    AxeMesh.SetActive(true);
                    GunMesh.SetActive(false);
                }
                else if (currentState == 2)
                {
                    AxeMesh.SetActive(false);
                    GunMesh.SetActive(true);
                }

                currentState += 1;
            }
            animator.SetInteger("WeaponState", currentState);
            Debug.Log("Weapon Changed ! ");
        }
    }

    void aimGun()
    {
        if (Input.GetButton("Aim") && animator.GetInteger("WeaponState") == 3)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            aimRigWeight = 1f;
            isAiming = true;
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            aimRigWeight = 0f;
            isAiming = false;
        }

        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);

    }
    void GroundCheck()
    {
        isGrounded = 
        Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius,
            groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);

    }
}
