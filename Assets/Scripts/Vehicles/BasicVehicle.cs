using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicVehicle : MonoBehaviour, IVehicle {

    public HitBox roofRoadHitBox;
    public HitBox vehicleHitBox;

    public float breakInDistance;
    public List<AxleInfo> axleInfos;
    public float MotorTorque = 5000;
    public float MaxSteeringAngle = 45;
    public float SteeringRate = 500;
    public float GroundedStablizationRate = 1000;

    public float normalMaxSpeed = 40f;
    float actualMaxSpeed;
    float startSpeed;

    private Rigidbody rb;

    float motor;
    float steeringInput;

    //Brady: New variables for crash.
    public float crashSpeed;
    public bool broken;

    //nick: dunkey's trademark spin move
    bool spinMoveHop;
    float curSpinJump;
    bool spinMove;
    float spinLeft;
    int spinDir;
    Vector3 spinDirection;

    //collision
    Vector3 prevVelocity;

    //Brady
    public bool WheelsOnGround;

    private void Start()
    {
        // Needed to keep it from being all wobbly
        // Doing this for one wheel collider does it for them all
        axleInfos[0].leftWheel.GetComponent<WheelCollider>().ConfigureVehicleSubsteps(5, 12, 15);
 
        rb = GetComponent<Rigidbody>();

        WheelsOnGround = true;
    }

    private void FixedUpdate()
    {

        if (!broken)
        {
            handleCrashCollision();
            foreach (AxleInfo axle in axleInfos)
            {
                if (axle.motor)
                {
                    axle.leftWheel.motorTorque = motor;
                    axle.rightWheel.motorTorque = motor;
                }
                if (axle.steering)
                {
                    float newSteering = Mathf.MoveTowards(axle.leftWheel.steerAngle, MaxSteeringAngle * steeringInput, SteeringRate * Time.deltaTime);
                    axle.leftWheel.steerAngle = newSteering;
                    axle.rightWheel.steerAngle = newSteering;
                }
            }

            if (!spinMoveHop)
            {

                Stablization();
            }

            TransformWheelMeshes();
            constrainMaxSpeed();

            if (spinMoveHop)
            {
                if (spinMove)
                {
                    if (spinMove && spinLeft > 0)
                    {
                        float newSpin = spinDir * 2000f * Time.deltaTime;
                        transform.RotateAround(transform.position, transform.up, newSpin);
                        spinLeft -= newSpin;
                    }
                }
                else
                {

                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                }

                rb.velocity = new Vector3(rb.velocity.x, curSpinJump, rb.velocity.z);
                curSpinJump -= Time.deltaTime * 30f;

                if (CheckWheelsOnGround() && rb.velocity.y < 0)
                {
                    endSpinMoveHop();
                }
            }

            Speedometer.ShowSpeed(rb.velocity.magnitude, 0, 100);
            prevVelocity = rb.velocity;
        }

    }

    public void inputHorz(float direction)
    {
        steeringInput = direction;
    }

    public void inputAccel(float direction)
    {
        motor = MotorTorque * direction;
    }

    //nick 
    public void initializeSpeed(float newMaxSpeed, float newStartSpeed, bool startSMH)
    {
        actualMaxSpeed = Mathf.Max(newMaxSpeed, normalMaxSpeed);

        startSpeed = newStartSpeed;

        Debug.Log("new max speed, start speed: " + actualMaxSpeed + " " + startSpeed);
        if (rb)
        {
            if (startSMH)
            {
                startSpinMoveHop();
            }
            else
            {
                rb.velocity = transform.forward * startSpeed;
            }
        }
       
    }

    //nick
    public virtual void startSpinMove(Vector3 directionToSpin)
    {
        
        //if (!spinMove)
        //{
            Debug.Log("spinmoe =" + calculateForward());
            spinMove = true;
            spinDirection = Vector3.zero;
            spinDirection = spinDirection + (Vector3.Cross(Vector3.up, calculateForward()) * directionToSpin.x);
            spinDirection = spinDirection + (calculateForward() * directionToSpin.z);

            float firstAngle = Vector3.SignedAngle(transform.forward, spinDirection, transform.up);
            if (firstAngle != 0)
            {
                spinDir = (int)(Mathf.Abs(firstAngle) / firstAngle);
            }
            firstAngle += spinDir * 360f;
            spinLeft = Mathf.Abs(firstAngle);

            //directionToSpin = Vector3.Cross(Vector3.up, calculateForward());
            //transform.LookAt(transform.position + actualDirection);
        //}
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
            //Debug.Log("forward " + forward);
            Debug.Log("SUCCESS");
            return forward;
        }
        Debug.Log("FAILURE");
        return Vector3.zero;
    }

    //nick
    public virtual bool isSpinMoveHop()
    {
        return spinMoveHop;
    }

    //nick
    protected virtual void startSpinMoveHop()
    {
        curSpinJump = 10f;
        spinMoveHop = true;
        //rb.velocity = transform.forward * startSpeed;
        //rb.velocity = transform.up * 10f;
        //rb.AddForce(transform.up * 1000f, ForceMode.Impulse);
    }

    //nick
    protected virtual void endSpinMoveHop()
    {
        spinMoveHop = false;
        if (spinMove)
        {
            transform.LookAt(transform.position + spinDirection);
        }
        spinMove = false;
        rb.velocity = transform.forward * startSpeed;
    }

    //nick
    private void constrainMaxSpeed()
    {
        if (rb)
        {
            if(rb.velocity.magnitude > actualMaxSpeed)
            {
                rb.velocity = rb.velocity.normalized * actualMaxSpeed;
            }
            
        }
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

    //    //Brady: Collision with anything but the ground activates a crash. Main purpose is for cars and other objects that don't slow player down when rb.velocity.magnitude is checked.
    //    //Should this be protected/virtual when more vehicles are added?
    //    //Should we use enter or stay. Will scraping against walls be counted as possible crashed?
    //    void OnCollisionEnter(Collision other)
    //    {
    //        if (LayerMask.LayerToName(other.gameObject.layer) != "Road")
    //        {
    //            //Debug.Log("Crash Collision = " + rb.velocity.magnitude);
    //            if (rb.velocity.magnitude > crashSpeed)
    //            {
    //                //High impact crash. Play crash sound and set broken to true.
    //                //When car checks if broken is true or false, result will cause a crash from within PlayerController.
    //                //Debug.Log("Major Crash on Late City Highway");
    //                broken = true;
    //            }
    //            else
    //            {
    //                //Low impact crash. Play scrape sound.
    //                //broken remains false, so checking from PlayerController will cause no difference.
    //                //Debug.Log("Minor Fender Bender");
    //            }
    //        }
    //    }

    //    //Brady: One problem faced is, when colliding the car with an immovable wall, the wall slows the player down to the point that puts its rb.velocity.magnitude below the crash speed.
    //    //Adding a trigger box collider in place of the original box collider, and making the original box collider smaller, should allow for proper crashes at high speeds. May cause clipping, and would
    //    //need to be done to every object. OnTriggerEnter behaves in similar manner to OnCollisionEnter.
    //    void OnTriggerEnter(Collider other)
    //    {
    //        if (LayerMask.LayerToName(other.gameObject.layer) != "Road" && LayerMask.LayerToName(other.gameObject.layer) != "Rider")
    //        {
    //            //Debug.Log(other.gameObject.name + " Crash Trigger = " + rb.velocity.magnitude);
    ////            if (rb.velocity.magnitude > crashSpeed)
    ////            {
    ////                //Debug.Log("Major Crash on Late City Highway");
    ////                broken = true;
    ////            }
    ////            else
    ////            {
    ////                //Debug.Log("Minor Fender Bender");
    ////            }
    //        }
    //    }

    protected virtual void handleCrashCollision()
    {
        if (motor != 0)
        {
            //roofRoadHitBox.collidersCount() > 0 || roadHitBox.collidersCount() > 0 || 
            Debug.Log("MPH: " + rb.velocity.magnitude + " prev " + prevVelocity.magnitude);
        }
        if (roofRoadHitBox != null && (roofRoadHitBox.collidersCount() > 0 || vehicleHitBox.collidersCount() > 0 || ((prevVelocity.magnitude - rb.velocity.magnitude) > crashSpeed && prevVelocity.magnitude > rb.velocity.magnitude)))
        {
            //Debug.Log("Major Crash on Late City Highway");
            broken = true;

            if (vehicleHitBox.collidersCount() > 0)
            {
                Debug.Log("vehic");
                foreach(Collider other in vehicleHitBox.returnColliders())
                {
                    Debug.Log("the vehic" + other.transform.root);
                    other.transform.root.transform.GetComponent<BasicVehicle>().broken = true;

                    Vector3 dir = (other.transform.root.transform.position - transform.position).normalized;

                    Debug.DrawRay(transform.position, dir * 10000f + transform.up * 10000f, Color.blue, 10f);

                    rb.AddForce((dir * 10000f + transform.up * 10000f), ForceMode.Impulse);
                    other.transform.root.transform.GetComponent<Rigidbody>().AddForce((-dir * 10000f + other.transform.root.transform.up * 10000f) , ForceMode.Impulse);
                }
            }
        }
    }

    public Vector3 returnExitVelocity()
    {
        return rb.velocity;
    }

    public float returnExitMaxSpeed()
    {
        return actualMaxSpeed;
    }


    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.transform.GetComponent<BasicVehicle>())
    //    {
    //        //Debug.Log("Crash Collision = " + rb.velocity.magnitude);
    //        if (rb.velocity.magnitude > crashSpeed)
    //        {
    //            //High impact crash. Play crash sound and set broken to true.
    //            //When car checks if broken is true or false, result will cause a crash from within PlayerController.
    //            //Debug.Log("Major Crash on Late City Highway");
    //            other.transform.root.transform.GetComponent<BasicVehicle>().broken = true;
    //            broken = true;
    //        }
    //        else
    //        {

    //        }
    //    }
    //}
}




[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}


    

