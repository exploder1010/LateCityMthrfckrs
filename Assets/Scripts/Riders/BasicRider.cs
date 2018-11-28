using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRider : MonoBehaviour, IRider {

    //nick's todo:
    //cleanup
    //polish jump boost threshold
    //make sure jump boost threshold is visible
    //make sure car weight gets polished
    //misc polishing

    bool cameraLockOn = false;

    //references to set in prefab
    public Rigidbody rb;
    public Animator charAnim;
    public GameObject ragdollPrefab;
    public HitBox lockOnCollider;
    public HitBox roadCollider;
    public HitBox vehicleCollider;
    public HitBox brokenVehicleCollider;
    public HitBox goalCollider;
    protected GameObject currentRagdoll;
    private Vector3 ragdollUp;
    private GameObject prevVehicle;

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
    protected Vector3 prevVectorToAdd;
    protected Vector3 horVelocityCheck;
    protected float maxSpeedThisJump;

    //movement variables
    public float airAccel = 30; //Rate at which player accelerates in air
    public float boostThreshold = 0.85f; //Point that previous car speed needs to surpass for player to move faster than the previous car

    //car jump variables
    public float carJumpTimeSet = 0.15f;
    public float carJumpStartImpulse = 300;
    public float carJumpVelocityAdd = 200;
    protected Vector3 carJumpUpDirection;
    protected float carJumpTimer;
    protected float carJumpVelocity;
    protected float carLaunchSpeed;

    //gravity variable
    public float gravityMagnitude = 4.5f;
    public Vector3 gravityDirection = Vector3.down;

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

        handleRoadCollision();
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
        if (input == 1 && false)// && prevVectorToAdd!= Vector3.zero)
        {
            //BasicVehicle closestVehicle = null;
            //handleLockOnCollision();
            foreach (BasicVehicle bv in closeVehicles)
            {
                if (!bv.broken && (rb.velocity.y < 0 || prevVehicle != bv.gameObject))
                {
                    //vectorToAdd = vectorToAdd.normalized;
                    Vector3 dir = prevVectorToAdd;
                    if (cameraLockOn)
                    {
                        dir = calculateForward();
                    }

                    

                    float dist = (transform.position + (dir * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - bv.transform.position).magnitude;

                    //float dist = DistanceToLine(new Ray (transform.position + vectorToAdd, vectorToAdd), bv.transform.position);
                    if (targetedVehicle == null || dist < (transform.position + (dir * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - targetedVehicle.transform.position).magnitude)
                    {
                        targetedVehicle = bv;

                    }
                }
               
            }
            if(targetedVehicle!= null)
            {
                float dist = (targetedVehicle.transform.position - transform.position).magnitude;

                //DONT CHANGE THIS FOR NOW -- NICK
                //new max speed and new start speed as a percent of current speed based on proximity to targeted car. right next to car = 99%, farthest away (but still in range) = 1%.
                storedNewCarMaxSpeed = Mathf.Max(30f, (rb.velocity.magnitude * 0.3f)) + (rb.velocity.magnitude * 1.1f) * Mathf.Max(((lockOnCollider.transform.GetComponent<SphereCollider>().radius + vehicleCollider.transform.GetComponent<SphereCollider>().radius - dist) / (lockOnCollider.transform.GetComponent<SphereCollider>().radius + vehicleCollider.transform.GetComponent<SphereCollider>().radius)), 0);
                //storedNewCarStartSpeed = Mathf.Max(30f, (rb.velocity.magnitude * 0.5f)) + (rb.velocity.magnitude * 0.2f) * Mathf.Max(((lockOnCollider.transform.GetComponent<SphereCollider>().radius + vehicleCollider.transform.GetComponent<SphereCollider>().radius - dist) / (lockOnCollider.transform.GetComponent<SphereCollider>().radius + vehicleCollider.transform.GetComponent<SphereCollider>().radius)), 0);
                storedNewCarStartSpeed = 0.6f * storedNewCarMaxSpeed;
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
            if(vectorToAdd == Vector3.zero && rb.velocity.magnitude > maxSpeedThisJump * 0.5f)
            {
                float mag = rb.velocity.magnitude;
                rb.velocity = rb.velocity.normalized * (mag - Time.deltaTime * 50f);
            }

            //constrain to max speed
            updateMaxSpeedCheck();

            //apply gravity
            rb.AddForce(gravityDirection * gravityMagnitude);

        }

        prevVectorToAdd = vectorToAdd.normalized; //use this for anything outside of the normal update (like starting abilities)
        vectorToAdd = Vector3.zero; //reset at the end of each update
    }

    //begin initial jump from vehicle
    public virtual void beginCarJump(Vector3 carVelocity, float carMaxSpeed, Vector3 newCarJumpUpDirection)
    {
        carJumpUpDirection = newCarJumpUpDirection;

        carLaunchSpeed = Mathf.Max(carVelocity.magnitude, 20f);
        float boostModifier = Mathf.Max((carLaunchSpeed / (carMaxSpeed * boostThreshold)), 1);
        maxSpeedThisJump = carLaunchSpeed * boostModifier;
        rb.velocity = (new Vector3(carVelocity.x, Mathf.Max(0, carVelocity.y), carVelocity.z).normalized ) * maxSpeedThisJump;
        rb.AddForce(carJumpUpDirection * Mathf.Max(carJumpStartImpulse, carJumpStartImpulse * boostModifier));

            //Debug.Log("carspeed: " + carVelocity + " boostModifier " + boostModifier);
            //Debug.Log("cls " +carLaunchSpeed);
        
        //else
        //{
        //    carLaunchSpeed = 2f;
        //    //float boostModifier = Mathf.Max((carVelocity.magnitude / (carMaxSpeed * boostThreshold)), 0.25f);
        //    maxSpeedThisJump = 20f;
        //    rb.velocity = carVelocity.normalized * carLaunchSpeed;
        //    rb.AddForce(carJumpUpDirection * 2f * carJumpStartImpulse);

        //    //Debug.Log("carspeed: " + carVelocity + " boostModifier " + boostModifier);
        //    //Debug.Log("cls " + carLaunchSpeed);
        //}


        carJumpTimer = carJumpTimeSet;
    }

    //hold initial jump from vehicle
    protected virtual void updateCarJump()
    {
        //add full hop to car jump while jump is held
        if (carJumpTimer > 0)
        {
            carJumpVelocity += ((carJumpVelocityAdd) / carJumpTimeSet) * Time.deltaTime; //add full hop normalized to 1 second

            carJumpTimer -= Time.deltaTime;

            vectorToAdd = vectorToAdd + carJumpUpDirection * carJumpVelocity;

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

    protected virtual void handleRoadCollision()
    {
        if (targetedVehicle == null && roadCollider.collidersCount() > 0 )//|| brokenVehicleCollider.collidersCount() > 0)
        {
            ragdollUp = Vector3.up;
            RaycastHit hit;
            int layerMask = 1 << LayerMask.NameToLayer("Road");
            Debug.DrawLine(transform.position - rb.velocity.normalized, transform.position - rb.velocity.normalized + rb.velocity.normalized * 5f, Color.red, 10f);
            if (Physics.Raycast(transform.position - rb.velocity.normalized, rb.velocity.normalized, out hit, 5f, layerMask))
            {
               // Debug.Log("hit");
                ragdollUp = hit.normal;
            }

            spawnRagdoll();
        }

    }

    protected virtual void spawnRagdoll()
    {
        if (currentRagdoll == null)
        {
            currentRagdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            currentRagdoll.SetActive(true);
            //Debug.Log(ragdollUp);
            currentRagdoll.GetComponent<RagdollStorage>().rb.velocity = (rb.velocity * 3f) + (ragdollUp * rb.velocity.magnitude * 4f);
            
        }
    }

    protected virtual void handleVehicleCollision()
    {
        if (vehicleCollider.collidersCount() > 0)//&& (rb.velocity.y < 0 || targetedVehicle!= null)
        {
            if (!vehicleCollider.returnColliders()[0].transform.root.transform.GetComponent<BasicVehicle>().broken)
            {
                hitVehicle = vehicleCollider.returnColliders()[0].transform.root.transform.GetComponent<BasicVehicle>();
            }
        }
    }

    protected virtual void handleGoalCollision()
    {
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

        if(targetIconPrefab != null && false)
        {
            BasicVehicle potentialTargetedVehicle = null;
            Destroy(currentTargetIcon);
            currentTargetIcon = null;
            
            if(vectorToAdd != Vector3.zero)
            {
                foreach (BasicVehicle bv in closeVehicles)
                {
                    //vectorToAdd = vectorToAdd.normalized;
                    if (!bv.broken && (rb.velocity.y < 0 || prevVehicle != bv.gameObject))
                    {
                        Vector3 dir = prevVectorToAdd;
                        if (cameraLockOn)
                        {
                            dir = calculateForward();
                        }

                        float dist = (transform.position + (dir * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - bv.transform.position).magnitude;

                        //float dist = DistanceToLine(new Ray (transform.position + vectorToAdd, vectorToAdd), bv.transform.position);
                        if (potentialTargetedVehicle == null || dist < (transform.position + (dir * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - potentialTargetedVehicle.transform.position).magnitude)
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

    //let playercontroller know to enter a car
    public virtual void rejectVehicleToEnter()
    {
        hitVehicle = null ;
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
            return Mathf.Max(20f, (rb.velocity.magnitude * 0.5f)) + (rb.velocity.magnitude * 0.6f);//dont change this yet - nick
            //return Mathf.Max(20f, (rb.velocity.magnitude * 1.05f));//dont change this yet - nick
        }
        return storedNewCarMaxSpeed;
    }


    //set new current speed for car based on current air speed
    public virtual float calculateNewCarStartSpeed()
    {
        if (!targetedVehicle)
        {
            return 0.6f * (Mathf.Max(20f, (rb.velocity.magnitude * 0.5f)) + (rb.velocity.magnitude * 0.6f));//dont change this yet - nick
            //return 0.6f * Mathf.Max(20f, (rb.velocity.magnitude * 1.05f));//dont change this yet - nick
        }
        return storedNewCarStartSpeed;
    }

    public virtual void destroyThis()
    {
        Destroy(currentTargetIcon);
        Destroy(transform.gameObject);
    }

    public virtual void setPreviousVehicle(GameObject newPrevVehicle)
    {
        prevVehicle = newPrevVehicle;
    }

}
