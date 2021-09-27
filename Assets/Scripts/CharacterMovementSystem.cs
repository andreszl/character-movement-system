using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementSystem : MonoBehaviour {
    Rigidbody myRigidbody;
    public Vector3 velocity;

    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float speedSmoothTime = 0.1f;
    public float timeToJumpApex = 0.5f;
    public float jumpForce;
    public float jumpHeight = 3.0f;

    float  speedSmoothVelocity;
    float  currentSpeed;
    float  gravity;
    float  ySpeed;
    float  distToGround;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    bool jumping = false;
    Transform cameraT;
    Transform myTransform;

    LayerMask ignoreMask;

    void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        myTransform = GetComponent<Transform>();

        distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
        ignoreMask = 8;
        cameraT = Camera.main.transform;

        CalculateGravity();
    }

    void Update() {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 inputDir = input.normalized;

        bool running = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;

        if(inputDir != Vector3.zero) {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            myTransform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(myTransform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        if(IsGrounded()) {
            ySpeed = 0;
            
            if(Input.GetAxis("Jump") != 0) {
                ySpeed = jumpForce;
            }
        }

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        ySpeed += gravity * Time.deltatime;
        velocity = input * currentSpeed;
        velocity.y = ySpeed;

    }

    void FixedUpdate() {
        myRigidbody.velocity = myTransform.forward * currentSpeed + Vector3.up * velocity.y;
    }

    void CalculateGravity() {
        gravity = -(2 * jumpHeight/Mathf.Pow(timeToJumpApex, 2));
        jumpForce = Mathf.Abs(gravity) * timeToJumpApex;
    }

    bool IsGrounded() {
        return Physics.Raycast(myTransform.position, -Vector3.up, distToGround + 0.1f);
    }
}
