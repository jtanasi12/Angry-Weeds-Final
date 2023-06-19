using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // ******* Global Variables *******

    [SerializeField]
    private float moveForce = 10f;

    [SerializeField]
    private float jumpForce = 11f;

    [SerializeField]
    private AudioSource jumpSFX;

    [SerializeField]
    private AudioSource landingSFX;
    [SerializeField] private AudioSource runningSFX;

    private float movementX;
    private float movementY;

    [SerializeField]
    private Rigidbody2D myBody;

    private Animator anim;
    private SpriteRenderer spriteR;

    [SerializeField] private SpriteRenderer[] PlayerSprites;
    private SpriteRenderer SpriteGun;

    private string WALK_ANIMATION = "Walk"; //VPC 6/13 "run_side" changing from "Walk"; to accomodate new asset animation
    private string JUMP_ANIMATION = "isJumping"; 
    private string GROUND_TAG = "Ground";
    private string GUN_ANIMATION = "hasPGun";

    private bool isGrounded;
    private const float rightSideOfScreen = 98.47478f;
    private const float leftSideOfScreen  = -98.47478f;
    [SerializeField] private AudioSource pickUpSFX;
    public int ammoCount = 0;

    [SerializeField]  private bool hasPGUN = false;

    // ******* Global Variables *******

    private void Awake()
    {
        // Getting Components so we can manipulate them in code
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteR = GetComponent<SpriteRenderer>();

        //VPC 6/18 - Not sure why this doesn't work. I tried putting it in the OnCollisionEnter2D function below
        // so that it would only try to get it when active, but that doesn't seem to make a difference
        PlayerSprites = GetComponentsInChildren<SpriteRenderer>(true);
        // VPC - This is horrible, but just getting it to see if it can work for now. Maybe fix in future to find it by name
        SpriteGun = PlayerSprites[1];
    }

    // Start is called before the first frame update
    void Start(){}

    // Update is called once per frame
    void Update()
    {
        PlayerMoveKeyBoard();
        animatePlayer();
        PlayerJump();
      //  renderPGun();
    }


    private void PlayerMoveKeyBoard()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        // Gets A and D keys or left and right arrow keys

        movementY = Input.GetAxisRaw("Vertical");

        //Transform is built in for the game object 


        if(transform.position.x < -99)

        {
            transform.position = new Vector3(rightSideOfScreen, transform.position.y, 0f);

        }
        else if(transform.position.x > 99)
        {
            transform.position = new Vector3(leftSideOfScreen, transform.position.y, 0f);

        }
        else
        {
            transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;

        }

    }

    void animatePlayer()
    {
        if (movementX > 0) // Going to the right side
        {
            if (!runningSFX.isPlaying && isGrounded)
                runningSFX.Play();
            anim.SetBool(WALK_ANIMATION, true);
            spriteR.flipX = true; // Going to the right side, VPC 6/13 - have to flip t/f for new sprite
            SpriteGun.flipX = true;
            SpriteGun.transform.SetLocalPositionAndRotation(new Vector2(0.35f, 0.109f), Quaternion.identity);
        }
        else if (movementX < 0) // Going to the left 
        {
            if (!runningSFX.isPlaying && isGrounded)
                runningSFX.Play();
            anim.SetBool(WALK_ANIMATION, true);
            spriteR.flipX = false; // Going to the left size, VPC 6/13 - have to flip t/f for new sprite
            SpriteGun.flipX = false;
            SpriteGun.transform.SetLocalPositionAndRotation(new Vector2(-0.234f, 0.109f), Quaternion.identity);
        }
        else // The player is not moving 
        {
            runningSFX.Pause();
            anim.SetBool(WALK_ANIMATION, false);

        }
    }

    void PlayerJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false; // Allows us to not jump two times
            runningSFX.Pause();
            jumpSFX.Play();
            myBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            anim.SetBool(JUMP_ANIMATION, true); // VPC 6/14 - adding the setting of jump animation for characters
        }
    }

    // Checks to see if the player is colliding onto the ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If player and the ground collides 
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            if (!isGrounded) landingSFX.Play(); // need the if to stop constant collision boops
            isGrounded = true; // The player is on the ground

            anim.SetBool(JUMP_ANIMATION, false); // VPC 6/14 - turning off jump animation when hitting ground
        }
        else if (collision.gameObject.tag == "Ammo")
        {
            pickUpSFX.Play();
            Destroy(collision.gameObject);
            ammoCount += Random.Range(1, 5);
        }
        else if (collision.gameObject.tag == "PotatoGun") {
            pickUpSFX.Play();
            Destroy(collision.gameObject);
            hasPGUN = true;
            anim.SetBool(GUN_ANIMATION, true);
        }
    }
/*
    private void renderPGun()
    {
        if (hasPGUN)
        {
            spriteGun.enabled =true;
        }
        else spriteGun.enabled = false;
    }
*/
}
