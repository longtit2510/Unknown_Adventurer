using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStat PS;

    private Rigidbody2D rb;
    private Animator anim;

    private float movementDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;

    public float movementSpeed = 5f;
    public float jumpForce = 10f;
    public float groundCheckRadius = 0.5f;
    public float wallCheckDistance = 0.5f;
    public float wallSlidingSpeed = 3f;
    public float movementForceInAir = 4f;
    public float airDragMultiplier = 0.95f;
    public float jumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet = 0.1f;
    public float turnTimerSet = 0.1f;
    public float wallJumpTimerSet = 0.5f;
    private float knockbackStartTime;
    [SerializeField]
    private float knockbackDuration;

    public int amountOfJumps = 1;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;
    [SerializeField]
    private Vector2 knockbackSpeed;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool CheckJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool knockback;
    private bool outOfBound = false;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    private int lastWallJumpDirection;


    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private Transform wallCheck;

    private CameraFollow cam;

    [SerializeField]
    private LayerMask whatisGround;

    // Start is called before the first frame update
    void Start()
    {
        PS = GetComponent<PlayerStat>();

        cam = GameObject.Find("Main Camera").GetComponent<CameraFollow>();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        amountOfJumpsLeft = amountOfJumps;

        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
        CheckKnockback();
        CheckOutOfCamera();
        UpdateAnimation();
        if (outOfBound)
        {
            PS.DecreaseHealth(200);
        }
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }
    private void CheckOutOfCamera()
    {
        if(transform.position.y < -7.5f)
        {
            outOfBound = true;
        }
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatisGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatisGround);
    }
    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementDirection == facingDirection && rb.velocity.y < 0)
        {
            isWallSliding = true;
        } else
        {
            isWallSliding = false;
        }
    }
    public void Knockback(int direction)
    {
        knockback = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);

    }

    private void CheckKnockback()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }
    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }
        if (isTouchingWall)
        {
            canWallJump = true;
        }
        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else canNormalJump = true;
    }

    private void CheckMovementDirection()
    {
        if(isFacingRight && movementDirection < 0)
        {
            Flip();
        } else if (!isFacingRight && movementDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) > 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimation()
    {
        anim.SetBool(TagManager.WALK_ANIMATION_PARAMETER, isWalking);
        anim.SetBool(TagManager.ISGROUND_ANIMATION_PARAMETER, isGrounded);
        anim.SetFloat(TagManager.YVELOCITY_ANIMATION_PARAMETER, rb.velocity.y);
        anim.SetBool(TagManager.WALLSLIDE_ANIMATION_PARAMETER, isWallSliding);
    }
    private void CheckInput()
    {
        movementDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            } else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }
        if(Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if(!isGrounded && movementDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }
        if (!canMove)
        {
            turnTimer -= Time.deltaTime;
            if(turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }
        if (CheckJumpMultiplier && !Input.GetKey(KeyCode.Space))
        {
            CheckJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpHeightMultiplier);
        }
    }
    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            
            if(!isGrounded && isTouchingWall && movementDirection != 0 && movementDirection != facingDirection)
            {
                FindObjectOfType<AudioManager>().Play("Start Jump");
                WallJump();
            } else if (!isGrounded)
            {
                FindObjectOfType<AudioManager>().Play("Start Jump");
                NormalJump();
            }
        }
        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }
        if(wallJumpTimer > 0)
        {
            if(hasWallJumped && movementDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, -1f);
                hasWallJumped = false;
            } else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            } else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }
    private void NormalJump()
    {
        if (canNormalJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            CheckJumpMultiplier = true;
        }
    }
    private void WallJump()
    {
        if (canWallJump)//Wall Jump
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);

            jumpTimer = 0;

            isAttemptingToJump = false;
            CheckJumpMultiplier = true;

            turnTimer = 0;

            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;

            lastWallJumpDirection = -facingDirection;
        }
    }
    private void ApplyMovement()
    {
        if (!isGrounded && !isWallSliding && movementDirection == 0 && !knockback)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (canMove && !knockback)
        {
            rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
        }
        
        
        if (isWallSliding)
        {
            if (rb.velocity.y < wallSlidingSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
            }
        }
    }
    public int GetFacingDirection()
    {
        return facingDirection;
    }
    private void DisableFlip()
    {
        canFlip = false;
    }
    private void EnableFlip()
    {
        canFlip = true;
    }
    private void Flip()
    {
        if (!isWallSliding && canFlip && !knockback)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance,wallCheck.position.y,wallCheck.position.z));
    }
}
