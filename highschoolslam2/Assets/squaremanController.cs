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
	bool grounded = false; //player is on ground
	public Transform groundCheck;
	float groundRadius = 0.2f;
	public LayerMask whatIsGround;
	int held = 0; //how long the jump button is held
	float shortHopForce = 600;
	float fullHopForce = 1100;
	int landingLagMaxFrames = 4; //landing lag of jumps
	int landingLag = 0; //landing lag counter
	bool attacking = false; //user is attacking
	public Collider2D [] jab; //hitboxes in the jab
	int attackFrame = 0; //frame of attack
	Collider2D [] currentAttack; //current attack
	float [] currentDamages; //current damage done by attack
	float [] currentKnockbackAngles;
	bool multihit = false; //dictates if the most can hit multiple times
	bool hit = false; //whether the player has landed a hit

	// Use this for initialization
	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();	
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float speed = walkSpeed;
		if (Input.GetButton("Sprint1")){
			speed = sprintSpeed; //increases speed with sprint
		}
		if (attacking){
			speed = 0; //user stops moving when attacking
		}
		float move = Input.GetAxis("Horizontal1");
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

		if (Input.GetButtonDown("Weak1")){
			anim.SetBool("weak", true);
			attacking = true;
			currentAttack = jab;
			currentDamages = new float [] {4, 4, 4};
			currentKnockbackAngles = new float [] {20, 25, 30};
			multihit = false;
		}
		else {
			anim.SetBool("weak", false);
		}

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
		if (anim.GetBool("ground") && landingLag >= landingLagMaxFrames && rigidbody2D.velocity.y == 0){
			if (Input.GetButton("Jump1") && held < 10){
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

	void UpdateAttack(){
		attackFrame++;
		if (attackFrame > currentAttack.Length || (hit && !multihit)){
			currentAttack = new Collider2D [0];
			attacking = false;
			attackFrame = 0;
			currentDamages = new float [0];
			currentKnockbackAngles = new float [0];
			multihit = false;
			hit = false;
		}
		else {
			Collider2D frame = currentAttack[attackFrame-1];
			float angle = Quaternion.Angle(Quaternion.Euler(new Vector3(0,0,0)),frame.transform.rotation);
			Collider2D col = Physics2D.OverlapBox(new Vector2(frame.bounds.center[0],frame.bounds.center[1]), 
												new Vector2(frame.bounds.extents[0], frame.bounds.extents[1]), 
												angle,
												LayerMask.GetMask("Hurtbox2"));
			//print(new Vector2(frame.bounds.center[0],frame.bounds.center[1]));
			//print(new Vector2(frame.bounds.extents[0], frame.bounds.extents[1]));
			//print(Quaternion.Angle(Quaternion.Euler(new Vector3(0,0,0)),frame.transform.rotation));

			try{
				print("You hit the " + col.name + " with a vector of "+ forceVector(currentKnockbackAngles[attackFrame-1], 400f) + "!");
				col.transform.root.GetComponent<Rigidbody2D>().AddForce(forceVector(currentKnockbackAngles[attackFrame-1], 400f));
				hit = true;
			} catch{
				print("You hit nothing.");
			}
			
		}
	}

	Vector2 forceVector(float angle, float magnitude){
		angle *= Mathf.PI/180;
		int sign = 1;
		if (!facingRight) sign = -1;
		return new Vector2(Mathf.Cos(angle)*magnitude*sign, Mathf.Sin(angle)*magnitude);
	}
}

