using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    [Header("Grappling")]
    [SerializeField]
    private LayerMask grabbaleLayer;
    [SerializeField]
    private float lineLength = 10f;
    [SerializeField]
    private float grapplingSpeed = 2f;
    [SerializeField]
    private int ringPartCounts = 360;

    private int keyHeldDownCounter = 0;

    private float ringSteps;

    private Vector2 aimingDirection;

    private Vector2 aimingEnd;

    private float grappingStartTime;
    private Vector2 grapplingStartPosition;
    private Vector2 grapplingEndPosition;
    private float grappingLength;
    private bool isGrappling;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 7f;

    private float dirX = 0f;





    private enum MovementState { idle, running, falling }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision name: " + collision.gameObject.name);

        if (!isGrappling) return;

        bool isGrabbale = collision.gameObject.layer == grabbaleLayer;

        //if (isGrabbale)
        //{
        //    EndGrappling();
        //    Debug.Log("isGrabable detacted!");
        //    transform.parent = collision.gameObject.transform;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);


        if (Input.GetButtonUp("Fire1"))
        {
            EndAim();
        }

        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        if (Input.GetButton("Fire1"))
        {
            StartAim();
        }

        if (Input.GetButton("Release"))
        {
            transform.parent = null;
            EndGrappling();
            Debug.Log("released! ");
        }

        Grappling();
    }

    private void StartAim()
    {

        float degree = keyHeldDownCounter * ringSteps;

        if (sprite.flipX)
        {
            degree = -degree;
        }

        aimingDirection = RotateVector(Vector2.up, -degree).normalized;

        aimingEnd = new Vector2(transform.position.x, transform.position.y) + lineLength * aimingDirection;

        Debug.DrawLine(transform.position, aimingEnd, Color.red);


        keyHeldDownCounter++;
    }

    private void EndAim()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimingDirection, lineLength, grabbaleLayer);
        if (hit.collider != null)
        {
            Debug.Log("contact point: " + hit.point + ", name: "+ hit.collider.name );
            StartGrappling(hit.point);
        }



        keyHeldDownCounter = 0;
    }

    private void StartGrappling(Vector2 aGrapplingDestination)
    {
        isGrappling = true;

        grappingStartTime = Time.time;
        grapplingStartPosition = transform.position;
        grapplingEndPosition = aGrapplingDestination;
        grappingLength = Vector2.Distance(grapplingStartPosition, grapplingEndPosition);
    }

    private void Grappling()
    {
        if(isGrappling)
        {
            float distCovered = (Time.time - grappingStartTime) * grapplingSpeed;
            float fractionOfJourney = distCovered / grappingLength;

            Vector3 newPosition = Vector3.Lerp(grapplingStartPosition, grapplingEndPosition, fractionOfJourney);
            rb.MovePosition(newPosition);
        }
    }

    private void EndGrappling()
    {
        isGrappling = false;
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

        if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }

}
