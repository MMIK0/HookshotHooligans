using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager instance;
    private const float NormalFov = 60f;
    private const float HookshotFov = 100f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform debugHitPointTransform;
    [SerializeField] private Transform hookshotTransform;
    private CameraFov cameraFov;
    private CharacterController charController;
    private float cameraVerticalAngle;
    private float characterVelocityY;
    private Vector3 characterVelocityMomentum;
    private Camera playerCamera;
    private State state;
    private Vector3 hookShotPosition;
    private float hookshotSize;
    public bool cooldown;
    public Animator anim;
    public bool canJump = true;
    
    
    private enum State
    {
        Normal,
        HookshotThrown,
        HookShotFlyingPlayer,



    }

    private void Awake()
    {
 
        anim.GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        state = State.Normal;
        hookshotTransform.gameObject.SetActive(false);
        cameraFov = playerCamera.GetComponent<CameraFov>();
      
    }

    // Update is called once per frame
    private void Update()
    {
       
        switch (state)
        {
            default:
            case State.Normal:
                HandleCharacterLook();
                HandleCharacterMovement();
                HandleHookShotStart();
                break;
            case State.HookshotThrown:
                HandleHookshotThrow();
                HandleCharacterLook();
                HandleCharacterMovement();
                break;
            case State.HookShotFlyingPlayer:
                HandleHookShotMovement();
                HandleCharacterLook();
                break;
        }
    }
    
    private void HandleCharacterLook()
    {

        float lookX = Input.GetAxisRaw("Mouse X");
        float lookY = Input.GetAxisRaw("Mouse Y");

        transform.Rotate(new Vector3(0f, lookX * mouseSensitivity, 0f), Space.Self);
        cameraVerticalAngle -= lookY * mouseSensitivity;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);
        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);

    }

    public void HandleCharacterMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");


        float moveSpeed = 20f;

        Vector3 characterVelocity = transform.right * moveX * moveSpeed + transform.forward * moveZ * moveSpeed;


        if (charController.isGrounded)
        {
            characterVelocityY = 0f;

            if (canJump && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                if (TestInputJump())
                {
                    float jumpSpeed = 30f;
                    characterVelocityY = jumpSpeed;
                }
            }
            
        }

        float gravityDownForce = -60f;
        characterVelocityY += gravityDownForce * Time.deltaTime;

        characterVelocity.y = characterVelocityY;

        //apply momentum
        characterVelocity += characterVelocityMomentum;

        charController.Move(characterVelocity * Time.deltaTime);

        //dampen the momentum
        if (characterVelocityMomentum.magnitude >= 0f)
        {
            float momentumDrag = 3f;
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }
        }

    }

    private void ResetGravity()
    {
        characterVelocityY = 0f;
    }

    private void HandleHookShotStart()
    {
      if (TestInputDownHookshot() && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {

                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit rayCastHit) && cooldown == false)
                {
                    //hit something
                    debugHitPointTransform.position = rayCastHit.point;
                    hookShotPosition = rayCastHit.point;
                    hookshotSize = 0f;
                    hookshotTransform.gameObject.SetActive(true);
                    hookshotTransform.localScale = Vector3.zero;
                    state = State.HookshotThrown;
                    Invoke("ResetCooldown", 1f);
                    cooldown = true;

                }
            }
    }
     
    private void HandleHookshotThrow()
    {
            hookshotTransform.LookAt(hookShotPosition);
            float hookshotMin = 1f;
            float hookshotMax = 120f;
            float hookshotThrowSpeed = 500f;
            hookshotSize += hookshotThrowSpeed * Time.deltaTime;
            hookshotTransform.localScale = new Vector3(1, 1, hookshotSize);
            hookshotSize = Mathf.Clamp(hookshotSize, hookshotMin, hookshotMax);

            if (hookshotSize == hookshotMax)
            {
                StopHookshot();
            }

            if (hookshotSize >= Vector3.Distance(transform.position, hookShotPosition))
            {
            state = State.HookShotFlyingPlayer;
            cameraFov.SetCameraFov(HookshotFov);
            }
    }

    private void HandleHookShotMovement()
    {
        hookshotTransform.LookAt(hookShotPosition);
        Vector3 hookshotDir = (hookShotPosition - transform.position).normalized;


        float hookshotSpeedMin = 20f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookShotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;

        //Move char controller
        charController.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

        float reachedHookshotPositionDistance = 2f;
        if (Vector3.Distance(transform.position, hookShotPosition) < reachedHookshotPositionDistance)
        {
            //cancel after reaching position
            
            StopHookshot();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            // Cancel Hookshot
            StopHookshot();
        }


        if (TestInputJump())
        {
            // Cancel with jump
            float momentumExtraSpeed = 4f;
            characterVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;
            float jumpSpeed = 30f;
            characterVelocityMomentum += Vector3.up * jumpSpeed;
            
            StopHookshot();
        }
    }
    private void StopHookshot()
    {
        state = State.Normal;
        ResetGravity();
        hookshotTransform.gameObject.SetActive(false);
        
        cameraFov.SetCameraFov(NormalFov);
    }
    private bool TestInputDownHookshot()
    {

        return Input.GetButtonDown("Fire1");

    }
    private  bool TestInputJump()
    {
        return Input.GetButtonDown("Jump");
    }
    private void ResetCooldown()
    {
        cooldown = false;
    }
    
    
}
