using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySpawner : MonoBehaviour {

    Rigidbody rb;
    BasicVehicle bv;
    Transform mc;
    public float overrideDistance;
    float distance;

	// Use this for initialization
	void Start () {
        rb = transform.GetComponent<Rigidbody>();
        bv = transform.GetComponent<BasicVehicle>();

        rb.isKinematic = true;
        rb.useGravity = false;
        bv.disabled = true;

        distance = 300;
        if(overrideDistance != 0)
        {
            distance = overrideDistance;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (mc == null && GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            mc = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }


        if (mc != null && (transform.position - mc.position).magnitude < distance)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            bv.disabled = false;
            Destroy(this);
        }
	}
}
