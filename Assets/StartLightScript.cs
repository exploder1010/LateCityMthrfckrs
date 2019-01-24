using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartLightScript : MonoBehaviour {

    public GameObject startLight;
    public Image redLight;
    public Image yellowLight;
    public Image greenLight;

	// Use this for initialization
	void Start () {
        StartCoroutine(Countdown(3));
        //redLight = startLight.GetComponentInChildren;
        //yellowLight = startLight.GetComponentInChildren;
        //greenLight = startLight.GetComponentInChildren;
    }
	
	// Update is called once per frame
	void Update () {
		//if(Time.deltaTime > 0 && Time.deltaTime < 1)
  //      {
  //          redLight.gameObject.SetActive(true);
  //      }
	}

    IEnumerator Countdown(int seconds)
    {
        int count = seconds;

        while (count > 0)
        {
            if(count > 2)
            {
                print("Red");
            }
            if(count > 1)
            {
                print("Yellow");
            }
            else
            {
                print("Green");
            }
            yield return new WaitForSeconds(1);
            count--;
        }
    }
}
