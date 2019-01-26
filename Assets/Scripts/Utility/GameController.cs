using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Luminosity.IO;

public class GameController : MonoBehaviour {

    //cinematic stuff 
    //float spawnTimer;
    [SerializeField]
    float spawnTimeSet = 3f;
    GameObject fallingRider;

    public static GameController instance;

    public GameObject mainCamera_Prefab;

    public GameObject playerController_Prefab;

    public GameObject startingVehicle_Prefab; // to do: set this with playerprefs in menu

    public GameObject selectedRider_Prefab; // to do: set this with playerprefs in menu

    public Transform spawnLocation; // to do: set this with playerprefs in menu

    public bool startInAir; // to do: set this with playerprefs in menu

    protected PlayerController curPlayerController;


    GameObject startVehicle;
    GameObject startVehicleInstance;
    GameObject mainCamera;

    //GameObject HUD;
    //GameObject HUDInstance;

    GameObject[] OriginalResetObjects;
    GameObject[] InstanceResetObjects;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += InitializeScene;
    }

    // Use this for initialization
    void Start() {
        GameController.instance = this;
            
        GameObject levelEditor = GameObject.Find("LevelEditor");
        if (levelEditor != null){
            Destroy(levelEditor);
        }
    }
  
    void InitializeScene(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
        {
            print("Finding Vehicles");
            OriginalResetObjects = GameObject.FindGameObjectsWithTag("ResetInScene");
            InstanceResetObjects = new GameObject[OriginalResetObjects.Length];
            foreach (GameObject v in OriginalResetObjects)
                v.SetActive(false);

            //HUD = GameObject.FindGameObjectWithTag("HUD");
            //HUD.SetActive(false);


            SpawnStartCar();

            mainCamera = Instantiate(mainCamera_Prefab);

            resetScene();
        }
    }

    void SpawnStartCar()
    {
        print("Spawning start car");
        //begin level in car
        if (!startInAir)
        {
            startVehicle = GameObject.Instantiate(startingVehicle_Prefab, spawnLocation.position, spawnLocation.rotation);
            startVehicle.SetActive(false);
        }
    }

    public void resetScene()
    {
        print("Resetting Scene");
        ResetObjects();
        SpawnPlayer();
    }

    private void ResetObjects()
    {
        // Destroy and Instantiate scene cars
        print("Resetting Objects");

        GameObject.Find("HUD").GetComponent<ButtonScripts>().StopAllCoroutines();

        GameObject[] killtheseheehee = GameObject.FindGameObjectsWithTag("DestroyInScene");
        for (int i = 0; i < killtheseheehee.Length; i++)
        {
            GameObject.Destroy(killtheseheehee[i]);
        }

        for (int i = 0; i < InstanceResetObjects.Length; i++)
        {
            GameObject.Destroy(InstanceResetObjects[i]);
        }
        InstanceResetObjects = new GameObject[OriginalResetObjects.Length];
        for (int i = 0; i < OriginalResetObjects.Length; i++)
        {
            InstanceResetObjects[i] = Instantiate(OriginalResetObjects[i]);
            InstanceResetObjects[i].SetActive(true);
        }

        if (GameObject.FindGameObjectWithTag("HUD"))
        {
            GameObject.FindGameObjectWithTag("HUD").GetComponent<timerScript>().timeRemaining = GameObject.FindGameObjectWithTag("HUD").GetComponent<timerScript>().startTime;
            GameObject.FindGameObjectWithTag("HUD").GetComponent<timerScript>().setSpawning();
        }
        //Destroy(HUDInstance);
        //HUDInstance = Instantiate(HUD);
        //HUDInstance.SetActive(true);


    }

    void SpawnPlayer()
    {
        // Destroy and Instantiate start car
        GameObject.Destroy(startVehicleInstance);
        startVehicleInstance = Instantiate(startVehicle);
        startVehicleInstance.SetActive(true);

        fallingRider = Instantiate(selectedRider_Prefab, startVehicleInstance.transform.position + Vector3.up * 65f, startVehicleInstance.transform.rotation);
        fallingRider.transform.GetComponent<BR_Business>().maxFallSpeed = 30f;
        fallingRider.transform.GetComponent<BR_Business>().noClip = true;
        //fallingRider.transform.GetComponent<Rigidbody>().isKinematic = true;
        //print("Spawning player");
        if (curPlayerController != null)
        {
            //print("cleand");
            curPlayerController.CleanDestroy();
        }

        curPlayerController = GameObject.Instantiate(playerController_Prefab, Vector3.zero, Quaternion.identity).transform.GetComponent<PlayerController>();
        curPlayerController.SetCamera(mainCamera.GetComponent<CameraController>());
        mainCamera.GetComponent<CameraController>().ResetFocus();

        curPlayerController.SelectRider(selectedRider_Prefab);

        if (startVehicleInstance.GetComponent<AiController>())
        {
            Destroy(startVehicleInstance.GetComponent<AiController>());
        }

        curPlayerController.SetSpawnState(fallingRider);

        mainCamera.GetComponent<CameraController>().SetCameraPosition(startVehicleInstance.transform.position - startVehicleInstance.transform.forward  * 2f + startVehicleInstance.transform.up * 15f);

        StopAllCoroutines();
        StartCoroutine(SpawnCountdown());
        //if (startVehicle != null)
        //{
        //    curPlayerController.EnterVehicle(startVehicleInstance.GetComponent<BasicVehicle>());
        //}
        //else
        //{
        //    selectedRider_Prefab.transform.position = spawnLocation.position;
        //    selectedRider_Prefab.transform.rotation = spawnLocation.rotation;

        //    curPlayerController.ExitVehicle();
        //}
    }

    IEnumerator SpawnCountdown()
    {
        yield return new WaitForSeconds(spawnTimeSet);

        Debug.Log("frog");
        curPlayerController.EnterVehicle(startVehicleInstance.GetComponent<BasicVehicle>());
        Destroy(fallingRider);
        if (GameObject.FindGameObjectWithTag("HUD"))
            GameObject.FindGameObjectWithTag("HUD").GetComponent<timerScript>().setGame();
        GameObject.Find("HUD").GetComponent<StartLightScript>().StartTheLights();
    }
}
