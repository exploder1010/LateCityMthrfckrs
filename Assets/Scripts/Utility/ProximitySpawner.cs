using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySpawner : MonoBehaviour {

    Rigidbody rb;
    BasicVehicle bv;
    AiController ai;
    GameObject[] theChildren;
    bool[] childStates;

    Transform mc;
    public float overrideDistance;
    float distance;

    bool spawned;

	// Use this for initialization
	void Start () {
        rb = transform.GetComponent<Rigidbody>();
        bv = transform.GetComponent<BasicVehicle>();
        ai = transform.GetComponent<AiController>();

        rb.isKinematic = true;
        //rb.useGravity = false;
        bv.disabled = true;
        ai.disabled = true;

        theChildren = new GameObject[transform.childCount];
        childStates = new bool[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            theChildren[i] = transform.GetChild(i).gameObject;
            childStates[i] = theChildren[i].activeSelf;
            theChildren[i].SetActive(false);
        }


        distance = 300;
        if(overrideDistance != 0)
        {
        distance = overrideDistance;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (mc == null && GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            mc = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }


        if (!spawned && mc != null && (transform.position - mc.position).magnitude < distance)
        {
            spawned = true;

            rb.isKinematic = false;
            //rb.useGravity = true;
            bv.disabled = false;
            ai.disabled = false;
            for (int i = 0; i < theChildren.Length; i++)
            {
                theChildren[i].SetActive(childStates[i]);
            }

            //Destroy(this);
        }

        if(spawned && mc != null && (transform.position - mc.position).magnitude > distance)
        {
            spawned = false;

            rb.isKinematic = true;
            //rb.useGravity = false;
            bv.disabled = true;
            ai.disabled = true;


            for (int i = 0; i < theChildren.Length; i++)
            {
                theChildren[i].SetActive(false);
            }

        }
    }
}
