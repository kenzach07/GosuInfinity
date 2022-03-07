using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,       // 0
        Walking,    // 1
        Flying,     // 2
        Stunned     // 3
    }

    public PlayerState curState;            // current player state
                                            // values
    public float moveSpeed;                 // force applied horizontally when moving
    public float flyingSpeed;               // force applied upwards when flying
    public bool grounded;                   // is the player currently standing on the ground?
    public float stunDuration;              // duration of a stun
  
    private float stunStartTime;            // time that the player was stunned
                                            // components
    public Rigidbody2D rig;                 // Rigidbody2D component
    public Animator anim;                   // Animator component
    public ParticleSystem jetpackParticle;  // ParticleSystem of jetpack
    
    protected Joystick joystick; // call the joystick components

    public static PlayerController instance;

    void Awake()
    {
        // set instance to this script.
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // moves the player horizontally
    void Move()
    {
        Vector3 dirA = Vector3.zero;
        dirA.x = Input.GetAxis("Horizontal");
        dirA.y = Input.GetAxis("Vertical");
        joystick = FindObjectOfType<Joystick>();
        var rigidbody = GetComponent<Rigidbody>();

     
        
        // get horizontal axis (A & D, Left Arrow & Right Arrow)
        Input.GetAxis("Horizontal");
        // flip player to face the direction they're moving
        if (joystick.Horizontal > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (joystick.Horizontal < 0)
            transform.localScale = new Vector3( -1, 1, 1);

        // set rigidbody horizontal velocity
        rig.velocity = new Vector2(joystick.Horizontal * moveSpeed, rig.velocity.y);
        rig.AddForce(joystick.Vertical * Vector2.up * flyingSpeed, ForceMode2D.Impulse); // to nake the player fly
        jetpackParticle.Play(); // always play the jetpack particle kase may bug haha
    }

    void Fly()
    {
        // add force upwards
        rig.AddForce(Vector2.up * flyingSpeed, ForceMode2D.Impulse);
        // play jetpack particle effect
        if (!jetpackParticle.isPlaying)
            jetpackParticle.Play();
    }

    // returns true if player is on ground, false otherwise
    bool IsGrounded()
    {
        // shoot a raycast down underneath the player
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.85f), Vector2.down, 0.3f);
        // did we hit anything?
        if (hit.collider != null)
        {
            // was it the floor?
            if (hit.collider.CompareTag("Floor"))
            {
                return true;
            }
            
        }
        return false;
    }

    // sets the player's state
    void SetState()
    {
        // don't worry about changing states if the player's stunned
        if (curState != PlayerState.Stunned)
        {
            // idle
            if (rig.velocity.magnitude == 0 && grounded)
                
                curState = PlayerState.Idle;
            // walking
            if (rig.velocity.x != 0 && grounded)
                
                curState = PlayerState.Walking;
            // flying
            if (rig.velocity.magnitude != 0 && !grounded)
            
            curState = PlayerState.Flying;
        }
        // tell the animator we've changed states
        anim.SetInteger("State", (int)curState);
    }


   



    // called when the player gets stunned
    public void Stun()
    {
   
        curState = PlayerState.Stunned;
        rig.velocity = Vector2.down * 3;
        stunStartTime = Time.time;
        jetpackParticle.Stop();

        GameObject.Find("GameManager").GetComponent<GameManager>().lifeMinus(); // call the life minus function if stunned

    }


  

    // checks for user input to control player
    void CheckInputs()
    {
        if (curState != PlayerState.Stunned)
        {
            // movement
            Move();
            // flying

          
        }
        // update our current state
        SetState();
    }

    void FixedUpdate()
    {
        grounded = IsGrounded();
        CheckInputs();
        // is the player stunned?
        if (curState == PlayerState.Stunned)
        {
            // has the player been stunned for the duration?
            if (Time.time - stunStartTime >= stunDuration)
            {
                curState = PlayerState.Idle;

                //invulnerable effect for player after being stunned
                GameObject.Find("Player").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .5f);

                StartCoroutine(EnableBoxB(3.0F));

            }

            //reject colliders for obstacles after being stunned
            GameObject.Find("Obstacle1").GetComponent<CircleCollider2D>().enabled = false;
            GameObject.Find("Obstacle2").GetComponent<CircleCollider2D>().enabled = false;
            GameObject.Find("Obstacle3").GetComponent<CircleCollider2D>().enabled = false;
            
            StartCoroutine(EnableBox(3.0F));
        }
    }

    //return the colliders for obstacles after 3 seconds
    IEnumerator EnableBox(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GameObject.Find("Obstacle1").GetComponent<CircleCollider2D>().enabled = true;
        GameObject.Find("Obstacle2").GetComponent<CircleCollider2D>().enabled = true;
        GameObject.Find("Obstacle3").GetComponent<CircleCollider2D>().enabled = true;
        
    }

    //return the original opacity of the player sprites after the invulnerability effect
    IEnumerator EnableBoxB(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GameObject.Find("Player").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    // called when the player enters another object's collider
    void OnTriggerEnter2D(Collider2D col)
    {
        // if the player isn't already stunned, stun them if the object was an obstacle
        if (curState != PlayerState.Stunned)
        {
            if (col.GetComponent<Obstacle>())
            {
                Stun();
            }
        }
    }
}
