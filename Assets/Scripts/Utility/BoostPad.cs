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
            other.GetComponentInParent<BasicVehicle>().normalMaxSpeed = other.GetComponentInParent<BasicVehicle>().normalMaxSpeed * 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle") && other.GetComponentInParent<BasicVehicle>().boosting == true)
        { 
            other.GetComponentInParent<BasicVehicle>().boosting = false;
            other.GetComponentInParent<BasicVehicle>().normalMaxSpeed = other.GetComponentInParent<BasicVehicle>().normalMaxSpeed * 1/2;
        }
    }
}

