using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    

    private Transform focus;
    
    private Vector3 targetPosition;
    private Vector3 curOffset;

    private Vector3 targetEulerAngles;

    private float followSpeed;
    private float trackSpeed;

    private float deathZoomMax = 3f;
    private float deathZoomMin = 12f;
    private float deathZoomFollow = 18f;
    private float deathZoomTrack = 8f;
    private float deathZoom;

    private Vector3 prevFocusPos;

    private enum CameraState { Vehicle = 0, Rider, Dead,  };
    private CameraState curState;

    private LevelBlockInfo curLBI;
    private bool firstLBI = true;

    // Use this for initialization
    void Start () {
        //forward = new Quaternion(0, 0, 0, 0);
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (focus != null && Time.timeScale == 1)
        {

            float dist = 100f;
            int layerMask = 1 << LayerMask.NameToLayer("Vehicle");
            Vector3 pos = focus.position + transform.up;
            RaycastHit hit;


            //update waypoint queues
            if (Physics.Raycast(pos, -focus.up, out hit, 1000f))//, 1 << LayerMask.NameToLayer("Road")))
            {
                if (hit.transform.GetComponent<LevelBlockInfo>() && hit.transform.GetComponent<LevelBlockInfo>() != curLBI)
                {
                    curLBI = hit.transform.GetComponent<LevelBlockInfo>();
                }
                else if (hit.transform.parent != null && hit.transform.parent.transform.GetComponent<LevelBlockInfo>() && hit.transform.parent.transform.GetComponent<LevelBlockInfo>() != curLBI)
                {
                    curLBI = hit.transform.parent.transform.GetComponent<LevelBlockInfo>();

                }
                else if (hit.transform.parent != null && hit.transform.parent.transform.parent != null && hit.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>() && hit.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>() != curLBI)
                {
                    curLBI = hit.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>();
                }
                else if (hit.transform.parent != null && hit.transform.parent.transform.parent != null && hit.transform.parent.transform.parent.transform.parent != null && hit.transform.parent.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>() && hit.transform.parent.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>() != curLBI)
                {
                    curLBI = hit.transform.parent.transform.parent.transform.parent.transform.GetComponent<LevelBlockInfo>();
                }

            }

            switch (curState)
            {
                case CameraState.Vehicle:
                    if (curLBI)
                    {
                        targetEulerAngles =  curLBI.VehicleCameraEulerAngles + new Vector3(0, curLBI.transform.eulerAngles.y, 0);
                        targetPosition = focus.transform.position  + curLBI.transform.rotation * curLBI.VehicleCameraOffset;
                        curOffset = curLBI.transform.rotation * curLBI.VehicleCameraOffset;

                        updateActualMovement( curLBI.VehicleCameraFollowSpeed, curLBI.VehicleCameraTrackSpeed);
                    }

                    break;

                case CameraState.Rider:
                    if (curLBI)
                    {
                        targetEulerAngles =  curLBI.RiderCameraEulerAngles + new Vector3(0,curLBI.transform.eulerAngles.y,0);
                        targetPosition = focus.transform.position  + curLBI.transform.rotation * curLBI.RiderCameraOffset;
                        curOffset = curLBI.transform.rotation * curLBI.RiderCameraOffset;


                        updateActualMovement(curLBI.RiderCameraFollowSpeed, curLBI.RiderCameraTrackSpeed);
                    }

                    break;

                case CameraState.Dead:
                    if (curLBI)
                    {
                        //deathZoom -= Time.deltaTime;
                        //deathZoom = Mathf.Max(deathZoom, deathZoomMin);

                        deathZoom += Time.deltaTime;
                        deathZoom = Mathf.Min(deathZoom, deathZoomMin);

                        //transform.LookAt(new Vector3 (targetPosition.x, focus.position.y, targetPosition.z) + curLBI.transform.forward * 0.3f);
                        targetEulerAngles = new Vector3(90, curLBI.transform.eulerAngles.y, 0);
                        targetPosition = focus.transform.position + Vector3.up * deathZoom  -curLBI.transform.forward * 0.3f;
                        curOffset = Vector3.up;

                        updateActualMovement(deathZoomFollow, deathZoomTrack);
                    }

                    break;

                default:
                        
                    break;
            }

            
 
        }
    }

    public void updateActualMovement(float follow, float track)
    {

        if (firstLBI)
        {
            transform.position = targetPosition;
            transform.eulerAngles = targetEulerAngles;
            firstLBI = false;
        }

        Quaternion rot = transform.rotation;
        rot.eulerAngles = targetEulerAngles;// (targetEulerAngles.x,targetEulerAngles.y,targetEulerAngles.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, track * Time.deltaTime);

        Vector3 dir = (targetPosition - transform.position).normalized * Mathf.Min((targetPosition - transform.position).magnitude, 1) * (Mathf.Max((targetPosition - transform.position).magnitude, 1));

        transform.position += dir * follow * Time.deltaTime;//default rotation behind

        prevFocusPos = focus.position;
    }

    public void ChangeFocus(Transform newFocus)
    {
        //forward = newFocus.rotation;
        if (focus != newFocus)
        {
            focus = newFocus;
        }
        if (focus.GetComponent<BasicVehicle>() != null)
        {
            curState = CameraState.Vehicle;
        }
        else if (focus.GetComponent<BasicRider>())
        {
            curState = CameraState.Rider;
        }
        else
        {
            deathZoom = deathZoomMax;
            curState = CameraState.Dead;
        }
    }

    public void ChangeDistance(float distanceBack, float speedMultiplier)
    {
        //backDistanceTarget = distanceBack;
        //backDistanceSpeed = speedMultiplier;

    }
}
