using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirRing : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Rider"))
        {
            GameObject.FindGameObjectWithTag("HUD").transform.GetComponent<timerScript>().addTime(10);
        }
    }
}
