using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour {

    [SerializeField]
    GameObject[] CarToSpawn;
    GameObject[] SpawnedCar;

    [SerializeField]
    float spawnDist = 200f;
    [SerializeField]
    float despawnDist = 300f;

    Transform mc;

    bool[] spawned;

    public float SpawnCountdown;

    // Use this for initialization
    void Start()
    {
        if (mc == null && GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            mc = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        for(int i = 0; i < CarToSpawn.Length; i++)
        {
            if(CarToSpawn[i] == null)
            {
                Destroy(this);
            }
            else
            {

                CarToSpawn[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(SpawnCountdown > 0)
        {
            SpawnCountdown -= Time.deltaTime;
        }
        else
        {
            if (mc == null && GameObject.FindGameObjectWithTag("MainCamera") != null)
            {
                mc = GameObject.FindGameObjectWithTag("MainCamera").transform;
            }
            else
            {

                if (spawned == null && ((transform.position - mc.position).magnitude < spawnDist))
                {


                    spawned = new bool[CarToSpawn.Length];
                    SpawnedCar = new GameObject[CarToSpawn.Length];
                    for (int i = 0; i < CarToSpawn.Length; i++)
                    {
                        spawned[i] = true;
                        SpawnedCar[i] = Instantiate(CarToSpawn[i], CarToSpawn[i].transform.position, CarToSpawn[i].transform.rotation);
                        SpawnedCar[i].SetActive(true);
                        SpawnedCar[i].tag = "DestroyInScene";
                        //Debug.Log("spawned car #" + i);
                    }



                    //SpawnedCar = Instantiate(CarToSpawn, transform.position, transform.rotation);
                    //SpawnedCar.SetActive(true);
                    //SpawnedCar.tag = "DestroyInScene";

                }

                if (spawned != null)
                {
                    for (int i = 0; i < SpawnedCar.Length; i++)
                    {
                        if (spawned[i] && (SpawnedCar[i].transform.position - mc.position).magnitude > despawnDist)
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
}
