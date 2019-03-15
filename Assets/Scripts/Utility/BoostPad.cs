using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class BoostPad : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle") && other.GetComponentInParent<BasicVehicle>().boosting == false)
        {
            other.GetComponentInParent<BasicVehicle>().boosting = true;
            other.GetComponentInParent<BasicVehicle>().potentialMaxSpeed = other.GetComponentInParent<BasicVehicle>().potentialMaxSpeed * 2;
           // other.GetComponentInParent<BasicVehicle>().initializeSpeed(other.GetComponentInParent<BasicVehicle>().normalMaxSpeed * 2, other.GetComponent<BasicVehicle>().GetComponent<Rigidbody>().velocity.magnitude, false);
            other.GetComponentInParent<BasicVehicle>().MotorTorque = 10000;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle") && other.GetComponentInParent<BasicVehicle>().boosting == true)
        { 
            other.GetComponentInParent<BasicVehicle>().boosting = false;
            other.GetComponentInParent<BasicVehicle>().potentialMaxSpeed = other.GetComponentInParent<BasicVehicle>().potentialMaxSpeed * 1/2;
            //other.GetComponentInParent<BasicVehicle>().initializeSpeed(other.GetComponentInParent<BasicVehicle>().normalMaxSpeed * 1 / 2, other.GetComponent<BasicVehicle>().GetComponent<Rigidbody>().velocity.magnitude, false);
            other.GetComponentInParent<BasicVehicle>().MotorTorque = 5000;
        }
    }
}

