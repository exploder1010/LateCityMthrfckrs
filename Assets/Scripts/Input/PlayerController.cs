using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Luminosity.IO
{
    //Handles ALL in-game input.
    public class PlayerController : MonoBehaviour
    {
        public bool GamePad;

        //type of character selected for this level
        GameObject selectedCharacter_Prefab;

        //references to current vehicle or current player
        BasicVehicle curVehicle;
        BasicRider curRider;
        Rigidbody curRagdoll;

        //reference to camera
        GameObject mainCamera;

        //references to audio sources
        public AudioSource playerSource;
        public AudioSource musicSource;
        public AudioSource ambientSource;

        //states
        private enum PlayerState { Vehicle = 0, Rider, Dead };
        private PlayerState curState;

        //Brady: This Bool is used to see what type of car exit it is (button press<true> or collision<false>).
        private bool buttonLaunch = true;

        //Brady: Attempt to fix leaping from forward or reverse.
        //Likely a better approach...
        private bool forwardMovement;
        private bool recentDirection;
        private float previousVelocity;

        //nick: needed for dunkey spin move
        private Vector3 prevLStickInput;
        private float keyboard_A_Leniency;
        private float keyboard_S_Leniency;
        private float keyboard_D_Leniency;
        private float keyboard_W_Leniency;
        private float keyboard_AS_Leniency;
        private float keyboard_SD_Leniency;
        private float keyboard_DW_Leniency;
        private float keyboard_WA_Leniency;

        // Use this for initialization
        void Start()
        {
            //forwardMovement = true;
            //recentDirection = true;
            //previousVelocity = 0;
        }

        // IMPORTANT: This should never be fixed update. Input always needs to be in normal update, or you constantly drop inputs.
        // TLDR: Use Update() for input and FixedUpdate() for motors/movement
        void Update()
        {
            //update vehicle or rider respectively, depending on state.
            switch (curState)
            {
                //process input for vehicle
                case PlayerState.Vehicle:
                    Speedometer.ShowSpeed(curVehicle.transform.GetComponent<Rigidbody>().velocity.magnitude, 0, 100);
                    if (curVehicle.GetComponent<AudioSource>().isPlaying == false)
                    {
                        SoundScript.PlaySound(curVehicle.GetComponent<AudioSource>(), "Engine");
                    }
                    //input horizontal movement
                    curVehicle.inputHorz(InputManager.GetAxis("Horizontal"));

                    //input accelleration
                    curVehicle.inputAccel(InputManager.GetAxis("Accelerate"));

                    //// Brady: Attempt to fix bug where player jumps despite direction they're moving. 
                    ////Now moving forward and speeding up or slowing down = forward jump.
                    ////Now moving backward and speeding up or slowing down = backward jump.
                    //if (InputManager.GetAxis("Accelerate") == 1)
                    //{
                    //    if (curVehicle.transform.GetComponent<Rigidbody>().velocity.magnitude > previousVelocity)
                    //    {
                    //        recentDirection = true;
                    //        forwardMovement = true;
                    //    }
                    //    else
                    //    {
                    //        if (InputManager.GetAxis("Horizontal") == -1 || InputManager.GetAxis("Horizontal") == 1)
                    //        {
                    //            if(recentDirection)
                    //            {
                    //                forwardMovement = true;
                    //            }
                    //            else
                    //            {
                    //                forwardMovement = false;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            forwardMovement = false;
                    //        }
                    //    }
                    //}

                    //else if (InputManager.GetAxis("Accelerate") == -1)
                    //{
                    //    if (curVehicle.transform.GetComponent<Rigidbody>().velocity.magnitude > previousVelocity)
                    //    {
                    //        recentDirection = false;
                    //        forwardMovement = false;
                    //    }
                    //    else
                    //    { 
                    //        if (InputManager.GetAxis("Horizontal") == -1 || InputManager.GetAxis("Horizontal") == 1)
                    //        {
                    //            if (recentDirection)
                    //            {
                    //                forwardMovement = true;
                    //            }
                    //            else
                    //            {
                    //                forwardMovement = false;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            forwardMovement = true;
                    //        }
                    //    }
                    //}
                    //previousVelocity = curVehicle.transform.GetComponent<Rigidbody>().velocity.magnitude;

                    //input jump
                    if (InputManager.GetButtonDown("Jump"))
                    {
                        if (!curVehicle.isSpinMoveHop())
                        {
                            ExitVehicle();
                        }
                        break;
                    }

                    //Brady: vehicle destroyed in crash. Should eventually destroy vehicle.
                    if (curVehicle.GetComponent<BasicVehicle>().broken)
                    {
                        buttonLaunch = false;
                        ExitVehicle();
                        // Destroy(curVehicle);
                        break;
                    }

                    if(!curVehicle.GetComponent<BasicVehicle>().WheelsOnGround)
                    {
                        if (InputManager.GetAxis("Accelerate") > 0)
                            curVehicle.transform.Rotate(Vector3.right * 2f);
                        else if (InputManager.GetAxis("Accelerate") < 0)
                            curVehicle.transform.Rotate(-Vector3.right * 2f);
                        if (InputManager.GetAxis("Horizontal") > 0)
                            curVehicle.transform.Rotate(-Vector3.forward * 1.2f);
                        else if (InputManager.GetAxis("Horizontal") < 0)
                            curVehicle.transform.Rotate(Vector3.forward * 1.2f);
                    }
                    //dunkey spin move
                    if(GamePad)
                    {
                        Vector3 newLStickInput = new Vector3(InputManager.GetAxis("Horizontal"), 0, InputManager.GetAxis("Vertical"));
                        if (curVehicle.isSpinMoveHop() && (newLStickInput - prevLStickInput).magnitude > 0.9f)
                        {
                            curVehicle.startSpinMove((newLStickInput - prevLStickInput).normalized);
                        }
                    }
                    else
                    {
                        Vector3 newLStickInput = new Vector3(InputManager.GetAxis("Horizontal"), 0, InputManager.GetAxis("Vertical"));

                        bool validSpinMove = false;

                        if (InputManager.GetAxis("Horizontal") < 0)
                        {
                            keyboard_A_Leniency = 0.3f;

                            if(keyboard_D_Leniency > 0)
                            {
                                validSpinMove = true;
                                keyboard_D_Leniency = 0;
                            }
                        }
                        if (InputManager.GetAxis("Horizontal") > 0)
                        {
                            keyboard_D_Leniency = 0.3f;

                            if (keyboard_A_Leniency > 0)
                            {
                                validSpinMove = true;
                                keyboard_A_Leniency = 0;
                            }
                        }
                        if (InputManager.GetAxis("Vertical") < 0)
                        {
                            keyboard_S_Leniency = 0.3f;

                            if (keyboard_W_Leniency > 0)
                            {
                                validSpinMove = true;
                                keyboard_W_Leniency = 0;
                            }
                        }
                        if (InputManager.GetAxis("Vertical") > 0)
                        {
                            keyboard_W_Leniency = 0.3f;

                            if (keyboard_S_Leniency > 0)
                            {
                                validSpinMove = true;
                                keyboard_S_Leniency = 0;
                            }
                        }
                        //////////////////////
                        if (InputManager.GetAxis("Horizontal") < 0 && InputManager.GetAxis("Vertical") < 0)
                        {
                            keyboard_AS_Leniency = 0.3f;

                            if (keyboard_DW_Leniency > 0)
                            {
                                validSpinMove = true;
                                keyboard_DW_Leniency = 0;
                            }
                        }
                        if (InputManager.GetAxis("Horizontal") > 0 && InputManager.GetAxis("Vertical") < 0)
                        {
                            keyboard_SD_Leniency = 0.3f;

                            if (keyboard_WA_Leniency > 0)
                            {
                                validSpinMove = true;
                                keyboard_WA_Leniency = 0;
                            }
                        }
                        if (InputManager.GetAxis("Horizontal") > 0 && InputManager.GetAxis("Vertical") > 0)
                        {
                            keyboard_DW_Leniency = 0.3f;

                            if (keyboard_AS_Leniency > 0)
                            {
                                validSpinMove = true;
                                keyboard_AS_Leniency = 0;
                            }
                        }
                        if (InputManager.GetAxis("Horizontal") < 0 && InputManager.GetAxis("Vertical") > 0)
                        {
                            keyboard_WA_Leniency = 0.3f;

                            if (keyboard_SD_Leniency > 0)
                            {
                                validSpinMove = true;
                                keyboard_SD_Leniency = 0;
                            }
                        }


                        if (curVehicle.isSpinMoveHop() && validSpinMove)
                        {
                            curVehicle.startSpinMove(newLStickInput);
                        }


                        keyboard_A_Leniency -= Time.deltaTime;
                        keyboard_D_Leniency -= Time.deltaTime;
                        keyboard_S_Leniency -= Time.deltaTime;
                        keyboard_W_Leniency -= Time.deltaTime;
                        keyboard_AS_Leniency -= Time.deltaTime;
                        keyboard_SD_Leniency -= Time.deltaTime;
                        keyboard_DW_Leniency -= Time.deltaTime;
                        keyboard_WA_Leniency -= Time.deltaTime;
                        }
                    


                    mainCamera.SendMessage("ChangeFocus", curVehicle.transform);
                    mainCamera.SendMessage("ChangeDistance", 10f);
                    break;

                //process input for air movement
                case PlayerState.Rider:

                    ambientSource.volume = curRider.GetComponent<Rigidbody>().velocity.magnitude / 100;

                    if(curRider.vehicleToEnter() != null)
                    {
                        EnterVehicle(curRider.vehicleToEnter());
                        break;
                    }
                    
                    if(curRider.checkRagdoll() != null)
                    {

                        //SoundScript.PlaySound(GetComponent<AudioSource>(), "Death");
                        SoundScript.PlaySound(playerSource, "Death");
                        GameObject.Find("HUD").GetComponent<ButtonScripts>().GameOver();

                        curRagdoll = curRider.checkRagdoll().transform.GetComponent<RagdollStorage>().rb;
                        curState = PlayerState.Dead;
                        curRider.destroyThis();
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

                    mainCamera.SendMessage("ChangeFocus", curRider.transform);
                    mainCamera.SendMessage("ChangeDistance", 10f);
                    break;

                case PlayerState.Dead:
                    mainCamera.SendMessage("ChangeFocus", curRagdoll.transform);
                    mainCamera.SendMessage("ChangeDistance", 5f);
                    break;

                default:
                    Debug.Log("ERROR - NO RIDER OR VEHICLE");
                    break;
            }


            prevLStickInput = new Vector3(InputManager.GetAxis("Horizontal"), 0, InputManager.GetAxis("Vertical"));

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
            if (curState != PlayerState.Dead)
            {
                curState = PlayerState.Vehicle;
                curVehicle = newVehicle;

                if (curRider != null)
                {
                    curVehicle.initializeSpeed(curRider.calculateNewCarMaxSpeed(), curRider.calculateNewCarStartSpeed(), InputManager.GetButton("BreakIn"));
                    curRider.destroyThis();
                }
                else
                {
                    curVehicle.initializeSpeed(0, 0, InputManager.GetButton("BreakIn"));
                }

                keyboard_A_Leniency = 0;
                keyboard_D_Leniency = 0;
                keyboard_S_Leniency = 0;
                keyboard_W_Leniency = 0;
                keyboard_AS_Leniency = 0;
                keyboard_SD_Leniency = 0;
                keyboard_DW_Leniency = 0;
                keyboard_WA_Leniency = 0;
            }
            else
            {
                Debug.Log("big no no error");
            }
           
        }

        //Brady: Added if statement to determine physics of launch. For the time being, the beginCarJump variable for carspeed is simply the car magnitude divided by 5.
        public void ExitVehicle()
        {
            //SoundScript.PlaySound(GetComponent<AudioSource>(), "Jump");

            curVehicle.GetComponent<AudioSource>().Stop();
            SoundScript.PlaySound(playerSource, "Jump");
            Debug.Log("EXIT CAR " + buttonLaunch);
            if (curState != PlayerState.Dead)
            {
                curState = PlayerState.Rider;

                if (curVehicle != null)
                {
                    //let go of steering wheel
                    curVehicle.inputHorz(0);
                    curVehicle.inputAccel(0);

                    

                    if (buttonLaunch)
                    {
                        //spawn rider above car.
                        curRider = Instantiate(selectedCharacter_Prefab, curVehicle.transform.position + Vector3.up * 3.5f, Quaternion.Euler(0, curVehicle.transform.eulerAngles.y, 0)).GetComponent<BasicRider>();
                    }
                    else
                    {
                        //spawn rider above car.
                        curRider = Instantiate(selectedCharacter_Prefab, curVehicle.transform.position + Vector3.up * 3.5f, Quaternion.Euler(0, curVehicle.transform.eulerAngles.y, 0)).GetComponent<BasicRider>();

                    }
                    curRider.externalStart(mainCamera.transform);
                    curRider.beginCarJump(curVehicle.returnExitVelocity(), curVehicle.returnExitMaxSpeed(), buttonLaunch);
                }
                else
                {
                    //spawn rider at prefab coordinates.
                    curRider = Instantiate(selectedCharacter_Prefab, selectedCharacter_Prefab.transform.position, selectedCharacter_Prefab.transform.rotation).GetComponent<BasicRider>();

                    curRider.externalStart(mainCamera.transform);
                }

                curVehicle = null;
            }
            else
            {
                Debug.Log("big no no error");
            }


        }
    }
}
