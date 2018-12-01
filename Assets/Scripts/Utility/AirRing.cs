using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirRing : MonoBehaviour {

    public Text textPrefab;

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
            Text bonustext = Instantiate(textPrefab, new Vector3(233, 55, 0), GameObject.FindGameObjectWithTag("HUD").transform.rotation) as Text;
            bonustext.transform.SetParent(GameObject.FindGameObjectWithTag("HUD").transform, false);
            bonustext.fontSize = 50;
            bonustext.text = "10 seconds added!";
            Destroy(this);
            //flashy effect go here
            Destroy(this.gameObject);
        }
    }
}
