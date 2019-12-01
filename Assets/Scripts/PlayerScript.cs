using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    Text checkText;
    
    //which player it is. 0 is left, 1 is right
    public int playerNum;

    public RetroSpriteAnimator m_animator;

    //these can be tweaked in the editor
    public float moveSpeed;
    public float swingDuration;
    public float recoveryDuration;

    public float aimAngle;

    //you can add stuff here and just drag a sprite on it in the editor
    //same for the ball, if you want a different visual there
    //spriteRenderer.sprite = whateverSprite; is how you change picture
    private SpriteRenderer spriteRenderer;
    public Collider swingCollider;
    public Sprite idleSprite;
    public Sprite swingSprite;
    public Sprite hitSprite;
    public Sprite recoverySprite;
    public Sprite dieSprite;
    public Sprite runSprite;

    //input mapping (just set these in the editor)
    public KeyCode hitKey = KeyCode.C;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode leftKey = KeyCode.A;

    [HideInInspector]
    public Vector3 position;

    Rigidbody rb;
    public float jumpForce=390f;

    //if you want to add more abilities or states of the player, add them in here
    //and then put them in Update() so the game knows what to do during them
    public enum PlayerState
    {
        NORMAL,
        SWINGING,
        HITTING,
        SWING_RECOVERY,
        GET_HIT,
        DEAD,
        RUN,
    }
    PlayerState playerState = PlayerState.NORMAL;
    Vector3 startPosition;


    private void Awake()
    {
        m_animator.AddAnimation(
            Name: "RUN",
            Frames: new int[] { 17, 18, 19, 20 },
            FrameRate: 5
            );
        
             
    }
    void Start()
    {
        position = startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        swingCollider.enabled = false;
        rb = this.GetComponent<Rigidbody>();
    }

    //checking if we hit the ball
    public void OnTriggerStay(Collider otherCollider)
    {
        BallScript ball = otherCollider.GetComponent<BallScript>();
        if (ball != null && ball.lastHitter != playerNum) //don't hit the ball twice
        {
            //you can do exemptions or calculations with this depending on the ball or the player
            float hitPause = 0.1f;
            
            ball.GetHit(this, hitPause);//send info to the ball
            StopAllCoroutines(); //stop the swinging 
            StartCoroutine(HitCoroutine(hitPause)); //start the hitting with the right hitpause duration
        }
    }

    //checking if we get hit
    public void OnCollisionEnter(Collision collision)
    {
        BallScript ball = collision.rigidbody.GetComponent<BallScript>();

        //only if the ball is flying around, you could put other exemptions here
        if (ball != null && ball.ballState == BallScript.BallState.NORMAL) 
        {
            StopAllCoroutines(); //stop whatever you're doing
            StartCoroutine(DieCoroutine()); //start dying
            ball.StartCoroutine(ball.HitPlayerCoroutine(this)); //remove the ball but let it know who it killed
        }
    }

    //we update the state of the player
    //what sprite to show and what hitbox to enable
    //you can add more abilities here as PlayerStates, more or other hitboxes etc
    void Update()
    {
        switch (playerState)
        {
            case PlayerState.NORMAL:
                swingCollider.enabled = false;
                spriteRenderer.sprite = idleSprite;

                //this is where we can check to start actions
                //since the player is doing nothing right now
                if (Input.GetKeyDown(hitKey))
                {
                    StartCoroutine(SwingCoroutine());
                }

                if (Input.GetKey(upKey))
                    //rb.AddForce(transform.up * jumpForce);
                    position += Vector3.up * moveSpeed;

                if (Input.GetKey(downKey))
                    position -= Vector3.up * moveSpeed;

                if (Input.GetKeyDown(rightKey) || (Input.GetKeyDown(leftKey)))
                {
                    playerState = PlayerState.RUN;
                }

               
                
               
                break;

            case PlayerState.RUN:
                //move around
                m_animator.PlayAnimation("RUN");
                //spriteRenderer.sprite = runSprite;
                if (Input.GetKey(rightKey))
                {
                    position += Vector3.right * moveSpeed;
                    playerState = PlayerState.RUN;
                    transform.localScale = new Vector3(2, 2, 2);
                }

                if (Input.GetKey(leftKey))
                {
                    position += Vector3.left * moveSpeed;
                    playerState = PlayerState.RUN;
                    transform.localScale = new Vector3(-2, 2, 2);
                }


                position.x = Mathf.Clamp(position.x, -8f, 8f);
                position.y = Mathf.Clamp(position.y, -3.5f, 2.5f);
                break;
            case PlayerState.SWINGING:
                swingCollider.enabled = true;
                spriteRenderer.sprite = swingSprite;
                break;

            case PlayerState.HITTING:
                swingCollider.enabled = true;
                spriteRenderer.sprite = hitSprite;
                //as we're hitting, we can aim the ball
                //feel free to remove or alter this completely

                //for setting specific directions you can use: 
                //new Vector2(0.5f, 0.5f); is diagonally up for example
                //see it as distance from zero, the direction is the line between zero and your x y numbers
                //Vector3.up or down etc also works



                if (playerNum == 0)//left side player
                {

                    float angle = .0f, angleDivider = 1.0f;

                    if (Input.GetKey(upKey)) angle -= aimAngle;

                    if (Input.GetKey(downKey)) angle += aimAngle;

                    if (Input.GetKeyUp(rightKey))
                    {
                        angleDivider = 2.0f;
                        Debug.Log("keyup");
                    }
                    checkText.text = "angle:" + angle + "" + "angleDivider:" + angleDivider;


                    angle /= angleDivider;
                        angle += 90.0f;


                    //if (Input.GetKey(leftKey))
                    //{
                    //    angleDivider = 2.0f;
                    //    angle /= angleDivider;
                    //    angle -= 180.0f;
                    //}


                    BallScript.ball.direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), .0f);
                }
                else
                {
                    float angle = .0f, angleDivider = 1.0f;
                    if (Input.GetKey(upKey)) angle += aimAngle;
                    if (Input.GetKey(downKey)) angle -= aimAngle;
                    if (Input.GetKey(leftKey)) angleDivider = 2.0f;

                    angle /= angleDivider;
                    angle -= 90.0f;

                    BallScript.ball.direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), .0f);
                }






                //tip: you can use BallScript.ball to access the ball from anywhere
                break;
            case PlayerState.SWING_RECOVERY:
                swingCollider.enabled = false;
                spriteRenderer.sprite = recoverySprite;
                break;

            case PlayerState.GET_HIT:
                swingCollider.enabled = false;
                spriteRenderer.sprite = dieSprite;
                break;

            case PlayerState.DEAD:
                swingCollider.enabled = false;
                spriteRenderer.sprite = dieSprite;
                position += Vector3.down * 0.25f;
                break;
        }

        if ((Input.GetKeyUp(rightKey)) || (Input.GetKeyUp(leftKey)))
        {
            playerState = PlayerState.NORMAL;
           
        }

        transform.position = position;
    }

    //these are for doing the timings
    //after WaitForSeconds it goes on where it left off
    //if you want to add different abilities you can copy one over and change it

    public IEnumerator SwingCoroutine()
    {
        playerState = PlayerState.SWINGING;
        yield return new WaitForSeconds(swingDuration);

        playerState = PlayerState.SWING_RECOVERY;
        yield return new WaitForSeconds(recoveryDuration);

        playerState = PlayerState.NORMAL;
    }

    public IEnumerator HitCoroutine(float hitPause)
    {
        playerState = PlayerState.HITTING;
        yield return new WaitForSeconds(hitPause);

        playerState = PlayerState.SWING_RECOVERY;
        yield return new WaitForSeconds(recoveryDuration);

        playerState = PlayerState.NORMAL;
    }

    public IEnumerator DieCoroutine()
    {
        playerState = PlayerState.GET_HIT;
        yield return new WaitForSeconds(0.2f);

        playerState = PlayerState.DEAD;
        yield return new WaitForSeconds(2.0f);

        playerState = PlayerState.NORMAL;
        position = startPosition;
    }
}