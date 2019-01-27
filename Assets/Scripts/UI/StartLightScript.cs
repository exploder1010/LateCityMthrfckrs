using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLightScript : MonoBehaviour {

    public GameObject StartLight;
    private GameObject redLight;
    private GameObject yellowLight;
    private GameObject greenLight;
    private bool isFirstRun = true;

	// Use this for initialization
	void Start () {
        redLight = StartLight.transform.Find("RedLight").gameObject;
        yellowLight = StartLight.transform.Find("RedLight").gameObject;
        greenLight = StartLight.transform.Find("RedLight").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartTheLights()
    {
        if (isFirstRun)
        {
            // Halt player input for 3-4 seconds
            StartLight.SetActive(true); // Enable StartLight at 0 seconds
            new WaitForSeconds(1f);
            redLight.SetActive(true); // Enable RedLight at 1 second
            new WaitForSeconds(1f);
            yellowLight.SetActive(true); // Enable YellowLight at 2 seconds
            new WaitForSeconds(1f);
            greenLight.SetActive(true); // Enable GreenLight at 3 seconds
            StartLight.SetActive(false);
            isFirstRun = false;
        }
    }
}
