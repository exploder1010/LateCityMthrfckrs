using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirRing : MonoBehaviour {

    public Text textPrefab;
    public GameObject particlePrefab;
    public float speed = 20f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Rider"))
        {
            GameObject.FindGameObjectWithTag("HUD").transform.GetComponent<timerScript>().addTime(10);
            Text bonustext = Instantiate(textPrefab, new Vector3(233, 55, 0), GameObject.FindGameObjectWithTag("HUD").transform.rotation) as Text;
            bonustext.transform.SetParent(GameObject.FindGameObjectWithTag("HUD").transform, false);
            bonustext.fontSize = 50;
            bonustext.text = "10 seconds added!";
            Destroy(this);
            //flashy effect go here
            speed = 100f;
            this.gameObject.transform.SetParent(GameObject.Find("BR_Business_NickTestVariables(Clone)").transform);
            this.gameObject.transform.localScale -= new Vector3(5, 5, 5);
            //wait a bit and put above line in a loop. Exit loop and use next 2 lines
            GameObject ringEffect = Instantiate(particlePrefab, this.gameObject.transform);
            Destroy(this.gameObject);
        }
    }
}
