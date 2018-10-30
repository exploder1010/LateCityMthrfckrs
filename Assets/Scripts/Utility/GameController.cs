using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Luminosity.IO
{

    public class GameController : MonoBehaviour {

        public GameObject mainCamera_Prefab;

        public GameObject playerController_Prefab;

        public GameObject startingVehicle_Prefab; // to do: set this with playerprefs in menu

        public GameObject selectedRider_Prefab; // to do: set this with playerprefs in menu

        public Vector3 spawnLocation; // to do: set this with playerprefs in menu

        public Vector3 spawnRotation; // to do: set this with playerprefs in menu

        public bool startInAir; // to do: set this with playerprefs in menu

        protected PlayerController curPlayerController;

	    // Use this for initialization
	    void Start() {

            curPlayerController = GameObject.Instantiate(playerController_Prefab, Vector3.zero, Quaternion.identity).transform.GetComponent<PlayerController>();
            curPlayerController.SetCamera(GameObject.Instantiate(mainCamera_Prefab, Vector3.zero, Quaternion.identity));

            //begin level in car
            if (!startInAir)
            {
                curPlayerController.SelectRider(selectedRider_Prefab);

                BasicVehicle startingVehicle = GameObject.Instantiate(startingVehicle_Prefab, spawnLocation, Quaternion.Euler(spawnRotation)).transform.GetComponent<BasicVehicle>();

                curPlayerController.EnterVehicle(startingVehicle);
            }
            //begin level in air
            else
            {
                selectedRider_Prefab.transform.position = spawnLocation;
                selectedRider_Prefab.transform.rotation = Quaternion.Euler(spawnRotation);

                curPlayerController.SelectRider(selectedRider_Prefab);

                curPlayerController.ExitVehicle();
            }
            
            GameObject levelEditor = GameObject.Find("LevelEditor");
            if (levelEditor != null){
                Destroy(levelEditor);
            }
        }

        // Update is called once per frame
        void Update() {

        }

    }
}
