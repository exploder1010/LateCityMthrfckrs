using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLightScript : MonoBehaviour {

    public GameObject StartLight;
    public  GameObject redLight;
    public  GameObject yellowLight;
    public GameObject greenLight;
    public bool isFirstRun = true;

    bool on;
    float timer = 0;

	// Use this for initialization
	void Start () {
        //redLight = StartLight.transform.Find("RedLight").gameObject;
        //yellowLight = StartLight.transform.Find("YellowLight").gameObject;
        //greenLight = StartLight.transform.Find("GreenLight").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if (on)
        {
            timer += Time.deltaTime;
            if(timer > 0)
            {
                StartLight.SetActive(true);
            }
            if(timer > 1)
            {
                redLight.SetActive(true);
            }
            if(timer > 2)
            {
                yellowLight.SetActive(true);
            }
            if(timer > 3)
            {
                greenLight.SetActive(true);
            }
            if(timer > 4)
            {
                on = false;
                StartLight.SetActive(false);
                redLight.SetActive(false);
                yellowLight.SetActive(false);
                greenLight.SetActive(false);
            }
        }
	}

    public void StartTheLights()
    {
        //if (isFirstRun)
        //{
        on = true;
        if (isFirstRun)
        {

            timer = 0;
        }
        else
        {
            timer = 3;
        }
        StartLight.SetActive(false);
        redLight.SetActive(false);
        yellowLight.SetActive(false);
        greenLight.SetActive(false);
        //Debug.Log("fuck ");
        isFirstRun = false;
        // Halt player input for 3-4 seconds
        //StartLight.SetActive(true); // Enable StartLight at 0 seconds
        //new WaitForSeconds(1f);
        //redLight.SetActive(true); // Enable RedLight at 1 second
        //new WaitForSeconds(1f);
        //yellowLight.SetActive(true); // Enable YellowLight at 2 seconds
        //new WaitForSeconds(1f);
        //greenLight.SetActive(true); // Enable GreenLight at 3 seconds
        //new WaitForSeconds(1f);
        //StartLight.SetActive(false);
        //redLight.SetActive(false);
        //yellowLight.SetActive(false);
        //greenLight.SetActive(false);



        //}
    }
}
