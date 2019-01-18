using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class BoostPad : MonoBehaviour
{
    public bool boost;
    void Start()
    {
        boost = false;
    }

    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
        {
            boost = true;
            other.GetComponentInParent<BasicVehicle>().boosting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
        { 
            boost = false;
            other.GetComponentInParent<BasicVehicle>().boosting = false;
        }
    }
}

