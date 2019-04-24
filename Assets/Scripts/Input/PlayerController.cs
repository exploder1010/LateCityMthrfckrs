﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        GameObject prevVehicle;
        float prevVehicleIntangibility;
        BasicRider curRider;
        Rigidbody curRagdoll;

        public GameObject boostEffectPrefab;
        GameObject curBoostEffect;
        bool prevBoostEffect;

        bool win;

        //reference to camera
        CameraController mainCamera;

        //references to audio sources
        public AudioSource playerSource;
        public AudioSource ambientSource;

        //states
        private enum PlayerState { Vehicle = 0, Rider, Dead, Win, Spawning };
        private PlayerState curState;

        //Brady: This Bool is used to see what type of car exit it is (button press<true> or collision<false>).
        private bool buttonLaunch = true;

        //Brady: Attempt to fix leaping from forward or reverse.
        //Likely a better approach...
        private bool forwardMovement;
        private bool recentDirection;
        private float previousVelocity;

        //flexing on the haters @ dell 
        public float comboTimer = 0f;
        float comboTimeSet = 3f;
        public int comboMultiplier = 0;
        float comboDistance = 50f;
        Vector3 prevComboPosition;
        ButtonScripts comboBS;
        bool prevBroken;

        float deathTimer = 4f;

        GameObject floorHolder;
        float floorPerson = -500;
        float floorCar = -525;
        // Use this for initialization
        void Start()
        {
            Time.timeScale = 1f;

            //The ever present HUD is called as a gameobject, and the position of the floor that causes rider death is recorded.
            //The car death floor is set to 25 units below this, so that after hitting the point the player doesn't jump out.
            floorHolder = GameObject.Find("HUD");
            floorPerson = floorHolder.GetComponent<timerScript>().StageFloor;
            floorCar = floorPerson - 25;

            //forwardMovement = true;
            //recentDirection = true;
            //previousVelocity = 0;
        }

        public void CleanDestroy()
        {
            //Debug.Log("cleandestroy");
            if (curBoostEffect)
            {
                Destroy(curBoostEffect);

            }
            if (curRagdoll)
            {
                Destroy(curRagdoll.gameObject);
            }
            if (curRider)
            {
                Destroy(curRider.gameObject);
            }
            Destroy(this);
        }

        // IMPORTANT: This should never be fixed update. Input always needs to be in normal update, or you constantly drop inputs.
        // TLDR: Use Update() for input and FixedUpdate() for motors/movement
        void Update()
        {
            if(comboBS == null && GameObject.Find("HUD") && GameObject.Find("HUD").GetComponent<ButtonScripts>())
            {
                //Debug.Log("found");
                comboBS = GameObject.Find("HUD").GetComponent<ButtonScripts>(); //.comboUpdate(comboTimer, comboTimeSet, comboMultiplier);
            }
            //Debug.Log("cm " + comboMultiplier);
            //combo stuff
            if(comboBS)
                comboBS.comboUpdate(comboTimer, comboTimeSet, comboMultiplier);

            if (curState != PlayerState.Rider)
            {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("VisibleWhenPlaying"))
                {
                    go.SetActive(false);
                }
            }

            //update vehicle or rider respectively, depending on state.
            switch (curState)
            {
                //process input for vehicle
                case PlayerState.Vehicle:

                    //---------------------------------------------------------speed
                    //Speedometer.ShowSpeed(curVehicle.transform.GetComponent<Rigidbody>().velocity.magnitude, 0, 100);

                    //speed > boost threshold
                    if(curVehicle.transform.GetComponent<Rigidbody>().velocity.magnitude > curVehicle.returnActualMaxSpeed() * selectedCharacter_Prefab.GetComponent<BasicRider>().boostThreshold )
                    {
                        if (!prevBoostEffect)
                        {
                            prevBoostEffect = true;
                            curBoostEffect = Instantiate(boostEffectPrefab, curVehicle.transform.position, curVehicle.transform.rotation);
                            //mainCamera.ChangeDistance(15, 2f);
                        }

                        curBoostEffect.transform.position = curVehicle.transform.position;
                        curBoostEffect.transform.rotation = curVehicle.transform.rotation;

                    }
                    else
                    {
                        if (curBoostEffect != null)
                        {
                            prevBoostEffect = false;
                            //mainCamera.ChangeDistance(10f, 2f);
                            Destroy(curBoostEffect);
                        }
                    }

                    //---------------------------------------------------------sound
                    if (curVehicle.GetComponent<AudioSource>().isPlaying == false)
                    {
                        SoundScript.PlaySound(curVehicle.GetComponent<AudioSource>(), "Engine");
                    }

                    //-------------------------------------------------------input:
                    //input horizontal movement
                    curVehicle.inputHorz(InputManager.GetAxis("Horizontal"));

                    float v_input = InputManager.GetAxis("Vertical") == 0 ? InputManager.GetAxis("Accelerate") : InputManager.GetAxis("Vertical");
                    
                    //Debug.Log("1 " + ((v_input / Mathf.Abs(v_input)) * 0.5f) + " 2 " + (v_input * 0.5f) + " 3 " + (((v_input / Mathf.Abs(v_input)) * 0.5f) + (v_input * 0.5f)));

                    //(v_input == 0) ? 0.5f : v_input
                    //input accelleration
                    curVehicle.inputAccel(InputManager.GetAxis("Vertical") == 0 ? InputManager.GetAxis("Accelerate") : InputManager.GetAxis("Vertical"));

                    //input vertical movement
                    curVehicle.inputVert(InputManager.GetAxis("Vertical"));

                    //input jump
                    if (InputManager.GetButtonDown("Jump") || curVehicle.GetComponent<BasicVehicle>().broken || curVehicle.transform.position.y < floorPerson - 25)
                    {
                        if (curVehicle.GetComponent<BasicVehicle>().broken)
                        {
                            prevBroken = true;
                            if (comboBS)
                                comboBS.comboEnd(comboMultiplier);
                            comboMultiplier = 0;
                            comboTimer = 0;
                        }
                        else
                        {
                            prevBroken = false;
                        }

                        ExitVehicle();

                        break;
                    }

                    break;

                //process input for air movement
                case PlayerState.Rider:
                    if (curRider.cashoutcombo)
                    {

                        CashOutCombo();
                    }

                    //If player has died, whether by ragdolling or being lower than an arbitrary threshold provided by the variable deadFloor.
                    if (curRider.checkRagdoll() != null || curRider.transform.position.y < floorPerson)
                    {
                        if (Time.timeScale == 1)
                        {
                            SoundScript.PlaySound(playerSource, "Death");

                            if (comboBS)
                                comboBS.GameOver();

                            curState = PlayerState.Dead;

                            if(curRider.checkRagdoll() == null)
                            {
                                curRider.spawnRagdoll();
                            }
                            curRagdoll = curRider.checkRagdoll().transform.GetComponent<RagdollStorage>().rb;

                            mainCamera.ChangeFocus(curRagdoll.transform, -1);
                            //mainCamera.ChangeDistance(6f, 2f);
                            curRider.destroyThis();
                        }

                        //drop combo before cashing in for death
                        comboMultiplier = 0;
                        comboTimer = 0;
                        if (comboBS)
                            comboBS.comboEnd(comboMultiplier);

                        break;
                    }
                    else
                    {
                        //only countdown combo in air
                        if (comboTimer > 0)
                        {
                            comboTimer -= Time.deltaTime;
                            if (comboTimer <= 0)
                            {
                                if (comboBS)
                                    comboBS.comboEnd(comboMultiplier);
                                comboTimer = 0;
                                comboMultiplier = 0;
                            }
                        }

                        //ENTER VEHICLE
                        if (prevVehicleIntangibility > 0)
                        {
                            prevVehicleIntangibility -= Time.deltaTime;

                            if (curRider.curAbilityAmmo == 0 || curRider.rb.velocity.y <= -50)
                            {
                                prevVehicleIntangibility = 0;
                            }
                            if (prevVehicleIntangibility < 0)
                            {
                                prevVehicleIntangibility = 0;
                            }
                        }
                        //maybe ut this back curRider.vehicleToEnter().gameObject != prevVehicle || 
                        if (curRider.vehicleToEnter() != null && curState != PlayerState.Dead && (prevVehicleIntangibility <= 0 || curRider.vehicleToEnter().gameObject != prevVehicle))
                        {
                            EnterVehicle(curRider.vehicleToEnter());
                            //if(curRider.veh)
                            //curState != PlayerState.Dead && (newVehicle.gameObject != prevVehicle || (newVehicle.easyCheckWheelsOnGround() && newVehicle.getGravity() == Vector3.down) || prevVehicleIntangibility <= 0)
                            break;
                        }
                        else
                        {
                            curRider.rejectVehicleToEnter();
                        }

                        //ambience
                        if (curRider)
                        {
                            ambientSource.volume = 0;//forever.S curRider.GetComponent<Rigidbody>().velocity.magnitude / 100;
                        }

                        //win
                        if (!win && curRider.goalCollider.collidersCount() > 0)
                        {
                            win = true;

                            if (comboBS && comboMultiplier != 0)
                            {

                                comboBS.comboEnd(comboMultiplier);
                            }
                            comboMultiplier = 0;
                            comboTimer = 0;

                            GameObject.FindGameObjectWithTag("HUD").GetComponent<timerScript>().setWin();


                            curRider.goalCollider.returnColliders()[0].transform.root.Find("shatter1").gameObject.SetActive(true);
                            curRider.goalCollider.returnColliders()[0].gameObject.SetActive(false);
                            curState = PlayerState.Win;
                            mainCamera.ChangeFocus(curRider.transform, 1);
                            mainCamera.SetCameraPosition(curRider.transform.position + curRider.transform.forward * 1.3f + curRider.transform.up * 1.1f);

                            //curRider.off = true;
                            Rigidbody killrb = curRider.rb;
                            Destroy(curRider);
                            killrb.isKinematic = true;
                            killrb.velocity = Vector3.zero;

                            SoundScript.PlaySound(playerSource, "Win");
                            if (comboBS)
                            {
                                comboMultiplier = 0;
                                comboBS.comboEnd(comboMultiplier);
                                comboBS.Win(GameObject.FindGameObjectWithTag("HUD").GetComponent<timerScript>().timeRemaining > 0);
                            }
                        }


                        //input horizontal movement
                        curRider.inputHorz(InputManager.GetAxis("Horizontal"));

                        //float v_input = InputManager.GetAxis("Vertical") == 0 ? InputManager.GetAxis("Accelerate") : InputManager.GetAxis("Vertical");


                        //input vertical movement
                        curRider.inputVert(InputManager.GetAxis("Vertical") == 0 ? InputManager.GetAxis("Accelerate") : InputManager.GetAxis("Vertical"));

                        //input fastfall
                        if (InputManager.GetButtonDown("FastFall"))
                        {
                            curRider.inputFastFall(1);
                            SoundScript.PlaySound(playerSource, "Ability");
                        }

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


                    }


                    break;

                case PlayerState.Dead:

                    updateReset();

                    break;

                default:
                    //Debug.Log("ERROR - NO RIDER OR VEHICLE");
                    break;
            }

        }

        void updateReset()
        {
            if(win != true)
            {
                if (deathTimer > 2f)
                {

                    deathTimer -= Time.deltaTime;
                }
                if (curRagdoll && curRagdoll.velocity.magnitude <= 3f)
                {

                    deathTimer -= Time.deltaTime;
                }
                else if (deathTimer <= 2f)
                {

                    deathTimer -= Time.deltaTime  * 0.5f;
                }

                if (deathTimer <= 0)
                {

                    GameController.instance.resetScene();

                    //deathTimer = 4f;
                }
            }
        
        }

        public void SetCamera(CameraController newCamera)
        {
            mainCamera = newCamera;
        }

        public void SelectRider(GameObject newCharacter)
        {
            selectedCharacter_Prefab = newCharacter;
        }
        
        public void SetSpawnState(GameObject focus)
        {
            curState = PlayerState.Spawning;
            mainCamera.ChangeFocus(focus.transform, 2);
        }

        void CashOutCombo()
        {
            if (comboBS && comboMultiplier != 0)
            {

                comboBS.comboEnd(comboMultiplier);
                SoundScript.PlaySound(playerSource, "Combo Cashout");  
            }
            comboMultiplier = 0;
            comboTimer = 0;
        }

        public void EnterVehicle(BasicVehicle newVehicle)
        {
            //Debug.Log("ENTER CAR " + Time.time);
            if (curState != PlayerState.Dead)
            {

                //successful combo
                if (comboTimer > 0 && (prevComboPosition - newVehicle.transform.position).magnitude >= comboDistance && newVehicle.gameObject != prevVehicle)
                {
                    comboMultiplier++;
                    comboTimer = comboTimeSet;
                    //SoundScript.PlaySound(playerSource, "Combo Add");
                }
                //start new combo
                else if ((prevComboPosition - newVehicle.transform.position).magnitude >= comboDistance && !prevBroken && newVehicle.gameObject != prevVehicle)
                {
                    if (comboBS)
                        comboBS.comboStart();
                    //SoundScript.PlaySound(playerSource, "Combo Add");
                    comboMultiplier = 1;
                    comboTimer = comboTimeSet;
                }
                //drop combo b/c too close
                else
                {
                    CashOutCombo();
                }

               

                curState = PlayerState.Vehicle;
                curVehicle = newVehicle;
                curVehicle.player = true;

                if (curRider != null)
                {

                    //curVehicle.initializeSpeed(0, 0, InputManager.GetButton("BreakIn"));
                    curVehicle.initializeSpeed(curRider.calculateNewCarMaxSpeed(), curRider.calculateNewCarStartSpeed(), false);
                    curRider.destroyThis();
                }
                else
                {
                    curVehicle.initializeSpeed(0, 0, false);
                }

                mainCamera.ChangeFocus(curVehicle.transform, 0);
                //mainCamera.ChangeDistance(10f, 2f);

                //set position for combo
                if (prevComboPosition != Vector3.zero )
                    prevComboPosition = curVehicle.transform.position;
            }
            else
            {
                Debug.Log("big no no error");
            }

            newVehicle = null;
        }

        //Brady: Added if statement to determine physics of launch. For the time being, the beginCarJump variable for carspeed is simply the car magnitude divided by 5.
        public void ExitVehicle()
        {
            var randomJump = Random.Range(0, 2);


            // Replace the sound clip string in the second case when someone finds a second jump sound
            curVehicle.GetComponent<AudioSource>().Stop();
            if(randomJump == 0)
            {
                SoundScript.PlaySound(playerSource, "Jump");
            }
            else if (randomJump == 1)
            {
                SoundScript.PlaySound(playerSource, "Jump");
            }


            //Debug.Log("EXIT CAR " + buttonLaunch);
            if (curState != PlayerState.Dead)
            {
               

                curState = PlayerState.Rider;

                if (curVehicle != null)
                {
                    curVehicle.transform.GetComponent<Rigidbody>().velocity *= 0.5f;
                    curVehicle.player = false;

                    //let go of steering wheel
                    curVehicle.inputHorz(0);
                    curVehicle.inputAccel(0);

                    Vector3 newUp = Vector3.up;
                    if (curVehicle.easyCheckWheelsOnGround() && curVehicle.gravityDirection.normalized != Vector3.down)
                    {
                        newUp = curVehicle.transform.up;
                    }


                    //spawn rider above car.
                    curRider = Instantiate(selectedCharacter_Prefab, curVehicle.transform.position + newUp * 5f, Quaternion.Euler(0, curVehicle.transform.eulerAngles.y, 0)).GetComponent<BasicRider>();

                    curRider.externalStart(mainCamera.transform);
                    curRider.beginCarJump(curVehicle.returnExitVelocity(), curVehicle.returnActualMaxSpeed(), newUp, curVehicle.easyCheckWheelsOnGround());
                }
                else
                {
                    //spawn rider at prefab coordinates.
                    curRider = Instantiate(selectedCharacter_Prefab, selectedCharacter_Prefab.transform.position, selectedCharacter_Prefab.transform.rotation).GetComponent<BasicRider>();

                    curRider.externalStart(mainCamera.transform);
                }

                mainCamera.ChangeFocus(curRider.transform, 0);
                //mainCamera.ChangeDistance(12f, 2f);

                prevVehicle = curVehicle.gameObject;
                prevVehicleIntangibility = 1f;
                curRider.setPreviousVehicle(curVehicle.gameObject);
                curVehicle = null;

                foreach (GameObject go in GameObject.FindGameObjectsWithTag("VisibleWhenPlaying"))
                {
                    go.SetActive(true);
                }

            }
            else
            {
                Debug.Log("big no no error");
            }


        }
    }
}
