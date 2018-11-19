using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour {

    public GameObject pedestrianPrefab;

    private BasicVehicle vehicleScript;
    private Rigidbody rb;
    private bool stopUpdate = false;
    private float forwardInput = 1;
    private bool stopCar;
    private float prevHitDistance = float.MaxValue;
    private List<AxleInfo> axleInfos;

    // Use this for initialization
    void Start () {
		vehicleScript = GetComponent<BasicVehicle>();
        rb = GetComponent<Rigidbody>();
		vehicleScript.initializeSpeed(20,0,false);
        //vehicleScript.AiControlled = true;
        axleInfos = vehicleScript.axleInfos;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!stopUpdate)
        {
            if (vehicleScript != null && !vehicleScript.broken)
            {
                //Gas and Break Logic
                float speed = Vector3.Dot(rb.velocity, transform.forward);
                float motor=0, breakForce=0;

                Vector3 pos = transform.position;
                pos.y += 1;
                RaycastHit hit;
                if (Physics.Raycast(pos, transform.forward, out hit, 50f))
                {
                    float distance = hit.distance;
                    //Debug.DrawLine(pos, hit.point);
                    if (distance <= 5f)
                    {
                        // Mash dat break

                        //Debug.Log("STOP");
                        breakForce = 10000f;
                        motor = 0f;
                    }
                    else if (prevHitDistance >= distance)
                    {
                        // Getting close to something need to slow down

                        //Debug.Log("Slowing");
                        breakForce = 1850f;
                        motor = 0f;
                    }
                    else
                    {
                        // Object in front of me is moving I can move

                        //Debug.Log("Accel");
                        breakForce = 0f;
                        motor = 1000f;
                    }

                    prevHitDistance = distance;
                }
                else
                {
                    // Open road just drive

                    motor = 2500f;
                    breakForce = 0f;
                }

                foreach (AxleInfo axle in axleInfos)
                {
                    axle.leftWheel.brakeTorque = breakForce;
                    axle.rightWheel.brakeTorque = breakForce;
                    if (axle.motor)
                    {
                        axle.leftWheel.motorTorque = motor;
                        axle.rightWheel.motorTorque = motor;
                    }
                }


                    //input horizontal movement
                    vehicleScript.inputHorz(0f);
            }
        }
	}

    void Eject()
    {
        stopUpdate = true;
        StartCoroutine(EjectDriver());
    }

    private IEnumerator EjectDriver()
    {
        GameObject person = GameObject.Instantiate(pedestrianPrefab);
        person.transform.position = transform.position;
        person.GetComponent<Rigidbody>().AddForce(5f * transform.right);
        yield return new WaitForSeconds(2f);
        Destroy(this);
    }
    private IEnumerator TryToStop()
    {
        while (stopCar && Vector3.Dot(rb.velocity, transform.forward) > 0)
        {
            Debug.Log(Vector3.Dot(rb.velocity, transform.forward));
            //input decelleration
            vehicleScript.inputAccel(-5f);
            yield return null;
        }
        vehicleScript.inputAccel(0);
        rb.velocity = Vector3.zero;
        yield return null;
    }
}
