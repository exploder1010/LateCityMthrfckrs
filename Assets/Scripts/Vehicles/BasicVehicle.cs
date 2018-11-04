using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicVehicle : MonoBehaviour, IVehicle {

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

    private void Start()
    {
        // Needed to keep it from being all wobbly
        // Doing this for one wheel collider does it for them all
        axleInfos[0].leftWheel.GetComponent<WheelCollider>().ConfigureVehicleSubsteps(5, 12, 15);
 
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

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
        Stablization();
        TransformWheelMeshes();
        constrainMaxSpeed();
        Debug.Log(rb.velocity.magnitude);
        //Speedometer.ShowSpeed(rb.velocity.magnitude, 0, 100); -- todo: add marissa's script
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
    public void initializeSpeed(float newMaxSpeed, float newStartSpeed)
    {
        actualMaxSpeed = Mathf.Max(newMaxSpeed, normalMaxSpeed);

        startSpeed = newStartSpeed;

        if (rb)
        {

            rb.velocity = transform.forward * startSpeed;
        }
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
        bool wheelsOnGround = true;
        foreach (AxleInfo axle in axleInfos)
        {
            if (!(axle.leftWheel.isGrounded && axle.rightWheel.isGrounded))
            {
                wheelsOnGround = false;
                break;
            }
        }

        if (wheelsOnGround)
        {
            Vector3 force = rb.velocity.magnitude * GroundedStablizationRate * -1 * transform.up;
            rb.AddForce(force);
        }
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

    //Brady: Collision with anything but the ground activates a crash. Main purpose is for cars and other objects that don't slow player down when rb.velocity.magnitude is checked.
    //Should this be protected/virtual when more vehicles are added?
    //Should we use enter or stay. Will scraping against walls be counted as possible crashed?
    void OnCollisionEnter(Collision other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) != "Road")
        {
            //Debug.Log("Crash Collision = " + rb.velocity.magnitude);
            if (rb.velocity.magnitude > crashSpeed)
            {
                //High impact crash. Play crash sound and set broken to true.
                //When car checks if broken is true or false, result will cause a crash from within PlayerController.
                //Debug.Log("Major Crash on Late City Highway");
                broken = true;
            }
            else
            {
                //Low impact crash. Play scrape sound.
                //broken remains false, so checking from PlayerController will cause no difference.
                //Debug.Log("Minor Fender Bender");
            }
        }
    }

    //Brady: One problem faced is, when colliding the car with an immovable wall, the wall slows the player down to the point that puts its rb.velocity.magnitude below the crash speed.
    //Adding a trigger box collider in place of the original box collider, and making the original box collider smaller, should allow for proper crashes at high speeds. May cause clipping, and would
    //need to be done to every object. OnTriggerEnter behaves in similar manner to OnCollisionEnter.
    void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) != "Road" && LayerMask.LayerToName(other.gameObject.layer) != "Rider")
        {
            //Debug.Log(other.gameObject.name + " Crash Trigger = " + rb.velocity.magnitude);
            if (rb.velocity.magnitude > crashSpeed)
            {
                //Debug.Log("Major Crash on Late City Highway");
                broken = true;
            }
            else
            {
                //Debug.Log("Minor Fender Bender");
            }
        }
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


    

