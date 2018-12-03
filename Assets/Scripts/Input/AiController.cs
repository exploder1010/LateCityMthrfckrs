using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour {

    public int laneID;
    private LevelBlockInfo curLBI;
    private int curWaypoint;
    private List<Transform> curLBIQueue;

    public bool DebugThis;

    public GameObject pedestrianPrefab;

    private BasicVehicle vehicleScript;
    private Rigidbody rb;
    public bool stopUpdate = false;
    private float forwardInput = 1;
    private bool stopCar;
    private float prevHitDistance = float.MaxValue;
    private List<AxleInfo> axleInfos;
    

    // Use this for initialization
    void Start () {
		vehicleScript = GetComponent<BasicVehicle>();
        rb = GetComponent<Rigidbody>();
        //vehicleScript.initializeSpeed(20,0,false);
        //vehicleScript.AiControlled = true;
        //axleInfos = vehicleScript.axleInfos;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!stopUpdate)
        {
            if( vehicleScript != null && vehicleScript.player)
            {
                stopUpdate = true; //temp solution
            }

            if (vehicleScript != null && !vehicleScript.broken && !vehicleScript.player)
            {
                //Gas and Break Logic
                //float speed = Vector3.Dot(rb.velocity, transform.forward);
                float motor = 0, breakForce = 0;

                float dist = 30f;
                int layerMask = 1 << LayerMask.NameToLayer("Vehicle");
                Vector3 pos = transform.position + transform.up;
                //pos.y += 1;
                RaycastHit hit;

                //if (DebugThis)
                //    Debug.DrawRay(pos, -transform.up * 1000f, Color.red, 0.1f);
                //update waypoint queues
                if (Physics.Raycast(pos, -transform.up, out hit, 1000f))//, 1 << LayerMask.NameToLayer("Road")))
                {
                    //if (DebugThis)
                    //{
                        //Debug.Log(hit.transform);
                    //}
                    if(hit.transform.GetComponent<LevelBlockInfo>() && hit.transform.GetComponent<LevelBlockInfo>() != curLBI)
                    {
                        //Debug.Log("get waypoints");
                        curLBI = hit.transform.GetComponent<LevelBlockInfo>();
                        curWaypoint = 0;
                        if (curLBI.Lanes != null)
                            curLBIQueue = curLBI.Lanes[laneID];
                    }
                    else if(hit.transform.parent != null && hit.transform.parent.transform.GetComponent<LevelBlockInfo>() && hit.transform.parent.transform.GetComponent<LevelBlockInfo>() != curLBI)
                    {
                        curLBI = hit.transform.parent.transform.GetComponent<LevelBlockInfo>();
                        curWaypoint = 0;
                        //Debug.Log("get waypoints " + curLBI);
                        //Debug.Log("get waypoints " + curLBI.Lanes);
                        if (curLBI.Lanes != null)
                            curLBIQueue = curLBI.Lanes[laneID];

                        //curLBIQueue = curLBI.Lane1Waypoints;
                    }
                    else if (hit.transform.parent != null && hit.transform.parent.transform.parent != null && hit.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>() && hit.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>() != curLBI)
                    {
                        curLBI = hit.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>();
                        curWaypoint = 0;
                        if (curLBI.Lanes != null)
                            curLBIQueue = curLBI.Lanes[laneID];
                    }
                    else if (hit.transform.parent != null && hit.transform.parent.transform.parent != null && hit.transform.parent.transform.parent.transform.parent != null && hit.transform.parent.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>() && hit.transform.parent.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>() != curLBI)
                    {
                        curLBI = hit.transform.parent.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>();
                        curWaypoint = 0;
                        if (curLBI.Lanes != null)
                            curLBIQueue = curLBI.Lanes[laneID];
                    }

                }

                
                //if(DebugThis)
                //    Debug.DrawRay(pos, transform.forward * dist, Color.red, 0.1f);
                if (Physics.Raycast(pos, transform.forward, out hit, dist, layerMask))
                {
                    
                    float distance = hit.distance;

                    //if(DebugThis)
                    //    Debug.Log(distance);

                    //Debug.DrawLine(pos, hit.point);
                    if (distance <= 5)
                    {
                        // Mash dat break
                        //if (DebugThis)
                        //    Debug.Log("STOP");
                        breakForce = 10000f;
                        motor = -1f;
                    }
                    else if (prevHitDistance >= distance)
                    {
                        // Getting close to something need to slow down
                        //if (DebugThis)
                        //    Debug.Log("Slowing");
                        breakForce = 1850f;
                        motor = 0f;
                    }
                    else
                    {
                        // Object in front of me is moving I can move

                        //Debug.Log("Accel");
                        breakForce = 0f;
                        motor = 1f;
                    }

                    prevHitDistance = distance;
                }
                else
                {
                    //if (DebugThis) 
                    //    Debug.Log("DRIVE");
                    // Open road just drive
                    motor = 1f;
                    breakForce = 0f;
                }

                //foreach (AxleInfo axle in axleInfos)
                //{
                //    axle.leftWheel.brakeTorque = breakForce;
                //    axle.rightWheel.brakeTorque = breakForce;
                //    if (axle.motor)
                //    {
                //        axle.leftWheel.motorTorque = motor;
                //        axle.rightWheel.motorTorque = motor;
                //    }
                //}

                //input motor movement 
                vehicleScript.inputAccel(motor);

                //lr movement based on waypoints
                Vector3 target = Vector3.zero;
                if (curLBIQueue != null && curLBIQueue.Count > curWaypoint)
                {
                    target = curLBIQueue[curWaypoint].position;
                    Vector3 targetDir = (target - transform.position);


                    //input horizontal movement
                    vehicleScript.inputHorz(AngleDir(transform.forward, targetDir, transform.up));

                    if(DebugThis)
                    Debug.Log(" left or right " + AngleDir(transform.forward, targetDir, transform.up));

                    //update waypoints
                    if ((transform.position - target).magnitude < 4f)
                    {
                        curWaypoint ++;
                    }
                }
                else
                {
                    vehicleScript.inputHorz(0);
                }
            }
        }
	}

    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);
        if (DebugThis)
        {
            Debug.Log(dir);
        }
        return dir * 10;

        //if (dir > 0.0f)
        //{
        //    return 1f;
        //}
        //else if (dir < -0.0f)
        //{
        //    return -1f;
        //}
        //else
        //{
        //    return 0f;
        //}
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
