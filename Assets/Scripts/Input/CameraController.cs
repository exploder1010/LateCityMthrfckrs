using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    
    //public float camSensX; //todo: set with player prefs
    //public float camSensY; //todo: set with player prefs

    //float camMaxX = 360; 
    //float camMinX = -360;
    //public float camMaxY; 
    //public float camMinY; 

    //private float camRotX;
    //private float camRotY;
    //private float backDistance = 20f;
    //private float backDistanceTarget;
    //private float backDistanceSpeed;
    //private float upDistance = 8f;
    private Transform focus;
    //private Quaternion forward;
    //private float lerpSpeed;
    
    private Vector3 targetPosition;
    private Vector3 targetEulerAngles;
    //private bool hasSpinHopped;
    //private bool prevHasSpinHopped;
    //private bool prevRider;
    //private bool prevCarGrounded;
    //private float prevFocusEulerY;

    private float followSpeed;
    private float trackSpeed;

    //nicks new stuff

    private enum CameraState { Vehicle = 0, Rider, Dead,  };
    private CameraState curState;

    private LevelBlockInfo curLBI;

    // Use this for initialization
    void Start () {
        //forward = new Quaternion(0, 0, 0, 0);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //update camera input

        //camRotX += (InputManager.GetAxis("LookHorizontal")) * camSensX;
        //camRotY -= (InputManager.GetAxis("LookVertical")) * camSensY;

        //camRotX = ClampAngle(camRotX, camMinX, camMaxX);
        //camRotY = ClampAngle(camRotY, camMinY, camMaxY);

        if (focus != null)
        {

            float dist = 100f;
            int layerMask = 1 << LayerMask.NameToLayer("Vehicle");
            Vector3 pos = focus.position + transform.up;
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

                    targetEulerAngles = curLBI.VehicleCameraEulerAngles;
                    targetPosition = focus.transform.position + curLBI.VehicleCameraOffset;//default rotation behind

                    break;

                case CameraState.Rider:

                    targetEulerAngles = curLBI.RiderCameraEulerAngles;
                    targetPosition = focus.transform.position + curLBI.RiderCameraOffset;//default rotation behind

                    break;

                case CameraState.Dead:

                    targetEulerAngles = new Vector3(25, 0, 0);
                    targetPosition = focus.transform.position - (Vector3.forward * 7f) - (Vector3.right * 0) + (Vector3.up * 6f);//default rotation behind

                    break;

                default:
                        
                    break;
            }

            transform.eulerAngles = targetEulerAngles;
            transform.position = targetPosition;//default rotation behind

            ///old below

            //if (backDistanceTarget > backDistance + .01f || backDistanceTarget < backDistance - .01f)
            //    backDistance = Mathf.Lerp(backDistance, backDistanceTarget, backDistanceSpeed * Time.deltaTime);


            //Vector3 nahThisThePosition = transform.position;

            //if (focus.transform.GetComponent<BasicVehicle>() && focus.transform.GetComponent<BasicVehicle>().easyCheckWheelsOnGround())
            //{
            //    if (hasSpinHopped || prevRider || !prevCarGrounded)
            //    {
            //        //Debug.Log("car shifter");
            //        //transform.rotation.SetLookRotation(focus.forward,focus.up);
            //        camRotX = -Mathf.DeltaAngle(transform.eulerAngles.y, focus.eulerAngles.y);
            //        hasSpinHopped = false;
            //    }
            //    transform.position = focus.transform.position - (focus.transform.forward * backDistance) + (focus.transform.up * upDistance);//default rotation behind vehicle
            //    transform.eulerAngles = new Vector3(focus.eulerAngles.x, focus.eulerAngles.y, focus.eulerAngles.z);
            //    transform.RotateAround(focus.transform.position, focus.transform.right, camRotY);//vert rot
            //    transform.RotateAround(focus.transform.position, focus.transform.up, camRotX);//horz rot
            //    camRotX = Mathf.LerpAngle(camRotX, 0, lerpSpeed * Time.deltaTime);

            //}
            //else
            //{
            //    if (!prevRider && focus.transform.GetComponent<BasicRider>() && prevCarGrounded || focus.transform.GetComponent<BasicVehicle>() && !focus.transform.GetComponent<BasicVehicle>().easyCheckWheelsOnGround() && prevCarGrounded)
            //    {
            //        //Debug.Log("air shifter");

            //        camRotX += prevFocusEulerY;
            //    }
            //    transform.position = focus.transform.position - (Vector3.forward * backDistance) + (Vector3.up * upDistance);//default rotation behind vehicle
            //    transform.LookAt(focus.transform.position);
            //    transform.RotateAround(focus.transform.position, Vector3.up, camRotX);//x rot
            //    transform.RotateAround(focus.transform.position, transform.right, camRotY);//y rot

            //    //if ((hasSpinHopped && focus.transform.GetComponent<BasicVehicle>() && !focus.transform.GetComponent<BasicVehicle>().isSpinMoveHop()))
            //    //{

            //    //    camRotX = Mathf.LerpAngle(camRotX, forward.eulerAngles.y, 5f * Time.deltaTime);
            //    //    if(Mathf.Abs(camRotX - forward.eulerAngles.y) < 5f)
            //    //    {
            //    //        hasSpinHopped = false;
            //    //    }
            //    //}
            //    //camRotX = Mathf.LerpAngle(camRotX, forward.eulerAngles.y, lerpSpeed * Time.deltaTime);
            //}
            //targetPosition = transform.position;
            ////targetRotation = transform.rotation;
            //transform.position = nahThisThePosition;
            ////transform.rotation = nahThisTheRotation;

            ////transform.position += (targetPosition - transform.position) * Time.deltaTime * 30f;
            ////if((targetPosition - transform.position).magnitude > 1f)
            ////{
            //    transform.position += (targetPosition - transform.position) * Time.deltaTime * 30f;
            //    //transform.position = targetPosition;
            ////}

            ////transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 50f);

            ////spring arm
            //RaycastHit hit;
            //int layerMask = 1 << LayerMask.NameToLayer("Road");

            //if (Physics.Raycast(focus.transform.position, (transform.position - focus.transform.position).normalized, out hit, (transform.position - focus.transform.position).magnitude, layerMask))
            //{
            //    //Debug.Log("use spring arm" + hit.transform.gameObject);
            //    transform.position = hit.point;
            //}

            //prevCarGrounded = (focus.GetComponent<BasicVehicle>() && focus.GetComponent<BasicVehicle>().easyCheckWheelsOnGround()); 
            //prevRider = focus.GetComponent<BasicRider>() != null;
            //prevFocusEulerY = focus.eulerAngles.y;
        }
        //camRotX = Mathf.LerpAngle(camRotX, forward.eulerAngles.y, lerpSpeed * Time.deltaTime);
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
            //lerpSpeed = 1.5f;
        }
        else if (focus.GetComponent<BasicRider>())
        {
            curState = CameraState.Rider;
            //lerpSpeed = 0;
        }
        else
        {
            curState = CameraState.Dead;
            //lerpSpeed = 0;
        }
    }

    public void ChangeDistance(float distanceBack, float speedMultiplier)
    {
        //backDistanceTarget = distanceBack;
        //backDistanceSpeed = speedMultiplier;

    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle <= -360F)
            angle += 360F;
        if (angle >= 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
