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
    public bool off;
    bool cameraLockOn = false;

    //references to set in prefab
    public GameObject briefcasePrefab;
    public Rigidbody rb;
    public Animator charAnim;
    public GameObject ragdollPrefab;
    private Vector3 rdSpawnPoint;
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
    protected bool horzAddDone;
    protected bool vertAddDone;
    protected bool dontNormalize; 
    protected Vector3 prevVectorToAdd;
    protected Vector3 horVelocityCheck;
    protected float maxSpeedThisJump;
    protected float speed;
    protected float InputDirection;
    protected float turn;

    //movement variables
    public float airAccel = 30; //Rate at which player accelerates in air
    public float boostThreshold = 0.85f; //Point that previous car speed needs to surpass for player to move faster than the previous car

    //car jump variables
    public float carJumpTimeSet = 0.15f;
    public float carJumpStartImpulse = 300;
    public float carJumpVelocityAdd = 200;
    public float maxFallSpeed = 50;
    protected Vector3 carJumpUpDirection;
    protected float carJumpTimer;
    protected float carJumpVelocity;
    protected float carLaunchSpeed;

    protected float carJumpModifier;

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
        //handleLockOnCollision();

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

        if (off)
        {
            rb.velocity = Vector3.zero;
        }
    }

    //---------------------------input:
    //input rightwards and leftwards movement
    public virtual void inputHorz(float direction)
    {
        if (!horzAddDone)
        {
            horzAddDone = true;
            vectorToAdd = vectorToAdd + (Vector3.Cross(Vector3.up, calculateForward()) * direction); //add input to a refreshed VectorToAdd each frame

            if (direction != 0 && Mathf.Abs(direction) != 1)
            {
                dontNormalize = true;
            }
            InputDirection = direction;
        }
    }

    //input forwards and backwards movement
    public virtual void inputVert(float direction)
    {
        if (!vertAddDone)
        {
            vertAddDone = true;
            vectorToAdd = vectorToAdd + (calculateForward() * direction); //add input to a refreshed VectorToAdd each frame

            if(direction != 0 && Mathf.Abs(direction) != 1)
            {
                dontNormalize = true;
            }
        }
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

    //attempt to perform fastfall --- helllllla quick and dirty
    public virtual void inputFastFall(int input)
    {
        if(input == 1 )
        {
            Debug.Log("kick");
            charAnim.SetBool("Special_Karate", true);
            rb.velocity = new Vector3(rb.velocity.x, -Mathf.Abs(maxFallSpeed), rb.velocity.z);
        }
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
        //Debug.Log("vta nn "  + vectorToAdd);

        if (!dontNormalize)
        {
            vectorToAdd = vectorToAdd.normalized;
        }

        vectorToAdd *= airAccel; //make sure diagonals aren't overpowered, and apply speed to normalized vector.
        horzAddDone = false;
        vertAddDone = false;
        dontNormalize = false;


        if (rb)
        {
            updateCarJump();

            //apply force to rigidbody
            rb.AddForce(vectorToAdd);

            Vector3 horVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (vectorToAdd == Vector3.zero && horVel.magnitude > maxSpeedThisJump * 0.6f)
            {
                float mag = rb.velocity.magnitude;
                float y = rb.velocity.y;
                rb.velocity = rb.velocity.normalized * (mag - Time.deltaTime * 30f);
                rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
            }

            //constrain to max speed
            updateMaxSpeedCheck();

            updateGravity();

        }

        prevVectorToAdd = vectorToAdd.normalized; //use this for anything outside of the normal update (like starting abilities)
        vectorToAdd = Vector3.zero; //reset at the end of each update
    }

    protected virtual void updateGravity()
    {
        //apply gravity
        if (carJumpTimer <= 0)
        {

            rb.AddForce(gravityDirection * gravityMagnitude);
        }
    }

    //begin initial jump from vehicle
    public virtual void beginCarJump(Vector3 carVelocity, float carMaxSpeed, Vector3 newCarJumpUpDirection, bool carGrounded)
    {
        carJumpUpDirection = newCarJumpUpDirection;
        carLaunchSpeed = Mathf.Max(carVelocity.magnitude, 20f);


        float boostModifier = Mathf.Max((carLaunchSpeed / (carMaxSpeed * boostThreshold)), 1);
        carJumpModifier = 1f + (carLaunchSpeed / carMaxSpeed);
        //Debug.Log(carJumpModifier);

        maxSpeedThisJump = carLaunchSpeed * boostModifier;
        float yVelRetention = carVelocity.y;
        if (!carGrounded)
        {
            yVelRetention = Mathf.Max(0, carVelocity.y);
        }
        rb.velocity = (new Vector3(carVelocity.x, yVelRetention , carVelocity.z).normalized ) * maxSpeedThisJump;
       
        float newY = rb.velocity.y;
        newY += carJumpStartImpulse;
        rb.velocity = new Vector3(rb.velocity.x, newY, rb.velocity.z);
        //rb.AddForce(carJumpUpDirection * Mathf.Max(carJumpStartImpulse, carJumpStartImpulse));

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

        carJumpVelocity = 0;
        carJumpTimer = carJumpTimeSet;
    }

    //hold initial jump from vehicle
    protected virtual void updateCarJump()
    {
        //add full hop to car jump while jump is held
        if (carJumpTimer > 0)
        {
            float newY = rb.velocity.y;
            newY += (carJumpVelocityAdd * carJumpModifier / carJumpTimeSet) * Time.deltaTime;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z) + carJumpUpDirection * newY;
            //rb.AddForce(transform.up * ((carJumpVelocityAdd) / carJumpTimeSet));
            //carJumpVelocity += ((carJumpVelocityAdd) / carJumpTimeSet) * Time.deltaTime; //add full hop normalized to 1 second

            carJumpTimer -= Time.deltaTime;

            //vectorToAdd = vectorToAdd + carJumpUpDirection * carJumpVelocity;
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

        if(rb.velocity.y < -Mathf.Abs(maxFallSpeed))
        {
            rb.velocity = new Vector3(rb.velocity.x, -Mathf.Abs(maxFallSpeed), rb.velocity.z);
        }

        //Debug.Log(horVelocityCheck +"HorzVelcocity");
        //get the speed for animation stuff
        speed = horVelocityCheck.magnitude / 50f;
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
            rdSpawnPoint = transform.position;
            RaycastHit hit;
            int layerMask = 1 << LayerMask.NameToLayer("Road");
            //Debug.DrawLine(transform.position - rb.velocity.normalized, transform.position - rb.velocity.normalized + rb.velocity.normalized * 5f, Color.red, 10f);
            if (Physics.Raycast(transform.position - rb.velocity.normalized, rb.velocity.normalized, out hit, 5f, layerMask))
            {
               // Debug.Log("hit");
                ragdollUp = hit.normal;
                rdSpawnPoint = hit.point;
            }

            spawnRagdoll();
        }

    }

    protected virtual void spawnRagdoll()
    {
        if (currentRagdoll == null)
        {
            currentRagdoll = Instantiate(ragdollPrefab, rdSpawnPoint, transform.rotation);
            currentRagdoll.SetActive(true);
            //Debug.Log(ragdollUp);
            currentRagdoll.GetComponent<RagdollStorage>().rb.velocity = Vector3.Reflect(rb.velocity, ragdollUp) * 5f;  //(rb.velocity * 3f) + (ragdollUp * rb.velocity.magnitude * 4f);
            
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

    //protected virtual void handleLockOnCollision()
    //{

    //    closeVehicles = new List<BasicVehicle>();
    //    //if (rb.velocity.y < 0)
    //    //{
    //    for (int i = 0; i < lockOnCollider.collidersCount(); i++)
    //    {
    //        closeVehicles.Add(lockOnCollider.returnColliders()[i].transform.root.transform.GetComponent<BasicVehicle>());
    //    }
    //    //}

    //    if(targetIconPrefab != null && false)
    //    {
    //        BasicVehicle potentialTargetedVehicle = null;
    //        Destroy(currentTargetIcon);
    //        currentTargetIcon = null;
            
    //        if(vectorToAdd != Vector3.zero)
    //        {
    //            foreach (BasicVehicle bv in closeVehicles)
    //            {
    //                //vectorToAdd = vectorToAdd.normalized;
    //                if (!bv.broken && (rb.velocity.y < 0 || prevVehicle != bv.gameObject))
    //                {
    //                    Vector3 dir = prevVectorToAdd;
    //                    if (cameraLockOn)
    //                    {
    //                        dir = calculateForward();
    //                    }

    //                    float dist = (transform.position + (dir * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - bv.transform.position).magnitude;

    //                    //float dist = DistanceToLine(new Ray (transform.position + vectorToAdd, vectorToAdd), bv.transform.position);
    //                    if (potentialTargetedVehicle == null || dist < (transform.position + (dir * lockOnCollider.transform.GetComponent<SphereCollider>().radius / 2) - potentialTargetedVehicle.transform.position).magnitude)
    //                    {
    //                        potentialTargetedVehicle = bv;
    //                    }
    //                }

                    
    //            }
    //            if (potentialTargetedVehicle != null)
    //            {
    //                currentTargetIcon = Instantiate(targetIconPrefab, potentialTargetedVehicle.transform.position, Quaternion.identity);
    //                //currentTargetIcon.transform.parent = transform;
    //                currentTargetIcon.transform.LookAt(cTransform);
    //            }
    //        }

    //    }
        
    //}

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
        //Debug.Log("Direction" + InputDirection);
        if(InputDirection>0)
        { turn = turn + .1f; }
        if (InputDirection < 0)
        { turn = turn - .1f; }
        if((InputDirection == 0) && turn>0)
        { turn = turn - .1f; }
        if ((InputDirection == 0) && turn < 0)
        { turn = turn + .1f; }
        if(turn>2)
        { turn = 2; }
        if(turn<-2)
        { turn = -2; }
        //Debug.Log(turn);
        charAnim.SetFloat("Turn", turn);
        charAnim.SetFloat("Speed", speed);
        transform.LookAt(transform.position + rb.velocity);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    //---------------------------enter new car:
    //set new max speed for car based on current air speed
    public virtual float calculateNewCarMaxSpeed()
    {
        if (!targetedVehicle)
        {
            Vector3 horVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            return 1.3f * Mathf.Max(20f, (horVel.magnitude));//dont change this yet - nick
            //return 1.5f * Mathf.Max(20f, (horVel.magnitude * 0.5f)) + (horVel.magnitude * 0.7f);//dont change this yet - nick
            //return Mathf.Max(20f, (rb.velocity.magnitude * 1.05f));//dont change this yet - nick
        }
        return storedNewCarMaxSpeed;
    }


    //set new current speed for car based on current air speed
    public virtual float calculateNewCarStartSpeed()
    {
        if (!targetedVehicle)
        {
            Vector3 horVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            return Mathf.Max(20f, (horVel.magnitude));//dont change this yet - nick
            //return (Mathf.Max(20f, (horVel.magnitude * 0.5f)) + (horVel.magnitude * 0.7f));//dont change this yet - nick
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
