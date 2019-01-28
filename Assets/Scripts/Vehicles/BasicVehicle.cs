using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicVehicle : MonoBehaviour, IVehicle {

    public HitBox roofRoadHitBox;
    public HitBox vehicleHitBox;
    public GameObject smokeParticleEffect;

    public bool DebugThis;

    public bool player;
    bool prevPlayer;
    bool prevPlayer_2nd;
    bool prevPlayer_3rd;
    public bool broken;
    public bool disabled;

    public float breakInDistance;
    public List<AxleInfo> axleInfos;
    public float MotorTorque = 5000;
    public float MaxSteeringAngle = 45;
    public float SteeringRate = 500;
    public float GroundedStablizationRate = 1000;

    float gravityMagnitude = 20f;
    public Vector3 gravityDirection = Vector3.down;

    public float normalMaxSpeed = 40f;
    public float computerMaxSpeed = 20f;
    float tempMaxSpeed;
    float tempMaxSpeedLowerLimitPercent = 0.85f;
    float tempMaxSpeedGainRate = 0.5f;
    float potentialMaxSpeed;
    //float prevPotentialMaxSpeed;
    float startSpeed;

    private Rigidbody rb;

    float motorInput;
    float steeringInput;
    float vertInput;

    //Brady: New variables for crash.
    public float crashSpeed;

    public bool boosting = false;

    float newSteering;

    //collision
    Vector3 prevVelocity;
    Vector3 prevVelocity_2nd;
    Vector3 prevVelocity_3rd;
    Vector3 crashVelocity;

    // //Brady
     private bool adjusting = false;
     public float AutoCorrect;
    
    private void Start()
    {
        // Needed to keep it from being all wobbly
        // Doing this for one wheel collider does it for them all
        axleInfos[0].leftWheel.GetComponent<WheelCollider>().ConfigureVehicleSubsteps(5, 12, 15);
        potentialMaxSpeed = normalMaxSpeed;
        rb = GetComponent<Rigidbody>();
        if(smokeParticleEffect != null)
            smokeParticleEffect.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!disabled)
        {
            constrainMaxSpeed();
            handleCrashCollision();

            if (!broken && (player || computerMaxSpeed > 0))
            {
                foreach (AxleInfo axle in axleInfos)
                {
                    if (axle.motor)
                    {
                        if (boosting)
                        {
                            axle.leftWheel.motorTorque = (1) * MotorTorque;
                            axle.rightWheel.motorTorque = (1) * MotorTorque;
                        }
                        else
                        {                            
                            axle.leftWheel.motorTorque = (motorInput) * MotorTorque;
                            axle.rightWheel.motorTorque = (motorInput) * MotorTorque;
                        }
                    }
                    if (axle.steering)
                    {

                        if (player)
                        {
                            newSteering = MaxSteeringAngle * steeringInput;
                            newSteering = Mathf.Clamp(newSteering, MaxSteeringAngle * -1, MaxSteeringAngle);
                            axle.leftWheel.steerAngle = newSteering;
                            axle.rightWheel.steerAngle = newSteering;
                        }
                        else
                        {

                            newSteering = steeringInput;
                            axle.leftWheel.steerAngle = newSteering;
                            axle.rightWheel.steerAngle = newSteering;
                        }

                    }
                }


                Stablization();
                TransformWheelMeshes();
                constrainMaxSpeed();

                rb.maxAngularVelocity = 7f;
                if (!CheckWheelsOnGround())
                {
                    rb.maxAngularVelocity = 1f;//this helps 

                    //spinDirection = spinDirection + (Vector3.Cross(Vector3.up, calculateForward()) * steeringInput);
                    //spinDirection = spinDirection + (calculateForward() * motor);

                    Vector3 currentForward = calculateForward();//used to get camera-based right 

                    if (vertInput > 0)
                    {
                        rb.AddTorque(new Vector3(currentForward.z, currentForward.y, -currentForward.x) * 5.5f * 1000f);
                    }
                    else if (vertInput < 0)
                    {

                        rb.AddTorque(-new Vector3(currentForward.z, currentForward.y, -currentForward.x) * 5.5f * 1000f);
                    }
                    if (steeringInput > 0)
                        rb.AddTorque(Vector3.up * 5.5f * 1000f);
                    else if (steeringInput < 0)
                        rb.AddTorque(-Vector3.up * 5.5f * 1000f);
                }

                //constrain speed
                constrainMaxSpeed();

                //Speedometer.ShowSpeed(rb.velocity.magnitude, 0, 100);


            }
            else
            {
                foreach (AxleInfo axle in axleInfos)
                {
                    if (axle.motor)
                    {
                        //axle.leftWheel.motorTorque = 0;
                        //axle.rightWheel.motorTorque = 0;
                    }
                    if (axle.steering)
                    {
                        axle.leftWheel.steerAngle = 0;
                        axle.rightWheel.steerAngle = 0;
                    }
                }
            }


        }
        //apply gravity
        updateGravity();
        //constrain speed
        constrainMaxSpeed();

    }

    //nick
    private void constrainMaxSpeed()
    {
        
        if (rb)
        {
            if (player || transform.GetComponent<AiController>() != null && transform.GetComponent<AiController>().stopUpdate)
            {

                if (rb.velocity.magnitude > tempMaxSpeed)
                {
                    float tempMaxSpeedGain = (potentialMaxSpeed - tempMaxSpeed) * tempMaxSpeedGainRate * Time.deltaTime;
                    tempMaxSpeed = Mathf.Min(tempMaxSpeed + tempMaxSpeedGain, potentialMaxSpeed);
                    rb.velocity = rb.velocity.normalized * tempMaxSpeed;
                    //rb.velocity = rb.velocity.normalized * potentialMaxSpeed;
                }
                else
                {
                    if (rb.velocity.magnitude < tempMaxSpeed)
                    {
                        tempMaxSpeed = Mathf.Max(rb.velocity.magnitude, potentialMaxSpeed * tempMaxSpeedLowerLimitPercent);
                    }
                }
            }
            else
            {
                //Vector3 storeY = rb.velocity;
                rb.velocity = rb.velocity.normalized * Mathf.Min(rb.velocity.magnitude, computerMaxSpeed);
                //rb.velocity = new Vector3(rb.velocity.x, storeY.y, rb.velocity.z);
            }


        }
    }

    public void inputHorz(float direction)
    {
        steeringInput = direction;
    }

    public void inputAccel(float direction)
    {
        motorInput = direction;
    }

    public void inputVert(float direction)
    {
        vertInput = direction;
    }

    //nick 
    public void initializeSpeed(float newMaxSpeed, float newStartSpeed, bool startSMH)
    {
        potentialMaxSpeed = Mathf.Max(newMaxSpeed, normalMaxSpeed);
        tempMaxSpeed = potentialMaxSpeed * tempMaxSpeedLowerLimitPercent;

        startSpeed = newStartSpeed;
        
        if (rb)
        {
            if(easyCheckWheelsOnGround())
                rb.velocity = transform.forward * startSpeed;
        }
       
    }
    
    protected Vector3 calculateForward()
    {
        Transform cTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (GameObject.FindGameObjectWithTag("MainCamera").transform)
        {
            cTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
            Vector3 forward = (this.transform.position - cTransform.position);
            forward.y = 0;
            forward = forward.normalized;

            return forward;
        }
        //Debug.Log("FAILURE");
        return Vector3.zero;
    }

    void updateGravity()
    {

        RaycastHit hit;
        Vector3 pos = transform.position + transform.up;
        float hitmod = 1;

        //if (DebugThis)
        //{
        //    Debug.DrawRay(pos, -transform.up * 10f, Color.red, 5f);
        //}

        if ((easyCheckWheelsOnGround() || !player)&& Physics.Raycast(pos, -transform.up, out hit, 10f, 1 << LayerMask.NameToLayer("Road")) && hit.transform.GetComponent<GravityRoad>())
        {
            //if (DebugThis)
            //{
            //    Debug.Log("grav");
            //}
            gravityMagnitude = hit.transform.GetComponent<GravityRoad>().gravity;
            gravityDirection = -hit.normal;
            Vector3.RotateTowards(transform.up, hit.normal, 100f * Time.deltaTime, 100f * Time.deltaTime);
            //Quaternion.RotateTowards(transform.rotation, transform.rotation);
            //transform.up = hit.normal;
            hitmod = 3f;
        }
        else if (!easyCheckWheelsOnGround())
        {
            bool hoobool = Physics.Raycast(pos, -transform.up, out hit, 10f, 1 << LayerMask.NameToLayer("Road")) && hit.transform.GetComponent<GravityRoad>();
            //if (DebugThis)
            //{
            //    Debug.Log("not grav: ( " + easyCheckWheelsOnGround() + " or " + !player + " ) and " + hoobool);
            //}
            gravityMagnitude = 20f;
            gravityDirection = Vector3.down;
        }

        Vector3 force = gravityDirection * gravityMagnitude * hitmod;
        rb.AddForce(force, ForceMode.Acceleration);
        


    }

    void Stablization()
    {
        if (CheckWheelsOnGround())
        {
            Vector3 force = rb.velocity.magnitude * GroundedStablizationRate * -1 * transform.up;
            rb.AddForce(force);
        }
    }

    bool CheckWheelsOnGround()
    {
        bool WheelsOnGround = true;
        foreach (AxleInfo axle in axleInfos)
        {
            if (!(axle.leftWheel.isGrounded && axle.rightWheel.isGrounded))
            {
                WheelsOnGround = false;
                return WheelsOnGround;
            }
        }
        WheelsOnGround = true;
        return WheelsOnGround;

    }

    public bool easyCheckWheelsOnGround()
    {
        bool wheelsOnGround = false;
        foreach (AxleInfo axle in axleInfos)
        {
            if ((axle.leftWheel.isGrounded || axle.rightWheel.isGrounded))
            {
                wheelsOnGround = true;
                break;
            }
        }
        return wheelsOnGround;

    }

    public Vector3 getGravity()
    {
        return gravityDirection;
    }


    void TransformWheelMeshes()
    {
        foreach(AxleInfo axle in axleInfos)
        {
            Transform l_mesh = axle.leftWheel.transform.GetChild(0);
            Transform r_mesh = axle.rightWheel.transform.GetChild(0);
            
            Vector3 loc = new Vector3();
            Quaternion rot = new Quaternion();

            axle.leftWheel.GetWorldPose(out loc, out rot);
            l_mesh.SetPositionAndRotation(loc, rot);

            axle.rightWheel.GetWorldPose(out loc, out rot);
            r_mesh.SetPositionAndRotation(loc, rot);
        }
    }

    protected virtual void handleCrashCollision()
    {
        bool vcheck1 = ((prevVelocity.magnitude - rb.velocity.magnitude) > crashSpeed && prevVelocity.magnitude > rb.velocity.magnitude);
        bool vcheck2 = ((prevVelocity_2nd.magnitude - rb.velocity.magnitude) > crashSpeed && prevVelocity_2nd.magnitude > rb.velocity.magnitude);
        bool vcheck3 = ((prevVelocity_3rd.magnitude - rb.velocity.magnitude) > crashSpeed && prevVelocity_3rd.magnitude > rb.velocity.magnitude);

        if (!broken && prevPlayer == player == prevPlayer_2nd == prevPlayer_3rd && ((roofRoadHitBox != null && roofRoadHitBox.collidersCount() > 0) || vcheck1 || vcheck2 || vcheck3 ))
        {

                //Debug.Log("Major Crash on Late City Highway");
                broken = true;

                if (smokeParticleEffect != null)
                    smokeParticleEffect.SetActive(true);

                if (vcheck1)
                {
                    //Debug.Log("check1");
                    crashVelocity = prevVelocity;
                }
                if (vcheck2)
                {
                    //Debug.Log("check2");
                    crashVelocity = prevVelocity_2nd;
                }
                if (vcheck3)
                {
                    //Debug.Log("check3");
                    crashVelocity = prevVelocity_3rd;
                }


                if (vehicleHitBox.collidersCount() > 0)
                {
                    //Debug.Log("vehic");
                    foreach (Collider other in vehicleHitBox.returnColliders())
                    {

                        //Debug.Log("the vehic" + other.transform.root);
                        other.transform.root.transform.GetComponent<BasicVehicle>().broken = true;
                    if (other.transform.root.transform.GetComponent<BasicVehicle>().smokeParticleEffect != null)
                        other.transform.root.transform.GetComponent<BasicVehicle>().smokeParticleEffect.SetActive(true);

                    Vector3 dir = (other.transform.root.transform.position - transform.position).normalized;

                        //Debug.DrawRay(transform.position, dir * 10000f + transform.up * 10000f, Color.blue, 10f);

                        rb.AddForce((dir * 10000f + transform.up * 10000f), ForceMode.Impulse);
                        other.transform.root.transform.GetComponent<Rigidbody>().AddForce((-dir * 10000f + other.transform.root.transform.up * 10000f), ForceMode.Impulse);

                    }
                }
            
            

          
        }
        //prevPotentialMaxSpeed = potentialMaxSpeed;
        prevPlayer_3rd = prevPlayer_2nd;
        prevPlayer_2nd = prevPlayer;
        prevPlayer = player;

        prevVelocity_3rd = prevVelocity_2nd;
        prevVelocity_2nd = prevVelocity;
        prevVelocity = rb.velocity;
    }

    public Vector3 returnExitVelocity()
    {
        if(crashVelocity.magnitude != 0)
        {
            return crashVelocity;
        }
        return rb.velocity;
    }

    public float returnActualMaxSpeed()
    {
        return potentialMaxSpeed;
    }

    
}




[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}


    

