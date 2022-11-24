using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    private float dirX = 0f;
    [SerializeField]
    private float moveSpeed = 7f;
    [SerializeField]
    private float jumpForce = 14f;
    [SerializeField]
    private float lineLength = 10f;
    [SerializeField]
    private LayerMask jumpableGround;


    private int keyHeldDownCounter = 0;
    [SerializeField]
    private int ringPartCounts = 360;
    private float ringSteps;

    private enum MovementState { idle, running, jumping, falling }

    private MovementState state = MovementState.idle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        ringSteps = (float)360 / ringPartCounts;
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if(Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity=new Vector2(rb.velocity.x, jumpForce);
        }



        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        if (Input.GetButton("Fire1"))
        {
            StartAim();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            EndAim();
        }
    }

    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }

    private void StartAim()
    {
        Debug.Log("start aimming");

        float degree = keyHeldDownCounter * ringSteps;

        Vector2 aimingDirection = RotateVector(Vector2.up, -degree);

        Debug.Log(aimingDirection);

        Debug.DrawRay(transform.position, aimingDirection, Color.red);
        keyHeldDownCounter++;
    }

    private void EndAim()
    {
        Debug.Log("stop aimming");
        keyHeldDownCounter = 0;
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if(rb.velocity.y > 0.1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
