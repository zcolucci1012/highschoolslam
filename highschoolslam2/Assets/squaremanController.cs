using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squaremanController : MonoBehaviour {

	public float walkSpeed = 8f;
	public float sprintSpeed = 12f;
	public float MAX_VELOCITY = 20f;
	Rigidbody2D rigidbody2D;
	Animator anim;
	bool facingRight = true;
	bool grounded = false;
	public Transform groundCheck;
	float groundRadius = 0.2f;
	public LayerMask whatIsGround;
	int held = 0;
	float shortHopForce = 600;
	float fullHopForce = 1100;
	int landingLagMaxFrames = 4;
	int landingLag = 0;

	// Use this for initialization
	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();	
		anim = GetComponent<Animator>();
		Physics2D.IgnoreLayerCollision(12, 13);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float speed = walkSpeed;
		if (Input.GetButton("Sprint")){
			speed = sprintSpeed;
		}
		float move = Input.GetAxis("Horizontal");
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
		anim.SetBool("ground", grounded);
		anim.SetFloat("vSpeed", rigidbody2D.velocity.y);
		anim.SetFloat("speed", Mathf.Abs(move * speed));


		float velY = rigidbody2D.velocity.y;
		if (Mathf.Abs(rigidbody2D.velocity.y) > MAX_VELOCITY){
			velY = MAX_VELOCITY * Mathf.Sign(rigidbody2D.velocity.y);
		}

		rigidbody2D.velocity = new Vector2(move * speed, velY);
		if (move > 0 && !facingRight){
			Flip();
		}
		else if (move < 0 && facingRight){
			Flip();
		}
	}

	void Update(){
		if (anim.GetBool("ground") && landingLag >= landingLagMaxFrames){
			if (Input.GetButton("Jump") && held < 10){
				held++;
				anim.SetBool("jumpsquat", true);
			}
			else {
				if (held < 5 && held > 0){
					ShortHop();
				}
				else if (held > 0){
					FullHop();
				}
				held = 0;
			}
		}
		else {
			held = 0;
			anim.SetBool("jumpsquat", false);
			if (landingLag < landingLagMaxFrames){
				landingLag++;
			}
		}
	}

	void ShortHop(){
		anim.SetBool("ground", false);
		anim.SetBool("jumpsquat", false);
		rigidbody2D.AddForce(new Vector2(0, shortHopForce));
		landingLag = 0;
		print("short hop" + held);
	}

	void FullHop(){
		anim.SetBool("ground", false);
		anim.SetBool("jumpsquat", false);
		rigidbody2D.AddForce(new Vector2(0, fullHopForce));
		landingLag = 0;
		print("full hop" + held);
	}

	void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
