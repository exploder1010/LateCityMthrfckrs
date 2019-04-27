using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour {

    public bool disabled;

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

    float refreshTimer;
    float refreshTimeSet = 1f;
    float refreshTimer2;
    float refreshTimeSet2 = 0.3f;

    float motor = 0, breakForce = 0;

    float dist = 30f;
    int layerMask;// = 1 << LayerMask.NameToLayer("Vehicle");

    // Use this for initialization
    void Start () {
		vehicleScript = GetComponent<BasicVehicle>();
        rb = GetComponent<Rigidbody>();
        //vehicleScript.initializeSpeed(20,0,false);
        //vehicleScript.AiControlled = true;
        //axleInfos = vehicleScript.axleInfos;
        int layerMask = 1 << LayerMask.NameToLayer("Vehicle");
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!stopUpdate && !disabled)
        {
            refreshTimer -= Time.fixedDeltaTime;
            refreshTimer2 -= Time.fixedDeltaTime;
            if ( vehicleScript != null)
            {

                if (vehicleScript.player)
                {
                    stopUpdate = true; //temp solution
                }
                else if (!vehicleScript.broken)
                {
                    {
                        
                        Vector3 pos = transform.position + transform.up;
                        
                        RaycastHit hit;

                        if (refreshTimer <= 0 && Physics.Raycast(pos, -transform.up, out hit, 1000f))//, 1 << LayerMask.NameToLayer("Road")))
                        {
                            
                            if (hit.transform.GetComponent<LevelBlockInfo>() && hit.transform.GetComponent<LevelBlockInfo>() != curLBI)
                            {
                                curLBI = hit.transform.GetComponent<LevelBlockInfo>();
                                curWaypoint = 0;
                                if (curLBI.Lanes != null)
                                    curLBIQueue = curLBI.Lanes[laneID];
                            }
                            else if (hit.transform.parent != null && hit.transform.parent.transform.GetComponent<LevelBlockInfo>() && hit.transform.parent.transform.GetComponent<LevelBlockInfo>() != curLBI)
                            {
                                curLBI = hit.transform.parent.transform.GetComponent<LevelBlockInfo>();
                                curWaypoint = 0;
                                if (curLBI.Lanes != null)
                                    curLBIQueue = curLBI.Lanes[laneID];
                                
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

                        
                        if (refreshTimer2 <= 0 && Physics.Raycast(pos, transform.forward, out hit, dist, layerMask))
                        {

                            float distance = hit.distance;
                            
                            if (distance <= 5)
                            {
                                // Mash dat break
                                breakForce = 10000f;
                                motor = -1f;
                            }
                            else if (prevHitDistance >= distance)
                            {
                                // Getting close to something need to slow down
                                breakForce = 1850f;
                                motor = 0f;
                            }
                            else
                            {
                                // Object in front of me is moving I can move
                                breakForce = 0f;
                                motor = 1f;
                            }

                            prevHitDistance = distance;
                        }
                        else
                        {
                            // Open road just drive
                            motor = 1f;
                            breakForce = 0f;
                        }

                        //input motor movement 
                        vehicleScript.inputAccel(motor);

                        //lr movement based on waypoints
                        Vector3 target = Vector3.zero;
                        if (curLBIQueue != null && curLBIQueue.Count > curWaypoint && curLBIQueue[curWaypoint] != null)
                        {
                            target = curLBIQueue[curWaypoint].position;
                            Vector3 targetDir = (target - transform.position);


                            //input horizontal movement
                            vehicleScript.inputHorz(AngleDir(transform.forward, targetDir, transform.up));

                            if (DebugThis)
                                Debug.Log(" left or right " + AngleDir(transform.forward, targetDir, transform.up));

                            //update waypoints
                            if ((transform.position - target).magnitude < 4f)
                            {
                                curWaypoint++;
                            }
                        }
                        else
                        {
                            vehicleScript.inputHorz(0);
                        }

                        if (refreshTimer <= 0)
                        {
                            refreshTimer = refreshTimeSet;
                        }
                        if (refreshTimer2 <= 0)
                        {
                            refreshTimer2 = refreshTimeSet;
                        }
                    }
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
