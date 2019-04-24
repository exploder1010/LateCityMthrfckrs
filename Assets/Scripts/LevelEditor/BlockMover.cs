using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMover : MonoBehaviour {

    public Vector3 speed;
    public timerScript timeScript;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (timeScript.spawning == false)
        {
            transform.position += speed * Time.deltaTime;
        }
	}
}
