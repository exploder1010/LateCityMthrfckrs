using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    
    public float camSensX; //todo: set with player prefs
    public float camSensY; //todo: set with player prefs

    float camMaxX = 360; 
    float camMinX = -360;
    public float camMaxY; 
    public float camMinY; 

    private float camRotX;
    private float camRotY;
    private float backDistance = 10f;
    private float backDistanceTarget;
    private float backDistanceSpeed;
    private float upDistance = 2f;
    private Transform focus;
    private Quaternion forward;
    private float lerpSpeed;
    
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool hasSpinHopped;
    private bool prevHasSpinHopped;
    private bool prevRider;
    private bool prevCarGrounded;
    private float prevFocusEulerY;

    // Use this for initialization
    void Start () {
        forward = new Quaternion(0, 0, 0, 0);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //update camera input

        camRotX += (InputManager.GetAxis("LookHorizontal")) * camSensX;
        camRotY -= (InputManager.GetAxis("LookVertical")) * camSensY;

        camRotX = ClampAngle(camRotX, camMinX, camMaxX);
        camRotY = ClampAngle(camRotY, camMinY, camMaxY);

        if (focus != null)
        {

            if (backDistanceTarget > backDistance + .01f || backDistanceTarget < backDistance - .01f)
                backDistance = Mathf.Lerp(backDistance, backDistanceTarget, backDistanceSpeed * Time.deltaTime);

            Vector3 nahThisThePosition = transform.position;
            //Quaternion nahThisTheRotation = transform.rotation;
            if (focus.transform.GetComponent<BasicVehicle>() && focus.transform.GetComponent<BasicVehicle>().isSpinMoveHop())
            {
                hasSpinHopped = true;
            }

            if (focus.transform.GetComponent<BasicVehicle>() && !focus.transform.GetComponent<BasicVehicle>().isSpinMoveHop() && focus.transform.GetComponent<BasicVehicle>().easyCheckWheelsOnGround())
            {
                if (hasSpinHopped || prevRider || !prevCarGrounded)
                {
                    Debug.Log("car shifter");
                    //transform.rotation.SetLookRotation(focus.forward,focus.up);
                    camRotX = -Mathf.DeltaAngle(transform.eulerAngles.y, focus.eulerAngles.y);
                    hasSpinHopped = false;
                }
                transform.position = focus.transform.position - (focus.transform.forward * backDistance) + (focus.transform.up * upDistance);//default rotation behind vehicle
                transform.eulerAngles = new Vector3(focus.eulerAngles.x, focus.eulerAngles.y, focus.eulerAngles.z);
                transform.RotateAround(focus.transform.position, focus.transform.right, camRotY);//vert rot
                transform.RotateAround(focus.transform.position, focus.transform.up, camRotX);//horz rot
                camRotX = Mathf.LerpAngle(camRotX, 0, lerpSpeed * Time.deltaTime);
                
            }
            else
            {
                if (!prevRider && focus.transform.GetComponent<BasicRider>() && prevCarGrounded || focus.transform.GetComponent<BasicVehicle>() && !focus.transform.GetComponent<BasicVehicle>().easyCheckWheelsOnGround() && !focus.transform.GetComponent<BasicVehicle>().isSpinMoveHop() && prevCarGrounded)
                {
                    Debug.Log("air shifter");

                    camRotX += prevFocusEulerY;
                }
                transform.position = focus.transform.position - (Vector3.forward * backDistance) + (Vector3.up * upDistance);//default rotation behind vehicle
                transform.LookAt(focus.transform.position);
                transform.RotateAround(focus.transform.position, Vector3.up, camRotX);//x rot
                transform.RotateAround(focus.transform.position, transform.right, camRotY);//y rot

                //if ((hasSpinHopped && focus.transform.GetComponent<BasicVehicle>() && !focus.transform.GetComponent<BasicVehicle>().isSpinMoveHop()))
                //{

                //    camRotX = Mathf.LerpAngle(camRotX, forward.eulerAngles.y, 5f * Time.deltaTime);
                //    if(Mathf.Abs(camRotX - forward.eulerAngles.y) < 5f)
                //    {
                //        hasSpinHopped = false;
                //    }
                //}
                //camRotX = Mathf.LerpAngle(camRotX, forward.eulerAngles.y, lerpSpeed * Time.deltaTime);
            }
            targetPosition = transform.position;
            //targetRotation = transform.rotation;
            transform.position = nahThisThePosition;
            //transform.rotation = nahThisTheRotation;

            //transform.position += (targetPosition - transform.position) * Time.deltaTime * 30f;
            //if((targetPosition - transform.position).magnitude > 1f)
            //{
                transform.position += (targetPosition - transform.position) * Time.deltaTime * 30f;
                //transform.position = targetPosition;
            //}

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 50f);

            //spring arm
            RaycastHit hit;
            int layerMask = 1 << LayerMask.NameToLayer("Road");

            if (Physics.Raycast(focus.transform.position, (transform.position - focus.transform.position).normalized, out hit, (transform.position - focus.transform.position).magnitude, layerMask))
            {
                //Debug.Log("use spring arm" + hit.transform.gameObject);
                transform.position = hit.point;
            }

            prevHasSpinHopped = hasSpinHopped;
            prevCarGrounded = (focus.GetComponent<BasicVehicle>() && focus.GetComponent<BasicVehicle>().easyCheckWheelsOnGround()); 
            prevRider = focus.GetComponent<BasicRider>() != null;
            prevFocusEulerY = focus.eulerAngles.y;
        }
        //camRotX = Mathf.LerpAngle(camRotX, forward.eulerAngles.y, lerpSpeed * Time.deltaTime);
    }

    public void ChangeFocus(Transform newFocus)
    {
        forward = newFocus.rotation;
        if (focus != newFocus)
        {
            focus = newFocus;
        }

        if (focus.GetComponent<BasicVehicle>() != null)
        {
            lerpSpeed = 1.5f;
        }
        else
        {
            if (focus.GetComponent<BasicRider>())
            {
                //camRotX = focus.transform.eulerAngles.y;
            }
            lerpSpeed = 0;
        }
    }

    public void ChangeDistance(float distanceBack, float speedMultiplier)
    {
        backDistanceTarget = distanceBack;
        backDistanceSpeed = speedMultiplier;

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
