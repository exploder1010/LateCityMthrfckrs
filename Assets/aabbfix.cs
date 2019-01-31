using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aabbfix : MonoBehaviour {
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        if (transform.GetComponent<Rigidbody>())
        {
            rb = transform.GetComponent<Rigidbody>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(rb.velocity.magnitude > 100)
        {
            rb.velocity = Vector3.zero;
            transform.position = Vector3.zero;
        }
	}
}
