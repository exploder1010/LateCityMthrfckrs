using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpawner : MonoBehaviour {

    public GameObject fire;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameObject fireTrail = Instantiate(fire, this.gameObject.transform.position, this.gameObject.transform.rotation);
    }
}
