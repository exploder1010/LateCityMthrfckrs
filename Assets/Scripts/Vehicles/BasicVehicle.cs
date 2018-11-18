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

    float newSteering;

    //nick: dunkey's trademark spin move
    bool spinMoveHop;
    float curSpinJump;
    bool spinMove;
    float spinLeft;
    int spinDir;
    Vector3 spinDirection;
    //float spinMoveHopTimeLimit;
    bool spinMoveHopGrounded;

    //collision
    Vector3 prevVelocity;

    // //Brady
    // private bool adjusting = false;
    // public float AutoCorrect;
    
    private void Start()
    {
        // Needed to keep it from being all wobbly
        // Doing this for one wheel collider does it for them all
        axleInfos[0].leftWheel.GetComponent<WheelCollider>().ConfigureVehicleSubsteps(5, 12, 15);
        actualMaxSpeed = normalMaxSpeed;
        rb = GetComponent<Rigidbody>();
        
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
                    axle.leftWheel.motorTorque = motor - (motor * 0.65f * (Mathf.Abs(rb.velocity.magnitude) /actualMaxSpeed));
                    axle.rightWheel.motorTorque = motor - (motor * 0.65f * (Mathf.Abs(rb.velocity.magnitude) / actualMaxSpeed));

                    axle.leftWheel.motorTorque += Mathf.Abs(newSteering) / MaxSteeringAngle * (motor - (Mathf.Abs(rb.velocity.magnitude) / actualMaxSpeed));
                    axle.rightWheel.motorTorque += Mathf.Abs(newSteering) / MaxSteeringAngle * (motor - (Mathf.Abs(rb.velocity.magnitude) / actualMaxSpeed));
                }
                if (axle.steering)
                {
                     newSteering = Mathf.MoveTowards(axle.leftWheel.steerAngle, MaxSteeringAngle * steeringInput, SteeringRate * Time.deltaTime);
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

            if(!easyCheckWheelsOnGround() && !isSpinMoveHop())
            {
                if (motor > 0)
                {
                    rb.AddTorque(transform.right * 2.5f * 1000f);
                }
                else if (motor < 0)
                {

                    rb.AddTorque(-transform.right * 2.5f * 1000f);
                }
                if (steeringInput > 0)
                    rb.AddTorque(transform.up * 2.5f * 1000f);
                else if (steeringInput < 0)
                    rb.AddTorque(-transform.up * 2.5f * 1000f);

                          //Brady: Possible autocorrection for the forward axis.
                //    //Alternatively, add slow down in opposite direction, like friction... Currently this is abrupt and unrealistic, but just to see if the approach is viable.
                //    //Might be trying to fix a non existant issue.
                //if(rb.rotation.z > AutoCorrect)
                //{
                //    adjusting = true;
                //    rb.AddTorque(transform.forward * 300f);
                //}
                //else if (rb.rotation.z < -AutoCorrect)
                //{
                //    adjusting = true;
                //    rb.AddTorque(-transform.forward * 300f);
                //}
                //else if (adjusting == true && (rb.rotation.z < 0.1f || rb.rotation.z > -0.1f))
                //{
                //    adjusting = false;
                //    rb.angularVelocity = new Vector3(rb.rotation.x, rb.rotation.y, 0);
                //}
            }

            if (spinMoveHop)
            {
                if (spinMove)
                {
                    if (spinMove && spinLeft > 0)
                    {
                        float newSpin = Mathf.Min(2000f * Time.deltaTime, spinLeft);
                        spinLeft -= newSpin;
                        transform.RotateAround(transform.position, transform.up, newSpin * spinDir);
                    }
                }



                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                curSpinJump -= Time.deltaTime * 30f;

                if (spinMoveHopGrounded)
                {
                    rb.angularVelocity = Vector3.zero;
                    curSpinJump = Mathf.Max(curSpinJump, -100f);
                    transform.position += transform.up * curSpinJump * Time.deltaTime;
                }
                else
                {
                    rb.angularVelocity = Vector3.zero;
                    curSpinJump = Mathf.Max(curSpinJump, -100f);
                    transform.position += transform.up * curSpinJump * Time.deltaTime;
                }
                
                //rb.AddForce(transform.up * curSpinJump, ForceMode.Force);

                //spinMoveHopTimeLimit -= Time.deltaTime;

                RaycastHit hit;
                int layerMask = 1 << LayerMask.NameToLayer("Road");
                if (easyCheckWheelsOnGround() && curSpinJump < 0 ||  !Physics.Raycast(transform.position, -transform.up, out hit, layerMask))
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

        //Debug.Log("new max speed, start speed: " + actualMaxSpeed + " " + startSpeed);
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

            spinMove = true;
            spinDirection = Vector3.zero;
            spinDirection = spinDirection + (Vector3.Cross(Vector3.up, calculateForward()) * directionToSpin.x);
            spinDirection = spinDirection + (calculateForward() * directionToSpin.z);

            float firstAngle = Vector3.SignedAngle(transform.forward, spinDirection, transform.up);
            if (firstAngle != 0)
            {
                spinDir = (int)(Mathf.Abs(firstAngle) / firstAngle);
            }
            spinLeft = Mathf.Abs(firstAngle + (spinDir * 360f * 1f));
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

    //nick
    public virtual bool isSpinMoveHop()
    {
        return spinMoveHop;
    }

    //nick
    protected virtual void startSpinMoveHop()
    {

        //spinMoveHopTimeLimit = 3f;
        curSpinJump = 10f;
        spinMoveHop = true;

        spinMoveHopGrounded = false;
        if (easyCheckWheelsOnGround())
        {
            spinMoveHopGrounded = true;
        }
        else
        {
            float eulerY = transform.eulerAngles.y;
            Vector3 roadUp = Vector3.up;
            RaycastHit hit;
            int layerMask = 1 << LayerMask.NameToLayer("Road");
            if (Physics.Raycast(transform.position, -transform.up, out hit, layerMask))
            {
                roadUp = hit.normal;
            }
            transform.up = roadUp;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
            rb.angularVelocity = Vector3.zero;
        }

        

       
        //transform.rotation.SetLookRotation(transform.forward, roadUp);
    }

    //nick
    protected virtual void endSpinMoveHop()
    {
        spinMoveHop = false;
        spinMove = false;
        //if(//spinMoveHopTimeLimit > 0)
        //{

            rb.velocity = transform.forward * startSpeed;
        //}
        //spinMoveHopTimeLimit = 0;
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
        if (motor != 0)
        {

        }
        if (roofRoadHitBox != null && (roofRoadHitBox.collidersCount() > 0 || vehicleHitBox.collidersCount() > 0 || ((prevVelocity.magnitude - rb.velocity.magnitude) > crashSpeed && prevVelocity.magnitude > rb.velocity.magnitude)))
        {
            //Debug.Log("Major Crash on Late City Highway");
            broken = true;

            if (vehicleHitBox.collidersCount() > 0)
            {
                //Debug.Log("vehic");
                foreach(Collider other in vehicleHitBox.returnColliders())
                {

                        //Debug.Log("the vehic" + other.transform.root);
                        other.transform.root.transform.GetComponent<BasicVehicle>().broken = true;

                        Vector3 dir = (other.transform.root.transform.position - transform.position).normalized;

                        //Debug.DrawRay(transform.position, dir * 10000f + transform.up * 10000f, Color.blue, 10f);

                        rb.AddForce((dir * 10000f + transform.up * 10000f), ForceMode.Impulse);
                        other.transform.root.transform.GetComponent<Rigidbody>().AddForce((-dir * 10000f + other.transform.root.transform.up * 10000f), ForceMode.Impulse);

                }
            }
        }
    }

    public Vector3 returnExitVelocity()
    {
        return rb.velocity;
    }

    public float returnActualMaxSpeed()
    {
        return actualMaxSpeed;
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


    

