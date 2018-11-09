using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRider : MonoBehaviour, IRider {

    //nick's todo:
    //get carjump to be modified by maxspeed
    //improve hitbox/detection
    //let player select car with joystick (and visualize selected car)
    //polish
    //test car hop rotation move

    //references to set in prefab
    public Rigidbody rb;
    public Animator charAnim;
    public GameObject ragdollPrefab;
    public HitBox roadCollider;
    public HitBox vehicleCollider;
    public HitBox lockOnCollider;
    protected GameObject currentRagdoll;

    public GameObject targetIconPrefab;
    GameObject currentTargetIcon;

    //references to set in external start
    protected Transform cTransform;

    //close vehicles
    protected List<BasicVehicle> closeVehicles = new List<BasicVehicle>();
    protected BasicVehicle targetedVehicle;
    protected BasicVehicle hitVehicle;
    protected float storedNewCarStartSpeed;
    protected float storedNewCarMaxSpeed;

    //current variables
    protected Vector3 vectorToAdd;
    protected Vector3 horVelocityCheck;
    protected float maxSpeedThisJump;

    //movement variables
    public float airAccel = 30; //Rate at which player accelerates in air
    public float boostThreshold = 40; //Point that previous car speed needs to surpass for player to move faster than the previous car

    //car jump variables
    public float carJumpTimeSet = 0.15f;
    public float carJumpStartImpulse = 300;
    public float carJumpVelocityAdd = 200;
    protected float carJumpTimer;
    protected float carJumpVelocity;
    protected float carLaunchSpeed;
    
    //gravity variable
    public float gravityMagnitude = 18f;

    // basic player doesn't use these variables but all characters will use them so storing them here
    public int charAbilityAmmo = 1;
    protected int curAbilityAmmo = 0;

    //---------------------------start:
    // Use this for initialization (to ensure things happen in proper order)
    public void externalStart(Transform newCam)
    {
        cTransform = newCam;
        curAbilityAmmo = charAbilityAmmo;
        storedNewCarStartSpeed = 0;
        storedNewCarMaxSpeed = 0;
    }

    //---------------------------update:
    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        handleLockOnCollision();

        if (targetedVehicle)
        {
            breakInMove();
        }
        else
        {
            updateMovement();
        }


        updateAnimation();
        if (roadCollider.collidersCount() > 0 )
        {
            handleRoadCollision();
        }

        handleVehicleCollision();

    }

    //---------------------------input:
    //input rightwards and leftwards movement
    public virtual void inputHorz(float direction)
    {
        vectorToAdd = vectorToAdd + (Vector3.Cross(Vector3.up, calculateForward()) * direction); //add input to a refreshed VectorToAdd each frame
    }

    //input forwards and backwards movement
    public virtual void inputVert(float direction)
    {
        vectorToAdd = vectorToAdd + (calculateForward() * direction); //add input to a refreshed VectorToAdd each frame
    }

    //get forward based on camera
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

    //attempt to perform ability
    public virtual void inputAbility(int input)
    {
        endCarJump(input);
    }

    //attempt to start break in move
    public virtual void inputBreakIn(int input)
    {
        if (input == 1 && rb.velocity.y < 0)
        {
            //BasicVehicle closestVehicle = null;
            //handleLockOnCollision();
            foreach (BasicVehicle bv in closeVehicles)
            {
                if (!bv.broken)
                {
                    //vectorToAdd = vectorToAdd.normalized;

                    float dist = (transform.position + (vectorToAdd * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - bv.transform.position).magnitude;
                    Debug.Log("dist" + dist);
                    //float dist = DistanceToLine(new Ray (transform.position + vectorToAdd, vectorToAdd), bv.transform.position);
                    if (targetedVehicle == null || dist < (transform.position + (vectorToAdd * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - targetedVehicle.transform.position).magnitude)
                    {
                        targetedVehicle = bv;

                    }
                }
               
            }
            if(targetedVehicle!= null)
            {
                storedNewCarMaxSpeed = rb.velocity.magnitude;
                storedNewCarStartSpeed = rb.velocity.magnitude * 0.75f;
            }
        }
    }

    //---------------------------update movement:
    //update horizontal movement
    protected virtual void updateMovement()
    {
        vectorToAdd = vectorToAdd.normalized * airAccel; //make sure diagonals aren't overpowered, and apply speed to normalized vector.

        if (rb)
        {
            updateCarJump();

            //apply force to rigidbody
            rb.AddForce(vectorToAdd);

            //constrain to max speed
            updateMaxSpeedCheck();

            //apply gravity
            rb.AddForce(Vector3.down * gravityMagnitude);

        }

        vectorToAdd = Vector3.zero; //reset at the end of each update
    }

    //begin initial jump from vehicle
    public virtual void beginCarJump(float carSpeed)
    {
        carLaunchSpeed = carSpeed;
        float boostModifier = Mathf.Max((carSpeed / boostThreshold), 0.25f);
        maxSpeedThisJump = Mathf.Max(30, carSpeed * boostModifier);
        rb.velocity = transform.forward * carSpeed * boostModifier;

        Debug.Log("carspeed: " + carSpeed + " boostModifier " + boostModifier);
        
        carJumpTimer = carJumpTimeSet;

        rb.AddForce(Vector3.up * Mathf.Max(carJumpStartImpulse, carJumpStartImpulse * boostModifier));
    }

    //hold initial jump from vehicle
    protected virtual void updateCarJump()
    {
        //add full hop to car jump while jump is held
        if (carJumpTimer > 0)
        {
            carJumpVelocity += ((carJumpVelocityAdd * (carLaunchSpeed / boostThreshold)) / carJumpTimeSet); //add full hop normalized to 1 second

            carJumpTimer -= Time.deltaTime;

            vectorToAdd = new Vector3(vectorToAdd.x, carJumpVelocity, vectorToAdd.z);

        }
    }
    
    //end initial jump from vehicle
    protected virtual void endCarJump(int input)
    {

        if (input == 3)
        {
            carJumpTimer = 0f;
            carJumpVelocity = 0f;
        }

    }

    //constrain max speed
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
    
    //move towards selected car
    protected virtual void breakInMove()
    {
        rb.velocity = Vector3.zero;
        Vector3 direction = (targetedVehicle.transform.position - transform.position).normalized;
        float dist = (targetedVehicle.transform.position - transform.position).magnitude;
        rb.velocity = direction * 50f; //*(transform.GetComponent<SphereCollider>().radius / dist)

    }

    //---------------------------update collisions:

    //old way
    ////add cars to close cars list
    //protected virtual void OnTriggerStay(Collider other)
    //{
    //    if (rb.velocity.y < 0)
    //    {
    //        if (other.transform.root.transform.GetComponent<BasicVehicle>() && !closeVehicles.Contains(other.transform.root.transform.GetComponent<BasicVehicle>()))
    //        {
    //            closeVehicles.Add(other.transform.root.transform.GetComponent<BasicVehicle>());
    //        }
    //    }

    //}

    ////remove cars from close cars list
    //protected virtual void OnTriggerExit(Collider other)
    //{
    //    if (other.transform.root.transform.GetComponent<BasicVehicle>())
    //    {
    //        closeVehicles.Remove(other.transform.root.transform.GetComponent<BasicVehicle>());
    //    }
    //}

    protected virtual void handleRoadCollision()
    {
        if(currentRagdoll == null)
        {
            currentRagdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            currentRagdoll.SetActive(true);
            currentRagdoll.GetComponent<RagdollStorage>().rb.velocity = new Vector3(rb.velocity.x * 5f,-rb.velocity.y * 2f, rb.velocity.z * 5f);
            
        }

        
    }

    protected virtual void handleVehicleCollision()
    {
        if (vehicleCollider.collidersCount() > 0 && (rb.velocity.y <= 0 || targetedVehicle!= null))
        {
            if (!vehicleCollider.returnColliders()[0].transform.root.transform.GetComponent<BasicVehicle>().broken)
            {
                hitVehicle = vehicleCollider.returnColliders()[0].transform.root.transform.GetComponent<BasicVehicle>();
            }
            else
            {
                handleRoadCollision();
            }
        }
    }

    protected virtual void handleLockOnCollision()
    {

        closeVehicles = new List<BasicVehicle>();
        //if (rb.velocity.y < 0)
        //{
        for (int i = 0; i < lockOnCollider.collidersCount(); i++)
        {
            closeVehicles.Add(lockOnCollider.returnColliders()[i].transform.root.transform.GetComponent<BasicVehicle>());
        }
        //}

        if(targetIconPrefab != null)
        {
            BasicVehicle potentialTargetedVehicle = null;
            Destroy(currentTargetIcon);
            currentTargetIcon = null;
            
            if(vectorToAdd != Vector3.zero)
            {
                foreach (BasicVehicle bv in closeVehicles)
                {
                    //vectorToAdd = vectorToAdd.normalized;
                    if (!bv.broken)
                    {
                        float dist = (transform.position + (vectorToAdd * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - bv.transform.position).magnitude;
                        Debug.Log("dist" + dist);
                        //float dist = DistanceToLine(new Ray (transform.position + vectorToAdd, vectorToAdd), bv.transform.position);
                        if (potentialTargetedVehicle == null || dist < (transform.position + (vectorToAdd * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - potentialTargetedVehicle.transform.position).magnitude)
                        {
                            potentialTargetedVehicle = bv;
                        }
                    }

                    
                }
                if (potentialTargetedVehicle != null)
                {
                    currentTargetIcon = Instantiate(targetIconPrefab, potentialTargetedVehicle.transform.position, Quaternion.identity);
                    //currentTargetIcon.transform.parent = transform;
                    currentTargetIcon.transform.LookAt(cTransform);
                }
            }

        }
        
    }

    //let playercontroller know player is dead
    public virtual GameObject checkRagdoll()
    {
        return currentRagdoll;
    }

    //let playercontroller know to enter a car
    public virtual BasicVehicle vehicleToEnter()
    {
        return hitVehicle;
    }

    //---------------------------update animation:
    protected virtual void updateAnimation()
    {
        transform.LookAt(transform.position + rb.velocity);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    //---------------------------enter new car:
    //set new max speed for car based on current air speed
    public virtual float calculateNewCarMaxSpeed()
    {
        if (!targetedVehicle)
        {
            return rb.velocity.magnitude;
        }
        return storedNewCarMaxSpeed;
    }

    //set new current speed for car based on current air speed
    public virtual float calculateNewCarStartSpeed()
    {
        if (!targetedVehicle)
        {
            return rb.velocity.magnitude * 0.75f;
        }
        return storedNewCarStartSpeed;
    }

    public virtual void destroyThis()
    {
        Destroy(currentTargetIcon);
        Destroy(transform.gameObject);
    }

}
