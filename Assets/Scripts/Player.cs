using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;

public class Player : MonoBehaviour
{
    public Vector3 theScale;
    private Collider2D collider;
	private Rigidbody2D rigidBody;
	private	Animator animator;
	private float speed = 0.7f;
	public Transform startCast;
	public Transform endCast;
    public float currentCoolDown;
	public float cooldown;

	//Jump Variables
	public bool grounded = false;
	public Vector3 jumpSpeed;
	public int jumpCount;

	//Dash Attack Variables
	public Vector3 dashSpeed;
	public bool isCharging = false;
	public bool isDashing = false;
    Vector3 move;
	public bool attack = false;
    public bool isDead = false;

    //Sfx for Player
    // public AudioClip moveSound;
    public AudioClip jumpSound;
    //public AudioClip attackSound;
    public AudioClip chargeSound;

    // Use this for initialization
    void Start () 
	{
		animator = GetComponent<Animator> ();
		collider = GetComponent<Collider2D> ();
		rigidBody = GetComponent<Rigidbody2D> ();

		//jumpSpeed = new Vector3 (0, 5f, 0);
		dashSpeed = new Vector3 (0, 0, 0);
        currentCoolDown = cooldown;
    }
	
	// Update is called once per frame
	void Update () 
	{
        Jump();
		if(rigidBody.velocity.x == 0)
		{
			isDashing = false;
		}
			
		isAttacking ();
		PlayerMove();
		//Debug.Log (rigidBody.velocity.x);
        if(currentCoolDown < cooldown)
            currentCoolDown += Time.deltaTime;
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded || jumpCount > 0)
            {
                rigidBody.velocity = jumpSpeed;
                grounded = false;
                animator.SetTrigger("PlayerJump");
                --jumpCount;
                SoundManager.instance.PlaySingle(jumpSound); //sfx for player jumping
            }
        }

    }

    public void PlayerMove()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        theScale = transform.localScale;

        /*if (GameManager.paused == false)
		{ */
        if (Input.GetKey(KeyCode.D))
        {
                if (!isCharging && !isDashing)
                {
                    if (theScale.x < 0)
                    {
                        theScale.x *= -1;
                        transform.localScale = theScale;
                    }

                    transform.position += move * speed * Time.deltaTime;
                    if (grounded)
                    {
                        animator.ResetTrigger("PlayerCharge");
                        animator.SetTrigger("PlayerMove");
                        //SoundManager.instance.PlaySingle(moveSound); //sfx for player walking
                    }

                    else
                        animator.SetTrigger("PlayerJump");
                }

        }
		else if (Input.GetKey (KeyCode.Z)) 
		{
			animator.SetTrigger ("PlayerDead");
		}

        else if (Input.GetKey(KeyCode.A))
        {
            if (!isCharging && !isDashing)
            {
                if (theScale.x > 0)
                {
                    theScale.x *= -1;
                    transform.localScale = theScale;
                }
                transform.position += move * speed * Time.deltaTime;
                if (grounded)
                {
                    animator.SetTrigger("PlayerMove");
                    //SoundManager.instance.PlaySingle(moveSound); //sfx for player walking
                }

                else
                    animator.SetTrigger("PlayerJump");
            }
        }

        else
        {
            if (grounded && !isCharging && !isDashing)
            {
                animator.ResetTrigger("PlayerCharge");
                animator.ResetTrigger("PlayerRelease");
                animator.SetTrigger("PlayerIdle");
            }
            else if (isCharging)
            {
                animator.ResetTrigger("PlayerIdle");
                animator.SetTrigger("PlayerCharge");
                SoundManager.instance.PlaySingle(chargeSound); //sfx for player walking
            }
            else if (isDashing)
            {

                animator.SetTrigger("PlayerRelease");
            }
            else if (!grounded)
            {
                animator.SetTrigger("PlayerJump");
                //SoundManager.instance.PlaySingle(jumpSound); //sfx for player jumping
            }
        }
        
        if(Input.GetMouseButton(0))
        {
            if(grounded && currentCoolDown>=cooldown)
            {
                isCharging = true;
                if(dashSpeed.x <= 2.5f)
                    dashSpeed.x += 0.05f;

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if(grounded && currentCoolDown >= cooldown)
            {
                isCharging = false;
                isDashing = true;
                if(theScale.x < 0)
                    rigidBody.velocity = dashSpeed * -1;
                else
                    rigidBody.velocity = dashSpeed;
                currentCoolDown = 0;

        }
            dashSpeed.x = 0;
        }

    }

    public void OnCollisionEnter2D(Collision2D coll) 
{
    if (coll.gameObject.tag == "Ground") 
    {
        grounded = true;
        //if (grounded)   
        jumpCount = 2;
    }
}

    public void Kill(bool attacked)
    {
        if (attacked)
        {
            StartCoroutine(playerDead());
        }
    }

    IEnumerator playerDead()
    {
        isDead = true;
        animator.SetTrigger("PlayerDead");
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        //SoundManager.instance.destroyMusic();
    }

    //Pake cara lineCast
    public void isAttacking()
    {
        /*
        attack = Physics2D.Linecast (startCast.position, endCast.position, 1 << LayerMask.NameToLayer ("Enemy"));
        if (attack && isDashing) 
        {
            Debug.Log ("Musuh Mati!");
            SendMessageUpwards
        }
        */

        RaycastHit2D hit = Physics2D.Raycast(startCast.position, endCast.position);
        if (hit != false && hit.collider.name == "Enemy" && isDashing)
        {
            hit.collider.SendMessageUpwards("Kill", true);
        }
    }
}