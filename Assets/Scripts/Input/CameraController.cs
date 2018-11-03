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
    private float upDistance = 2f;
    private Transform focus;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void LateUpdate () {
        //update camera input
        camRotX += (InputManager.GetAxis("LookHorizontal")) * camSensX;
        camRotY -= (InputManager.GetAxis("LookVertical")) * camSensY;

        camRotX = ClampAngle(camRotX, camMinX, camMaxX);
        camRotY = ClampAngle(camRotY, camMinY, camMaxY);

        Quaternion xQuaternion = Quaternion.AngleAxis(camRotX, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(camRotY, -Vector3.right);

        if (focus != null)
        {
            transform.position = focus.transform.position - (Vector3.forward * backDistance) + (Vector3.up * upDistance);//default rotation behind vehicle
            transform.LookAt(focus.transform.position + (Vector3.up * 2f));//look slightly about vehicle
            transform.RotateAround(focus.transform.position, Vector3.up, camRotX);//x rot
            transform.RotateAround(focus.transform.position, transform.right, camRotY);//y rot
        }
    }

    private void ChangeFocus(Transform newFocus)
    {
        focus = newFocus;
    }

    private void ChangeDistance(float distanceBack)
    { 
        backDistance = Mathf.Lerp(backDistance, distanceBack, 0.01f);
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
