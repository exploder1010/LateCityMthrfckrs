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
    public float doubleJumpTimeSet;
    public float doubleJumpStartImpulse;
    public float doubleJumpVelocityAdd;

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
                doubleJumpTimer = 0;
                break;

            default:
                break;
        }

        endCarJump(input);
    }

    protected override void updateCarJump()
    {
        //add full hop to car jump while jump is held
        if (carJumpTimer > 0)
        {
            carJumpVelocity += (carJumpVelocityAdd / carJumpTimeSet) * Time.deltaTime; //add full hop normalized to 1 second

            carJumpTimer -= Time.deltaTime;

            vectorToAdd = new Vector3(vectorToAdd.x, carJumpVelocity, vectorToAdd.z);

        }
        else if (doubleJumpTimer > 0)
        {
            doubleJumpVelocity += (doubleJumpVelocityAdd / doubleJumpTimeSet) * Time.deltaTime; //add full hop normalized to 1 second

            doubleJumpTimer -= Time.deltaTime;

            vectorToAdd = new Vector3(vectorToAdd.x, doubleJumpVelocity, vectorToAdd.z);

        }
    }

    public virtual void beginDoubleJump()
    {
        if(curAbilityAmmo > 0)
        {
            curAbilityAmmo--;

            rb.velocity = rb.velocity.normalized * maxSpeedThisJump * 0.2f;

            doubleJumpTimer = doubleJumpTimeSet;

            rb.AddForce(Vector3.up * doubleJumpStartImpulse);

            if (briefcasePrefab)
            {
                Instantiate(briefcasePrefab, transform.position, transform.rotation);
            }
        }
        
    }
}
