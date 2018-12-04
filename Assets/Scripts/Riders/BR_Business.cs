using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_Business : BasicRider {

    //business specific variables

    //references set in editor
    public GameObject briefcasePrefab;

    //double jump variables
    protected float doubleJumpTimer;
    protected float doubleJumpVelocity;
    public float doubleJumpTimeSet = 0.15f;
    public float doubleJumpStartImpulse = 300f;
    public float doubleJumpVelocityAdd = 200f;
    public float doubleJumpHorizontalModifier = .2f;
    public float maxSpeedThisJumpDJModifier = 1.2f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		
	}

    public override void inputAbility(int input)
    {
        switch (input)
        {
            case 0: //none

                break;
            case 1: //button down
                beginDoubleJump();
                break;
            case 2: //button stay

                break;
            case 3: //button up
                carJumpTimer = 0;
                doubleJumpTimer = 0;
                break;

            default:
                break;
        }

        endCarJump(input);
    }

    protected override void updateCarJump()
    {
        base.updateCarJump();

        if (doubleJumpTimer > 0)
        {
            //doubleJumpVelocity += (doubleJumpVelocityAdd / doubleJumpTimeSet) * Time.deltaTime; //add full hop normalized to 1 second

            doubleJumpTimer -= Time.deltaTime;

            float newY = rb.velocity.y;
            newY += (doubleJumpVelocityAdd / doubleJumpTimeSet) * carJumpModifier * Time.deltaTime;
            rb.velocity = new Vector3(rb.velocity.x, newY, rb.velocity.z);

            //rb.AddForce(transform.up * ((doubleJumpVelocityAdd) / carJumpTimeSet));
            //vectorToAdd = new Vector3(vectorToAdd.x, doubleJumpVelocity, vectorToAdd.z);

        }
    }

    public virtual void beginDoubleJump()
    {
        if(curAbilityAmmo > 0)
        {
            charAnim.SetBool("Special_Buisness", true);
            curAbilityAmmo--;

            //rb.velocity = prevVectorToAdd * maxSpeedThisJump * doubleJumpHorizontalModifier;
            //rb.velocity = Vector3.zero;
            //maxSpeedThisJump = maxSpeedThisJump * maxSpeedThisJumpDJModifier;
            carJumpVelocity = 0;
            rb.velocity = new Vector3(rb.velocity.x, doubleJumpStartImpulse, rb.velocity.z);


            doubleJumpTimer = doubleJumpTimeSet;

            
            //doubleJumpStartImpulserb.AddForce(Vector3.up * doubleJumpStartImpulse);

            if (briefcasePrefab)
            {
                Instantiate(briefcasePrefab, transform.position, transform.rotation);
            }
        }
        
    }
    protected override void updateGravity()
    {
        //apply gravity
        if (carJumpTimer <= 0 && doubleJumpTimer <= 0)
        {

            rb.AddForce(gravityDirection * gravityMagnitude);
        }
    }
}
