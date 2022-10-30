using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;
    private float jumpForce = 4.0f;
    private float verticalVelocity;
    private float gravity = 12.0f;
    private float originalSpeed = 7.0f; 

    private int desiredLane = 1; // 0 = left, 1 = middle, 2 = right
    private CharacterController characterController;
    private Animator anim;
    private bool isRunning = false;

    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;

    private void Start()
        {
        speed = originalSpeed;
        characterController =GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        }

    private void Update()
        {
        if (!isRunning) return;

        SetSpeed();

        MoveLaneDirection();
        SetTargetPosition();
        
        }

    void MoveLane(bool goingRight)
        {
        //To Move Left
     /*   if (!goingRight)
            {
            desiredLane--;
            if(desiredLane == -1)
                {
                desiredLane = 0;
                }
            }                                           <-- Can all be done with the code below
        else //Moving right
            {
            desiredLane++;
            if(desiredLane == 3)
                {
                desiredLane = 1;
                }
            } */
       
        desiredLane += (goingRight ? 1 : -1);
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
        }

    void MoveLaneDirection()
        {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || InputScript.Instance.SwipeLeft)
            {
            MoveLane(false);
            }
        if (Input.GetKeyDown(KeyCode.RightArrow) || InputScript.Instance.SwipeRight)
            {
            MoveLane(true);
            }
        }
   
    void SetTargetPosition()
        {
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        if (desiredLane == 0)
            {
            targetPosition += Vector3.left * LANE_DISTANCE;
            } else
        if (desiredLane == 2)
            {
            targetPosition += Vector3.right * LANE_DISTANCE;
            }

        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).x * speed;

        CheckGrounded();

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        characterController.Move(moveVector * Time.deltaTime); //Makes the game move at same rate no matter the type of phone (as opposed to CODM that needs phone with high framerates)

        Vector3 dir = characterController.velocity;
        if(dir != Vector3.zero)
            {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
            }
        
        }
    
    bool IsGrounded()
        {
        Ray groundRay = new Ray(
            new Vector3(
                characterController.bounds.center.x,
                (characterController.bounds.center.y - characterController.bounds.extents.y) + 0.2f, characterController.bounds.center.z),
                Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.red, 1.0f);
        return Physics.Raycast(groundRay, 0.2f + 0.1f);
        }

    void CheckGrounded()
        {
        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        if (isGrounded) //On the ground
            {
            verticalVelocity = -0.1f;
            if (Input.GetKeyDown(KeyCode.Space) || InputScript.Instance.SwipeUp)
                {
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
                }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || InputScript.Instance.SwipeDown)
                {
                StartSliding();
               Invoke("StopSliding", 1.0f);
                }
            } 
        else
            {
            verticalVelocity -= (gravity * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space) || InputScript.Instance.SwipeDown)
                {
                verticalVelocity -= jumpForce;
                }
            }
        }
   
    public void StartRunning()
        {
        isRunning = true;
        anim.SetTrigger("StartRunning"); 
        }
    
    void StartSliding()
        {
        anim.SetBool("Sliding", true);
        characterController.height /= 2;
        characterController.center = new Vector3(characterController.center.x, characterController.center.y /2, characterController.center.z);     
        }

    void StopSliding()
        {
        anim.SetBool("Sliding", false);
        characterController.height *= 2;
        characterController.center = new Vector3(characterController.center.x, characterController.center.y * 2, characterController.center.z);
        }

    void Crash()
        {
        anim.SetTrigger("Death");
        isRunning = false;
        GameManager.instance.OnDeath();
        }

    void SetSpeed()
        {
        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
            {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.instance.UpdateModifier(speed - originalSpeed);
            }
        }
    private void OnControllerColliderHit(ControllerColliderHit hit)
        {
        switch (hit.gameObject.tag)
            {
            case "Obstacle":
                Crash();
                break;
            }
        }
    }
 