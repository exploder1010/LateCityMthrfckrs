using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Luminosity.IO;

public class GameController : MonoBehaviour {

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

    GameObject[] OriginalVehicles;
    GameObject[] SpawnedVehicles;

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

        print("Finding Vehicles");
        OriginalVehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        SpawnedVehicles = new GameObject[OriginalVehicles.Length];
        foreach (GameObject v in OriginalVehicles)
            v.SetActive(false);

        SpawnStartCar();

        mainCamera = Instantiate(mainCamera_Prefab);

        resetScene();
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
        ResetCars();
        SpawnPlayer();
    }

    private void ResetCars()
    {
        // Destroy and Instantiate scene cars
        print("Resetting vehicles");
        for (int i = 0; i < SpawnedVehicles.Length; i++)
        {
            GameObject.Destroy(SpawnedVehicles[i]);
        }
        SpawnedVehicles = new GameObject[OriginalVehicles.Length];
        for (int i = 0; i < OriginalVehicles.Length; i++)
        {
            SpawnedVehicles[i] = Instantiate(OriginalVehicles[i]);
            SpawnedVehicles[i].SetActive(true);
        }

        // Destroy and Instantiate start car
        GameObject.Destroy(startVehicleInstance);
        startVehicleInstance = Instantiate(startVehicle);
        startVehicleInstance.SetActive(true);
    }

    void SpawnPlayer()
    {
        print("Spawning player");
        if (curPlayerController != null)
            Destroy(curPlayerController.gameObject);

        curPlayerController = GameObject.Instantiate(playerController_Prefab, Vector3.zero, Quaternion.identity).transform.GetComponent<PlayerController>();
        curPlayerController.SetCamera(mainCamera.GetComponent<CameraController>());

        curPlayerController.SelectRider(selectedRider_Prefab);

        if (startVehicle != null)
        {
            curPlayerController.EnterVehicle(startVehicleInstance.GetComponent<BasicVehicle>());
        }
        else
        {
            selectedRider_Prefab.transform.position = spawnLocation.position;
            selectedRider_Prefab.transform.rotation = spawnLocation.rotation;

            curPlayerController.ExitVehicle();
        }
    }

}
