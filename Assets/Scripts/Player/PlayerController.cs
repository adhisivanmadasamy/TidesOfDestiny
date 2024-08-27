using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    public static PlayerController i;

    [Header("Movement settings")]
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] float WaterMoveSpeed = 2.5f;
    [SerializeField] float rotationSpeed = 500f;

    [Header("Ground check settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;

    [Header("Controller settings")]
    public bool isAiming;
    float yspeed;

    Quaternion targetRotaion;

    CameraController cameraController;
    [SerializeField] SecondCamera secondCamera;
    [SerializeField] GameObject firstCamera;

    [SerializeField] Animator animator;
    CharacterController characterController;
    MeeleFighter meeleFighter;

    [SerializeField] Rig aimRig;
    float aimRigWeight;

    bool isSprinting = false;

    [SerializeField] GameObject AxeMesh;
    [SerializeField] GameObject GunMesh;

    [SerializeField] CinemachineVirtualCamera aimVirtualCamera;
    
    public Vector3 moveDir;
    float moveAmount;

    [Header("Water Settings")]
    //Declared public to access from other classes
    public bool inWater = false, isFloating = false;
    public bool isUnderwater = false;

    public bool PrevInWater, PrevIsFloating, PrevIsUnderwater;
    public int LastWeaponState = 0;
    public LayerMask waterMask;
    public string WaterTag;

    public GameObject PoolFloater, OceanFloater;
    public GameObject Floaters;        

    [Header("Vehicle settings")]
    //Declared public to access from other classes
    public ControllerManager controllerManager;
    public bool inVehicle;
    public GameObject PSpawnCar;
    public GameObject PSpawnBoat;
    public GameObject PSpawnHeli;
    public GameObject PSpawnTruck;

    public bool inRangeCar = false, inRangeBoat = false, inRangeHeli = false,
        inRangeTruck = false;

    public GameObject PlayerMesh;
    SkinnedMeshRenderer[] skinnedMeshRenderers;

    public bool inBoat, inCar, inHeli, inTruck;
    
    [Header("Other Settings")]
    public CombatController combatController;
    Rigidbody rb;
    public Vector3 InputDir {  get; set; }

    private void Awake()
    {
        //Initializing 
        cameraController = Camera.main.GetComponent<CameraController>();
        characterController = GetComponent<CharacterController>();
        meeleFighter = GetComponent<MeeleFighter>();
        rb = GetComponent<Rigidbody>();
        i = this;
        skinnedMeshRenderers = PlayerMesh.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        //Variable are used to trigger floaters activation 
        //only when the values are changed
        PrevInWater = inWater;
        PrevIsFloating = isFloating;
        PrevIsUnderwater = isUnderwater;
    }


    private void Update()
    {
        //rigid body is needed for triggering attacks but it's causing
        // problems in under water movements. So, activating it accordingly
        if (isUnderwater)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }

        //Activating Ground, Water, and Vehicle movements as needed
        if (!inVehicle)
        {            
            Sprint();            

            aimGun();

            if (!inWater)
            {
                Floaters.SetActive(false);
                GroundMovement();  
                
                //To enter vehicles while on ground
                if (inRangeCar)
                {
                    if (Input.GetButtonDown("EnterVehicle"))
                    {
                        controllerManager.EnterCar();
                    }
                }
                else if (inRangeBoat)
                {
                    if (Input.GetButtonDown("EnterVehicle"))
                    {
                        controllerManager.EnterBoat();
                    }
                }
                else if (inRangeHeli)
                {
                    if (Input.GetButtonDown("EnterVehicle"))
                    {
                        controllerManager.EnterHeli();
                    }
                }
                else if (inRangeTruck)
                {
                    if (Input.GetButtonDown("EnterVehicle"))
                    {
                        controllerManager.EnterTruck();
                    }
                }
            }
            else
            {                
                WaterMovement();
            }
        }
        else
        {
            //The character is hidden while in vehicle
            //Moving the player along with the vehicle hidden
            if(inCar)
            {
                transform.position = PSpawnCar.transform.position;
            }
            else if(inBoat)
            {
                transform.position = PSpawnBoat.transform.position;
                
            }
            else if(inHeli)
            {
                transform.position = PSpawnHeli.transform.position;
            }
            else if (inTruck)
            {
                transform.position = PSpawnTruck.transform.position;
            }
        }
    }

    public void HideChar()
    {
        //Cannot disable player while in vehicle
        //so, just hiding the skin mesh renderers and moving along
        //with the vehicle
        characterController.enabled = false;
        foreach (SkinnedMeshRenderer smr in skinnedMeshRenderers)
        {
            smr.enabled = false;
        }
    }

    public void UnhideChar()
    {
        //Unhiding the player while getting out of vehicle
        characterController.enabled = true;
        foreach (SkinnedMeshRenderer smr in skinnedMeshRenderers)
        {
            smr.enabled = true;
        }        
    }

    //Character movement and animation controlling while
    //on ground
    public void GroundMovement()
    {        
        if (!inWater)
        {
            isFloating = false;
        }

        if (meeleFighter.InAction || meeleFighter.Health <= 0)
        {
            targetRotaion = transform.rotation;
            animator.SetFloat("forwardSpeed", 0f);
            return;
        }

        //Player Movements input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (!isSprinting)
        {
            //Capping the speed to half when not sprinting
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

        InputDir = cameraController.PlanarRotation * moveInput;
        
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

        if(combatController.CombatMode)
        {
            velocity /= 2f;

            if(combatController.TargetEnemy != null)
            {
                var targetVec = combatController.TargetEnemy.
                transform.position - transform.position;
                targetVec.y = 0;

                if (moveAmount > 0)
                {
                    targetRotaion = Quaternion.LookRotation(targetVec);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotaion,
                    rotationSpeed * Time.deltaTime);
                }
            }

            //Will use if later if using Combat animator state like enemy

            //float forwardSpeed = Vector3.Dot(velocity, transform.forward);
            //animator.SetFloat("forwardSpeed",
            //    forwardSpeed / moveSpeed, 0.2f, Time.deltaTime);
            //float angle = Vector3.SignedAngle(transform.forward, velocity,
            //    Vector3.up);
            //float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
            //animator.SetFloat("strafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);

            //So using this instead
            animator.SetFloat("forwardSpeed", moveAmount, 0.1f, Time.deltaTime);
        }
        else
        {
            //Player orientation
            if (moveAmount > 0 || isAiming)
            {
                targetRotaion = Quaternion.LookRotation(moveDir);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotaion,
                rotationSpeed * Time.deltaTime);

            animator.SetFloat("forwardSpeed", moveAmount, 0.1f, Time.deltaTime);
        }

        velocity.y = yspeed;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void EnterWater()
    {        
        inWater = true;
        //Not turning on the floaters because it'll be
        //turned On&Off in CheckFloat()

        //setting animator to water movement        
        animator.SetInteger("WeaponState", 4);
    }

    public void ExitWater()
    {      
        inWater = false;           
        //Turning off again just to make sure it's off
        //while coming out of water irresptive of coming from
        //both Floating and Underwater state
        FloatersOnOff(false);
        //Back to Casual state
        animator.SetInteger("WeaponState", 0);        
    }

    //To toggle flaoters based on the type of water
    public void FloatersOnOff(bool OnOff)
    {
        if (WaterTag == "Pool")
        {
            PoolFloater.SetActive(OnOff);
        }
        else if (WaterTag == "Ocean")
        {
            OceanFloater.SetActive(OnOff);
        }
    }
        
    //Checking player is floating or under water
    void CheckFloat()
    {
        WaterRayCast(WaterTag);
    }

    //Raycasting based on pool or ocean - Combined functions of pool and ocean together
    public void WaterRayCast(string waterTag)
    {
        float OriginHeight;
        float MaxHitDistance;

        if(WaterTag == "Pool")
        {
            OriginHeight = 1.6f;
            MaxHitDistance = 0.2f;
        }
        else if (WaterTag == "Ocean")
        {
            OriginHeight = 2.2f;
            MaxHitDistance = 0.3f;
        }
        else
        {
            OriginHeight = 2.2f;
            MaxHitDistance = 0.3f;
        }

        //Raycasting from a certain height, if the ray hit water within
        //a particular distance, the player is floating
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + OriginHeight,
            transform.position.z), Vector3.down, out hit, Mathf.Infinity, waterMask) &&
            inWater == true)
        {
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 3f,
            transform.position.z), Vector3.down, Color.green);
            if (hit.distance < MaxHitDistance)
            {
                //player is floating
                isFloating = true;
                isUnderwater = false;
                Debug.Log("Player Floating");
                FloatersOnOff(true);
            }
        }
        else
        {
            //player is underwater
            isFloating = false;
            isUnderwater = true;
            Debug.Log("Player Under water");
            FloatersOnOff(false);
        }
    }

    //Character movement and animation controlling while
    //in water
    void WaterMovement()
    {
        //To enable player to get in boat while in water
        if (inRangeBoat)
        {
            if (Input.GetButtonDown("EnterVehicle"))
            {
                controllerManager.EnterBoat();
            }
        }

        CheckFloat();
                      
        //movement inputs
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float u = Input.GetAxis("UpDown");

        //sprint speed
        if (!isSprinting)
        {
            moveAmount = Mathf.Clamp(Mathf.Abs(h) + Mathf.Abs(v) + Mathf.Abs(u), 0, 0.5f);
        }
        else
        {
            moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v) + Mathf.Abs(u));
        }

        //Movement while in water
        var moveInput = (new Vector3(h, 0, v)).normalized;
        //Getting 2D direction to move from camera
        moveDir = cameraController.PlanarRotation * moveInput;

        var velocity = moveDir * WaterMoveSpeed;

        if (inWater && !isFloating && !isUnderwater)
        {
            //isWater- will be in On when the feet of player touches the ground
            //so, a manual gravity is applied till the player goes down & floats
            yspeed += Physics.gravity.y * Time.deltaTime;
            velocity.y = yspeed;
            characterController.Move(velocity * Time.deltaTime);
        }

        if (isFloating && inWater && !isUnderwater)
        {
            //movement while floating
            Floaters.SetActive(true);
            velocity.y += Mathf.Clamp(u, -1, 0) * Time.deltaTime * WaterMoveSpeed * 150f;
            if (velocity != Vector3.zero)
            {
                characterController.Move(velocity * Time.deltaTime);
            }
        }

        if (inWater && !isFloating && isUnderwater)
        {
            //movement while underwater
            velocity.y += u * Time.deltaTime * WaterMoveSpeed * 150f;
            characterController.Move(velocity * Time.deltaTime);
        }

        //Orienting player in the cam direction 
        if (moveAmount > 0 && (moveInput.x != 0 || moveInput.z != 0))
        {
            //updating target rotation only when moving
            targetRotaion = Quaternion.LookRotation(moveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotaion,
            rotationSpeed * Time.deltaTime);

        //Stopping the player to move up while floating, can only go down
        if ((u > 0 && isFloating))
        {
            moveAmount = 0;
        }

        //the rigidbody is causing glitch movement while moving, still needed in combat
        //collisions, so turning off just while moving
        if(isFloating)
        {
            if (moveAmount != 0)
            {
                rb.isKinematic = true;
            }
            else
            {
                rb.isKinematic = false;
            }
        }
        
        //setiing animator property for water movement animation
        animator.SetFloat("moveAmount", moveAmount, 0.1f, Time.deltaTime);
    }

    //Function to toggle floaters - used in multiple occasions
    private void FloatersActivity()
    {
        if (inWater && isFloating && !isUnderwater)
        {
            FloatersOnOff(true);
        }

        if (inWater && !isFloating && isUnderwater)
        {
            FloatersOnOff(false);
        }
    }

    //Sprint movement
    void Sprint()
    {
        if(Input.GetButton("Sprint"))
        {
            moveSpeed = 5f;
            isSprinting = true;            
        }
        else
        {
            isSprinting= false;
            moveSpeed = 2f;            
        }
    }    

    //Aiming with second camera
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

    //Check weather the player is on ground
    //Just to apply manual gravity if not
    void GroundCheck()
    {
        isGrounded = 
        Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius,
            groundLayer);
    }

    //This debug sphere is used for ground check visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);

    }
    public Vector3 GetIntentDir()
    {
        return InputDir != Vector3.zero ? InputDir : transform.forward;
    }
}
