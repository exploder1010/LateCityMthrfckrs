﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour {

    [SerializeField]
    GameObject[] CarToSpawn;
    GameObject[] SpawnedCar;

    Transform mc;

    bool[] spawned;


    // Use this for initialization
    void Start()
    {
        if (mc == null && GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            mc = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {

 
        if (mc == null && GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            mc = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
        else
        {

            if (spawned == null && ((transform.position - mc.position).magnitude < 200))
            {


                spawned = new bool[CarToSpawn.Length];
                SpawnedCar = new GameObject[CarToSpawn.Length];
                for (int i = 0; i < CarToSpawn.Length; i++)
                {
                    spawned[i] = true;
                    SpawnedCar[i] = Instantiate(CarToSpawn[i], CarToSpawn[i].transform.position, CarToSpawn[i].transform.rotation);
                    SpawnedCar[i].SetActive(true);
                    SpawnedCar[i].tag = "DestroyInScene";
                }



                //SpawnedCar = Instantiate(CarToSpawn, transform.position, transform.rotation);
                //SpawnedCar.SetActive(true);
                //SpawnedCar.tag = "DestroyInScene";

            }

            if(spawned != null)
            {
                for (int i = 0; i < SpawnedCar.Length; i++)
                {
                    if (spawned[i] && (SpawnedCar[i].transform.position - mc.position).magnitude > 200)
                    {
                        Debug.Log("despawn");
                        spawned[i] = false;
                        Destroy(SpawnedCar[i]);
                        SpawnedCar[i] = null;


                    }
                }
            }
            


        }


    }
}
