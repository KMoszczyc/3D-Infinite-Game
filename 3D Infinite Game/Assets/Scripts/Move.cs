using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float walkSpeed= 5; 
    public float runSpeed= 15; 
    public float jumpForce = 50;
    public float gravity = 9.8f;

    public Animator animator;
    public Transform cam;
    public Transform ground;
    public float turnSmoothTime = 0.1f;

    private float ySpeed = 0; 
    private CharacterController controller;
    private PlayerController playerController;
    
    float turnSmoothVelocity;
    float distToGround = 0;
    float lastGroundedTime = 0;
    [HideInInspector] public bool isGrounded;
    
    void Start() {
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        Cursor.lockState = CursorLockMode.Locked;
        distToGround = controller.bounds.extents.y;
    }

    void Update(){
        if(!playerController.isGameEnded && !playerController.isControlsViewOpen)
            moveCharacter();
    }

    void moveCharacter() {
        float speed = walkSpeed;
        float movement = 0f;
        if(Input.GetKey(KeyCode.LeftShift))
            speed = runSpeed;


        movement += Mathf.Abs(Input.GetAxis("Vertical"));
        movement += Mathf.Abs(Input.GetAxis("Horizontal"));

        speed *= Mathf.Clamp01(movement);


        Vector3 move = new Vector3();


        // rotate
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (controller.isGrounded)
            lastGroundedTime = Time.time;
            
        if(Time.time - lastGroundedTime<0.3f)
            isGrounded = true;
        else
           isGrounded = false;
        
        if (isGrounded){
            lastGroundedTime = 0;
            ySpeed = -gravity * Time.deltaTime; 
            // ySpeed = 0;
            if (Input.GetKeyDown("space")){ 
                ySpeed = jumpForce;
            }
        } 
        
        ySpeed -= gravity * Time.deltaTime;

        if(direction.magnitude>0.1f){

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            move = moveDir.normalized * speed;
        }
        
        animator.SetFloat("Speed", speed);
        move.y = ySpeed;
        controller.Move(move * Time.deltaTime);
    }

    bool IsGrounded(){
        return Physics.Raycast(transform.position, Vector3.down,  distToGround + 0.1f);
    }
}
