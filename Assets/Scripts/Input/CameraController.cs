using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    

    private Transform focus;
    
    //private Vector3 targetPosition;
    private Vector3 curOffset;
    private Vector3 curEuler;

    private Vector3 targetEulerAngles;

    private float followSpeed;
    private float trackSpeed;

    private float deathZoomMax = 3f;
    private float deathZoomMin = 12f;
    private float deathZoomFollow = 30;
    private float deathZoomTrack = 100;
    private float deathZoom;

    private Vector3 prevFocusPos;

    private enum CameraState { Vehicle = 0, Rider, Dead, Win, Spawning  };
    private CameraState curState;

    private LevelBlockInfo curLBI;
    private bool firstLBI = true;

    // Use this for initialization
    void Start () {
        //forward = new Quaternion(0, 0, 0, 0);
       // ResetFocus();
        curOffset = Vector3.zero;
        curLBI = null;
    }

    public void ResetFocus()
    {
        focus = null;
        curOffset = Vector3.zero;
        curLBI = null;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (focus != null)
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
                        

                        if (curOffset == Vector3.zero)
                        {
                            
                            curOffset = curLBI.VehicleCameraOffset;
                            //Debug.Log("initial focus " + curOffset + " bc " + focus.position + " " + curLBI.transform.rotation * curLBI.VehicleCameraOffset);
                            transform.eulerAngles = curLBI.VehicleCameraEulerAngles + new Vector3(0, curLBI.transform.eulerAngles.y, 0);
                        }
                        else
                        {

                            Vector3 modifiedFocus = focus.position + curLBI.transform.rotation * curLBI.VehicleCameraOffset;
                            //Debug.Log("bish focus " + curOffset + " bc " + focus.position + " " + curLBI.transform.rotation * curLBI.VehicleCameraOffset);
                            Vector3 dir = (modifiedFocus - transform.position).normalized * Mathf.Min((modifiedFocus - transform.position).magnitude, 1);

                            curOffset += dir * curLBI.VehicleCameraFollowSpeed * Time.deltaTime;


                            Quaternion rot = transform.rotation;
                            rot.eulerAngles = curLBI.VehicleCameraEulerAngles + new Vector3(0, curLBI.transform.eulerAngles.y, 0);
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, curLBI.VehicleCameraTrackSpeed * Time.deltaTime);

                        }


                        //targetEulerAngles =  curLBI.VehicleCameraEulerAngles + new Vector3(0, curLBI.transform.eulerAngles.y, 0);
                        //targetPosition = focus.transform.position  + curLBI.transform.rotation * curLBI.VehicleCameraOffset;
                        //curOffset = curLBI.transform.rotation * curLBI.VehicleCameraOffset;

                        //updateActualMovement( curLBI.VehicleCameraFollowSpeed, curLBI.VehicleCameraTrackSpeed);
                    }

                    break;

                case CameraState.Rider:
                    if (curLBI)
                    {
                        

                        if (curOffset == Vector3.zero)
                        {
                            curOffset = curLBI.RiderCameraOffset;
                            transform.eulerAngles = curLBI.RiderCameraEulerAngles + new Vector3(0, curLBI.transform.eulerAngles.y, 0);
                        }
                        else
                        {
                            Vector3 modifiedFocus = focus.position + curLBI.transform.rotation * curLBI.RiderCameraOffset;

                            Vector3 dir = (modifiedFocus - transform.position).normalized * Mathf.Min((modifiedFocus - transform.position).magnitude, 1);

                            curOffset += dir * curLBI.RiderCameraFollowSpeed * Time.deltaTime;


                            Quaternion rot = transform.rotation;
                            rot.eulerAngles = curLBI.RiderCameraEulerAngles + new Vector3(0, curLBI.transform.eulerAngles.y, 0);
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, curLBI.RiderCameraTrackSpeed * Time.deltaTime);

                        }


                        //targetEulerAngles =  curLBI.RiderCameraEulerAngles + new Vector3(0,curLBI.transform.eulerAngles.y,0);
                        //targetPosition = focus.transform.position  + curLBI.transform.rotation * curLBI.RiderCameraOffset;
                        //curOffset = curLBI.transform.rotation * curLBI.RiderCameraOffset;


                        //updateActualMovement(curLBI.RiderCameraFollowSpeed, curLBI.RiderCameraTrackSpeed);
                    }

                    break;

                case CameraState.Dead:
                    if (curLBI)
                    {
                        deathZoom += Time.deltaTime;
                        deathZoom = Mathf.Min(deathZoom, deathZoomMin);

                        Vector3 modifiedFocus = focus.position + Vector3.up * deathZoom;

                        Vector3 dir = (modifiedFocus - transform.position).normalized * Mathf.Min((modifiedFocus - transform.position).magnitude, 1);

                        curOffset += dir * deathZoomFollow * Time.deltaTime;


                        Quaternion rot = transform.rotation;
                        rot.eulerAngles = new Vector3(90, curLBI.transform.eulerAngles.y, 0);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, deathZoomTrack * Time.deltaTime);

                        //deathZoom += Time.deltaTime;
                        //deathZoom = Mathf.Min(deathZoom, deathZoomMin);

                        //targetEulerAngles = new Vector3(90, curLBI.transform.eulerAngles.y, 0);
                        //targetPosition = focus.transform.position + Vector3.up * deathZoom  -curLBI.transform.forward * 0.3f;
                        //curOffset = Vector3.up;

                        //updateActualMovement(deathZoomFollow, deathZoomTrack);
                    }
                    break;

                case CameraState.Win:
                    
                        Debug.Log("win");
                    transform.LookAt(focus.position + focus.transform.up * 1.1f);
                    //curOffset = focus.position + focus.forward;

                    //transform.LookAt(focus);

                    //Quaternion rot = transform.rotation;
                    //rot.eulerAngles = new Vector3(90, curLBI.transform.eulerAngles.y, 0);
                    //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, deathZoomTrack * Time.deltaTime);

                    //deathZoom += Time.deltaTime;
                    //deathZoom = Mathf.Min(deathZoom, deathZoomMin);

                    //targetEulerAngles = new Vector3(90, curLBI.transform.eulerAngles.y, 0);
                    //targetPosition = focus.transform.position + Vector3.up * deathZoom  -curLBI.transform.forward * 0.3f;
                    //curOffset = Vector3.up;

                    //updateActualMovement(deathZoomFollow, deathZoomTrack);



                    break;

                case CameraState.Spawning:
                    transform.LookAt(focus);
                    break;

                default:
                        
                    break;
            }

            if(curState != CameraState.Spawning && curState != CameraState.Win)
            {
                //Debug.Log("doing offset " + curState);
                transform.position = focus.position + curOffset;
            }
 
        }
    }

    public void updateActualMovement(float follow, float track)
    {

        //if (firstLBI)
        //{
        //    transform.position = targetPosition;
        //    transform.eulerAngles = targetEulerAngles;
        //    firstLBI = false;
        //}

        //Quaternion rot = transform.rotation;
        //rot.eulerAngles = targetEulerAngles;// (targetEulerAngles.x,targetEulerAngles.y,targetEulerAngles.z);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, track * Time.deltaTime);

        //Vector3 dir = (targetPosition - transform.position).normalized * Mathf.Min((targetPosition - transform.position).magnitude, 1) * (Mathf.Max((targetPosition - transform.position).magnitude, 1));

        //transform.position += dir * follow * Time.deltaTime;//default rotation behind

        //prevFocusPos = focus.position;
    }

    public void ChangeFocus(Transform newFocus, int state)
    {
        if (focus != newFocus || state != 0)
        {
            curOffset = Vector3.zero;
            focus = newFocus;

            if (focus.GetComponent<BasicVehicle>() != null)
            {
                curState = CameraState.Vehicle;
            }
            else
            {
                Debug.Log(state);
                if(state == 2)
                {
                    Debug.Log("spawn cam set");
                    curState = CameraState.Spawning;
                }
                else if(state == 1)
                {
                    Debug.Log("win cam set");
                    curState = CameraState.Win;
                }
                else if(state == 0)
                {
                    Debug.Log("rider cam set");
                    curState = CameraState.Rider;
                }
                else if(state == -1)
                {
                    deathZoom = deathZoomMax;
                    curState = CameraState.Dead;
                }
                Debug.Log(state);
            }
        }

    }

    public void SetCameraPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void ChangeDistance(float distanceBack, float speedMultiplier)
    {
        //backDistanceTarget = distanceBack;
        //backDistanceSpeed = speedMultiplier;

    }
}
