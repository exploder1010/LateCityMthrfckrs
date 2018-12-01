using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour {

    public float startTime;
    public float timeRemaining;

    // Use this for initialization
    void Start () {
        timeRemaining = startTime;
    }
	
	// Update is called once per frame
	void Update () {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
