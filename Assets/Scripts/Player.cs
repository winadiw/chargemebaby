using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.UI;
using Com.MyCompany.MyGame;
using UnityEngine.SceneManagement;
public class Player : Photon.PunBehaviour, IPunObservable
{
    public Vector3 theScale;
    private Collider2D collider;
	private Rigidbody2D rigidBody;
	private	Animator animator;
	private float speed = 0.5f;
	public Transform startCast;
	public Transform endCast;
    public float currentCoolDown;
	public float cooldown;
    private int health;

    //Jump Variables
    public bool grounded = false;
	public Vector3 jumpSpeed;
	public int jumpCount;
    private GameManager gameManager;
	//Dash Attack Variables
	public Vector3 dashSpeed;
	public bool isCharging = false;
	public bool isDashing = false;
    Vector3 move;
	public bool attack = false;
    public bool isDead = false;
    public int maxJump;

    //Sfx for Player
    // public AudioClip moveSound;
    public AudioClip jumpSound;
    //public AudioClip attackSound;
    public AudioClip chargeSound;

	[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
	public static GameObject LocalPlayerInstance;

	void Awake()
	{
        health = 3;
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.isMine)
		{
			Player.LocalPlayerInstance = this.gameObject;
            
        }
        if(!photonView.isMine)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1.0f, 0.0f, 0.0f);
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () 
	{
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (photonView.isMine)
        {
            renderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        animator = GetComponent<Animator> ();
		collider = GetComponent<Collider2D> ();
		rigidBody = GetComponent<Rigidbody2D> ();
        //jumpSpeed = new Vector3 (0, 5f, 0);
        dashSpeed = new Vector3 (0, 0, 0);
        currentCoolDown = cooldown;
        //gameManager.disableYouLose();
        
    }
    // Update is called once per frame
    void Update () 
	{
        if (photonView.isMine == false && PhotonNetwork.connected == true )
		{
			return;
		}
        if(isDead==false)
        {
            Jump();
            isAttacking();
            PlayerMove();
        }
        
		if(rigidBody.velocity.x == 0)
		{
			isDashing = false;
		}

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

        if(isDead==false)
        {
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
    }

    public void OnCollisionEnter2D(Collision2D coll) 
    {
        if (coll.gameObject.tag == "Ground") 
        {
            grounded = true;
            //if (grounded)   
            jumpCount = maxJump;
        }
    }

    [PunRPC]
    void Kill()
    {
        if (!photonView.isMine)
        {
            Debug.Log("Masuk if Kill");
            return;
        }

        StartCoroutine(gameEnd());
    }

    IEnumerator gameEnd()
    {
        Debug.Log("Masuk Coroutine playerDead()");
        isDead = true;
        animator.SetTrigger("PlayerDead");
        gameManager.enableYouLose();
        yield return new WaitForSeconds(2f);
        gameManager.EndRoom();

        //PhotonNetwork.Destroy(this.gameObject);
        //SoundManager.instance.destroyMusic();
    }

    public void isAttacking()
    {
        RaycastHit2D hit = Physics2D.Raycast(startCast.position, endCast.position);
        if (hit != false && hit.collider.gameObject.tag == "Player" && isDashing)
        {
            PhotonView target = hit.collider.gameObject.GetComponent<PhotonView>();
            StartCoroutine(killSelf(target));
            //gameManager.EndRoom();
        }
    }

    IEnumerator killSelf(PhotonView view)
    {
        view.photonView.RPC("Kill", PhotonTargets.All);
        yield return new WaitForSeconds(2.5f);
        PhotonNetwork.LeaveRoom();
    }

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(theScale);
		}else{
			// Network player, receive data
			this.theScale = (Vector3)stream.ReceiveNext();
		}
	}

    public void OnLeftRoom()
    {
        Debug.Log("Left Room Called!");
        SceneManager.LoadScene(0);
    }

}