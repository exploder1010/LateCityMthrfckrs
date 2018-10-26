using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Luminosity.IO
{
    //Handles ALL in-game input.
    public class PlayerController : MonoBehaviour
    {
        
        //type of character selected for this level
        GameObject selectedCharacter_Prefab;

        //references to current vehicle or current player
        BasicVehicle curVehicle;
        BasicRider curRider;
        Rigidbody curRagdoll;

        //reference to camera
        GameObject mainCamera;

        //states
        private enum PlayerState { Vehicle = 0, Rider, Dead };
        private PlayerState curState;

        //camera variables
        public float cameraDistance;
        private Quaternion originalRotation;

        public float camSensX; //todo: set with player prefs
        public float camSensY; //todo: set with player prefs

        float camMinX = -360; //todo: set with player prefs
        public float camMinY; //todo: set with player prefs

        float camMaxX = 360; //todo: set with player prefs
        public float camMaxY; //todo: set with player prefs

        private float camRotX;
        private float camRotY;

        // Use this for initialization
        void Start()
        {
            originalRotation = transform.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            //update camera input
            camRotX += (InputManager.GetAxis("LookHorizontal")) * camSensX;
            camRotY -= (InputManager.GetAxis("LookVertical")) * camSensY;

            camRotX = ClampAngle(camRotX, camMinX, camMaxX);
            camRotY = ClampAngle(camRotY, camMinY, camMaxY);

            Quaternion xQuaternion = Quaternion.AngleAxis(camRotX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(camRotY, -Vector3.right);

            //update vehicle or rider respectively, depending on state.
            switch (curState)
            {
                //process input for vehicle
                case PlayerState.Vehicle:

                    //input horizontal movement
                    curVehicle.inputHorz(InputManager.GetAxis("Horizontal"));

                    //input accelleration
                    curVehicle.inputAccel(InputManager.GetAxis("Accelerate"));

                    //input jump
                    if (InputManager.GetButtonDown("Jump"))
                    {
                        
                        ExitVehicle();
                        break;
                    }

                    //camera follow vehicle:
                    mainCamera.transform.position = curVehicle.transform.position - (Vector3.forward * 10f) + (Vector3.up * 2f);//default rotation behind vehicle
                    mainCamera.transform.LookAt(curVehicle.transform.position + (Vector3.up * 2f));//look slightly about vehicle
                    mainCamera.transform.RotateAround(curVehicle.transform.position, Vector3.up, camRotX);//x rot
                    mainCamera.transform.RotateAround(curVehicle.transform.position, mainCamera.transform.right, camRotY);//y rot

                    break;

                //process input for air movement
                case PlayerState.Rider:

                    if(curRider.vehicleToEnter() != null)
                    {
                        EnterVehicle(curRider.vehicleToEnter());
                        break;
                    }
                    
                    if(curRider.checkRagdoll() != null)
                    {
                        curRagdoll = curRider.checkRagdoll().transform.GetComponent<RagdollStorage>().rb;
                        curState = PlayerState.Dead;
                        Destroy(curRider.gameObject);
                        break;
                    }

                    //input horizontal movement
                    curRider.inputHorz(InputManager.GetAxis("Horizontal"));

                    //input vertical movement
                    curRider.inputVert(InputManager.GetAxis("Vertical"));

                    //input abilty
                    if (InputManager.GetButtonDown("Jump"))
                    {
                        curRider.inputAbility(1);
                    }
                    else if (InputManager.GetButton("Jump"))
                    {
                        curRider.inputAbility(2);
                    }
                    else if (InputManager.GetButtonUp("Jump"))
                    {
                        curRider.inputAbility(3);
                    }
                    else
                    {
                        curRider.inputAbility(0);
                    }

                    //input breakin
                    if (InputManager.GetButtonDown("BreakIn"))
                    {
                        curRider.inputBreakIn(1);
                    }
                    else if (InputManager.GetButton("BreakIn"))
                    {
                        curRider.inputBreakIn(2);
                    }
                    else if (InputManager.GetButtonUp("BreakIn"))
                    {
                        curRider.inputBreakIn(3);
                    }
                    else
                    {
                        curRider.inputBreakIn(0);
                    }

                    //camera follow rider:
                    mainCamera.transform.position = curRider.transform.position - (Vector3.forward * 10f) + (Vector3.up * 2f); //default rotation behind rider
                    mainCamera.transform.LookAt(curRider.transform.position + (Vector3.up * 2f)); //look slightly about rider
                    mainCamera.transform.RotateAround(curRider.transform.position, Vector3.up, camRotX); //rotate to current x rot
                    mainCamera.transform.RotateAround(curRider.transform.position, mainCamera.transform.right, camRotY); //rotate y rot
                    break;

                case PlayerState.Dead:

                    //camera follow ragdoll:
                    mainCamera.transform.position = curRagdoll.transform.position - (Vector3.forward * 10f) + (Vector3.up * 2f); //default rotation behind rider
                    mainCamera.transform.LookAt(curRagdoll.transform.position + (Vector3.up * 2f)); //look slightly about rider
                    mainCamera.transform.RotateAround(curRagdoll.transform.position, Vector3.up, camRotX); //rotate to current x rot
                    mainCamera.transform.RotateAround(curRagdoll.transform.position, mainCamera.transform.right, camRotY); //rotate y rot
                    break;

                default:
                    Debug.Log("ERROR - NO RIDER OR VEHICLE");
                    break;
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle <= -360F)
                angle += 360F;
            if (angle >= 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }

        public void SetCamera(GameObject newCamera)
        {
            mainCamera = newCamera;
        }

        public void SelectRider(GameObject newCharacter)
        {
            selectedCharacter_Prefab = newCharacter;
        }
        
        public void EnterVehicle(BasicVehicle newVehicle)
        {
            Debug.Log("ENTER CAR " + Time.time);

            curState = PlayerState.Vehicle;
            curVehicle = newVehicle;
            
            if(curRider != null)
            {
                curVehicle.initializeSpeed(curRider.calculateNewCarMaxSpeed(),curRider.calculateNewCarStartSpeed());
                Destroy(curRider.transform.gameObject);
            }
            else
            {
                curVehicle.initializeSpeed(0,0);
            }
        }

        public void ExitVehicle()
        {

            Debug.Log("EXIT CAR " + Time.time);

            curState = PlayerState.Rider;

            if (curVehicle != null)
            {
                //let go of steering wheel
                curVehicle.inputHorz(0);
                curVehicle.inputAccel(0);

                //spawn rider above car.
                curRider = Instantiate(selectedCharacter_Prefab, curVehicle.transform.position + Vector3.up * 2.5f, Quaternion.Euler(0,curVehicle.transform.eulerAngles.y,0)).GetComponent<BasicRider>();

                curRider.externalStart(mainCamera.transform);
                curRider.beginCarJump(curVehicle.transform.GetComponent<Rigidbody>().velocity.magnitude);
            }
            else
            {
                //spawn rider at prefab coordinates.
                curRider = Instantiate(selectedCharacter_Prefab, selectedCharacter_Prefab.transform.position, selectedCharacter_Prefab.transform.rotation).GetComponent<BasicRider>();

                curRider.externalStart(mainCamera.transform);
                curRider.beginCarJump(30f);
            }

            curVehicle = null;

            
        }
    }
}
