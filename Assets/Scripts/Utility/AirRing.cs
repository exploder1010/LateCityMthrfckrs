using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirRing : MonoBehaviour {

    public Text textPrefab;
    public GameObject particlePrefab;
    public float speed = 20f;
    public float animLength;
    public float timeRemaining;
    private bool done = false;

    // Use this for initialization
    void Start () {
        timeRemaining = animLength;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
        if(done == true)
        {
            airEffect();
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Rider"))
        {
            if(done == false)
            {
                GameObject.FindGameObjectWithTag("HUD").transform.GetComponent<timerScript>().addTime(10);
                Text bonustext = Instantiate(textPrefab, new Vector3(233, 55, 0), GameObject.FindGameObjectWithTag("HUD").transform.rotation) as Text;
                bonustext.transform.SetParent(GameObject.FindGameObjectWithTag("HUD").transform, false);
                bonustext.fontSize = 50;
                bonustext.text = "10 seconds added!";
                done = true;
                airEffect();
            }
            //flashy effect go here
        }
    }

    private void airEffect()
    {
        speed = 400f;
        this.gameObject.transform.SetParent(GameObject.Find("BR_Business_NickTestVariables(Clone)").transform);
        this.gameObject.transform.localPosition = Vector3.zero;
        this.gameObject.transform.localScale -= new Vector3(1.5f, 1.5f, 1.5f);
        //wait a bit and put above line in a loop. Exit loop and use next 2 lines
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            Destroy(this.gameObject);
            GameObject ringEffect = Instantiate(particlePrefab, this.gameObject.transform);
        }
    }
}
