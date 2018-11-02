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

        public Transform spawnLocation; // to do: set this with playerprefs in menu

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
                
                BasicVehicle startingVehicle = GameObject.Instantiate(startingVehicle_Prefab, spawnLocation.position, spawnLocation.rotation).GetComponent<BasicVehicle>();

                curPlayerController.EnterVehicle(startingVehicle);
            }
            //begin level in air
            else
            {
                selectedRider_Prefab.transform.position = spawnLocation.position;
                selectedRider_Prefab.transform.rotation = spawnLocation.rotation;

                curPlayerController.SelectRider(selectedRider_Prefab);

                curPlayerController.ExitVehicle();
            }
            

        }

        // Update is called once per frame
        void Update() {

        }

    }
}
