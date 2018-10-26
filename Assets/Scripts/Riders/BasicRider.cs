using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRider : MonoBehaviour, IRider {

    //references to set in prefab
    public Rigidbody rb;
    public Animator charAnim;
    public GameObject ragdollPrefab;
    protected GameObject currentRagdoll;

    //references to set in external start
    protected Transform cTransform;

    //close vehicles
    protected List<BasicVehicle> closeVehicles = new List<BasicVehicle>();
    protected BasicVehicle targetedVehicle;

    //current variables
    protected Vector3 vectorToAdd;
    protected Vector3 horVelocityCheck;
    protected float maxSpeedThisJump;

    //movement variables
    public float airAccel; //Rate at which player accelerates in air
    public float boostThreshold; //Point that previous car speed needs to surpass for player to move faster than the previous car

    //car jump variables
    protected float carJumpTimer;
    protected float carJumpVelocity;
    public float carJumpTimeSet;
    public float carJumpStartImpulse;
    public float carJumpVelocityAdd;

    // basic player doesn't use these variables but all characters will use them so storing them here
    public int charAbilityAmmo = 1;
    protected int curAbilityAmmo = 0;

    // Use this for initialization (to ensure things happen in proper order)
    public void externalStart(Transform newCam)
    {
        cTransform = newCam;
        curAbilityAmmo = charAbilityAmmo;
    }
	
	// Update is called once per frame
	protected virtual void FixedUpdate () {
        
        if (targetedVehicle)
        {
            breakInMove();
        }
        else
        {
            updateMovement();
        }


        updateAnimation();
    }

    protected virtual void breakInMove()
    {
        rb.velocity = Vector3.zero;
        Vector3 direction = (targetedVehicle.transform.position - transform.position).normalized;
        float dist = (targetedVehicle.transform.position - transform.position).magnitude;
        rb.velocity = direction * 50f ; //*(transform.GetComponent<SphereCollider>().radius / dist)

    }

    protected virtual void updateMovement()
    {
        vectorToAdd = vectorToAdd.normalized * airAccel; //make sure diagonals aren't overpowered, and apply speed to normalized vector.

        if (rb)
        {
            updateCarJump();

            //apply force to rigidbody
            rb.AddForce(vectorToAdd);

            updateMaxSpeedCheck();

        }

        vectorToAdd = Vector3.zero; //reset at the end of each update
    }

    protected virtual void updateCarJump()
    {
        //add full hop to car jump while jump is held
        if (carJumpTimer > 0)
        {
            carJumpVelocity += (carJumpVelocityAdd / carJumpTimeSet) * Time.deltaTime; //add full hop normalized to 1 second

            carJumpTimer -= Time.deltaTime;

            vectorToAdd = new Vector3(vectorToAdd.x, carJumpVelocity, vectorToAdd.z);

        }
    }

    protected virtual void updateMaxSpeedCheck()
    {
        ////max speed check, and reduce horizontal velocity if needed;
        horVelocityCheck = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (horVelocityCheck.magnitude > maxSpeedThisJump)
        {
            float saveY = rb.velocity.y;
            horVelocityCheck = horVelocityCheck.normalized;
            horVelocityCheck *= maxSpeedThisJump;
            rb.velocity = new Vector3(horVelocityCheck.x, saveY, horVelocityCheck.z);
        }
    }

    protected virtual void updateAnimation()
    {
        transform.LookAt(transform.position + rb.velocity);
        transform.rotation = Quaternion.Euler(0,transform.eulerAngles.y,0);
    }

    public virtual void inputHorz(float direction)
    {
        vectorToAdd = vectorToAdd + (Vector3.Cross(Vector3.up, calculateForward()) * direction); //add input to a refreshed VectorToAdd each frame
    }

    public virtual void inputVert(float direction)
    {
        vectorToAdd = vectorToAdd + (calculateForward() * direction); //add input to a refreshed VectorToAdd each frame
    }

    public virtual void inputAbility(int input)
    {
        endCarJump(input);
    }
    

    public virtual void inputBreakIn(int input)
    {
        if(input == 1)
        {
            //BasicVehicle closestVehicle = null;
            foreach (BasicVehicle bv in closeVehicles)
            {
                float dist = (transform.position - bv.transform.position).magnitude;
                if (targetedVehicle == null || dist < (transform.position - targetedVehicle.transform.position).magnitude)
                {
                    targetedVehicle = bv;
                }
            }
        }
    }

    public virtual BasicVehicle vehicleToEnter()
    {
        if (targetedVehicle)
        {
            float dist = (transform.position - targetedVehicle.transform.position).magnitude;
            if (dist < 1)
            {
                return targetedVehicle; //return targeted vehicle
            }
            return null; //return targeted vehicle
        }

        BasicVehicle closestVehicle = null;
        foreach(BasicVehicle bv in closeVehicles)
        {
            float dist = (transform.position - bv.transform.position).magnitude;
            if (dist < 1 && (closestVehicle == null || dist < (transform.position - closestVehicle.transform.position).magnitude))
            {
                closestVehicle = bv; 
            }
        }
        return closestVehicle; // return the closest vehicle that is less than 1 unit away
    }

    //kill player
    protected virtual void OnCollisionEnter(Collision other)
    {
        Debug.Log("hit " + LayerMask.LayerToName(other.gameObject.layer));
        if(LayerMask.LayerToName( other.gameObject.layer) == "Road" && currentRagdoll == null)
        {
            currentRagdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            currentRagdoll.SetActive(true);
            currentRagdoll.GetComponent<RagdollStorage>().rb.velocity = rb.velocity * 5f;
        }
    }

    public virtual GameObject checkRagdoll()
    {
        return currentRagdoll;
    }

    //add cars to close cars list
    protected virtual void OnTriggerStay(Collider other)
    {
        if(rb.velocity.y < 0)
        {
            if (other.transform.root.transform.GetComponent<BasicVehicle>() && !closeVehicles.Contains(other.transform.root.transform.GetComponent<BasicVehicle>()))
            {
                closeVehicles.Add(other.transform.root.transform.GetComponent<BasicVehicle>());
            }
        }
        
    }

    //remove cars from close cars list
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.transform.root.transform.GetComponent<BasicVehicle>())
        {
            closeVehicles.Remove(other.transform.root.transform.GetComponent<BasicVehicle>());
        }
    }

    public virtual void beginCarJump(float carSpeed)
    {
        float boostModifier = (carSpeed / boostThreshold);
        maxSpeedThisJump = Mathf.Max(30, carSpeed * boostModifier);
        rb.velocity = transform.forward * carSpeed * boostModifier;

        carJumpTimer = carJumpTimeSet;

        rb.AddForce(Vector3.up * Mathf.Max(150f, carJumpStartImpulse * boostModifier));
    }

    protected virtual void endCarJump(int input)
    {

        if(input == 3)
        {
            carJumpTimer = 0f;
            carJumpVelocity = 0f;
        }

    }

    public virtual float calculateNewCarMaxSpeed()
    {
        return rb.velocity.magnitude;
    }

    public virtual float calculateNewCarStartSpeed()
    {
        return rb.velocity.magnitude * 0.75f;
    }

    protected Vector3 calculateForward()
    {
        if (!cTransform)
        {
            if (GameObject.FindGameObjectWithTag("MainCamera").transform)
            {
                cTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
            }
        }
        else
        {
            Vector3 forward = (this.transform.position - cTransform.position);
            forward.y = 0;
            forward = forward.normalized;
            //Debug.Log("forward " + forward);
            return forward;
        }
        return Vector3.zero;
    }
}
