﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour
{
    public Vector3 theScale;
    private Collider2D collider;
	private Rigidbody2D rigidBody;
	private	Animator animator;
	private float speed = 0.7f;
	public Transform groundCheck;
    public float currentCoolDown;
    public float cooldown;


	//Jump Variables
	public bool grounded = false;
	public Vector3 jumpSpeed;
	public int jumpCount = 2;

	//Dash Attack Variables
	public Vector3 dashSpeed;
	public bool isCharging = false;
	public bool isDashing = false;
    Vector3 move;

    // Use this for initialization
    void Start () 
	{
		animator = GetComponent<Animator> ();
		collider = GetComponent<Collider2D> ();
		rigidBody = GetComponent<Rigidbody2D> ();

		jumpSpeed = new Vector3 (0, 2.5f, 0);
		dashSpeed = new Vector3 (0, 0, 0);
        currentCoolDown = cooldown;
    }
	
	// Update is called once per frame
	void Update () 
	{
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space))
		{
			if (grounded || jumpCount > 0)
			{
				rigidBody.velocity = jumpSpeed;
				grounded = false;	
				animator.SetTrigger("PlayerJump");
				--jumpCount;
			}
		}

		if(rigidBody.velocity.x == 0)
		{
			isDashing = false;
		}

		isMidair ();
		PlayerMove();
		Debug.Log (rigidBody.velocity.x);
        if(currentCoolDown < cooldown)
            currentCoolDown += Time.deltaTime;
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
                    }

                    else
                        animator.SetTrigger("PlayerJump");
                }

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

		/*}*/
	}

	public void isMidair()
	{
		grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		if (grounded)   
			jumpCount = 2;
	}
}
