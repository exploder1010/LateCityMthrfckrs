using System.Collections;
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
        private enum PlayerState { Vehicle = 0, Rider, Dead };
        private PlayerState curState;

        //Brady: This Bool is used to see what type of car exit it is (button press<true> or collision<false>).
        private bool buttonLaunch = true;

        //Brady: Attempt to fix leaping from forward or reverse.
        //Likely a better approach...
        private bool forwardMovement;
        private bool recentDirection;
        private float previousVelocity;

        //flexing on the haters @ dell 
        float comboTimer = 0f;
        float comboTimeSet = 3f;
        int comboMultiplier = 1;
        float comboDistance = 50f;
        Vector3 prevComboPosition;
        ButtonScripts comboBS;
        bool prevBroken;

        float deathTimer = 4f;

        // Use this for initialization
        void Start()
        {
            Time.timeScale = 1f;
            //forwardMovement = true;
            //recentDirection = true;
            //previousVelocity = 0;
        }

        public void CleanDestroy()
        {
            Debug.Log("cleandestroy");
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
                Debug.Log("found");
                comboBS = GameObject.Find("HUD").GetComponent<ButtonScripts>(); //.comboUpdate(comboTimer, comboTimeSet, comboMultiplier);
            }
            //Debug.Log("cm " + comboMultiplier);
            //combo stuff
            if(comboBS)
                comboBS.comboUpdate(comboTimer, comboTimeSet, comboMultiplier);



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

                    //input accelleration
                    curVehicle.inputAccel(InputManager.GetAxis("Accelerate"));

                    //input vertical movement
                    curVehicle.inputVert(InputManager.GetAxis("Vertical"));

                    //input jump
                    if (InputManager.GetButtonDown("Jump") || curVehicle.GetComponent<BasicVehicle>().broken)
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

                    //only countdown in air
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

                    if (prevVehicleIntangibility > 0)
                    {
                        prevVehicleIntangibility -= Time.deltaTime;
                        if(prevVehicleIntangibility < 0)
                        {
                            prevVehicleIntangibility = 0;
                        }
                    }


                    ambientSource.volume = curRider.GetComponent<Rigidbody>().velocity.magnitude / 100;

                    if(curRider.vehicleToEnter() != null && curState != PlayerState.Dead && (curRider.vehicleToEnter().gameObject != prevVehicle || (curRider.vehicleToEnter().easyCheckWheelsOnGround() && curRider.vehicleToEnter().getGravity() == Vector3.down) || prevVehicleIntangibility <= 0))
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
                    
                    if(curRider.checkRagdoll() != null)
                    {

                        //SoundScript.PlaySound(GetComponent<AudioSource>(), "Death");
                        if (Time.timeScale == 1)
                        {
                            SoundScript.PlaySound(playerSource, "Death");

                            if (comboBS)
                                comboBS.GameOver();

                            curState = PlayerState.Dead;

                            curRagdoll = curRider.checkRagdoll().transform.GetComponent<RagdollStorage>().rb;

                            mainCamera.ChangeFocus(curRagdoll.transform, 0);
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

                    if(!win && curRider.goalCollider.collidersCount() > 0)
                    {
                        win = true;
                        curRider.goalCollider.returnColliders()[0].transform.parent.Find("shatter1").gameObject.SetActive(true);
                        curRider.goalCollider.returnColliders()[0].gameObject.SetActive(false);
                        Time.timeScale = 0.1f;
                        //mainCamera.ChangeFocus(curRider.transform, 1);
                        curRider.off = true;
                        SoundScript.PlaySound(playerSource, "Win");
                        if (comboBS)
                            comboBS.Win();
                    }
                    if (win)
                    {
                        updateReset();
                    }


                    //input horizontal movement
                    curRider.inputHorz(InputManager.GetAxis("Horizontal"));

                    //input vertical movement
                    curRider.inputVert(InputManager.GetAxis("Vertical"));

                    //input fastfall
                    if (InputManager.GetButtonDown("FastFall"))
                    {
                        curRider.inputFastFall(1);
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

                    
                    break;

                case PlayerState.Dead:

                    updateReset();

                    break;

                default:
                    Debug.Log("ERROR - NO RIDER OR VEHICLE");
                    break;
            }

        }

        void updateReset()
        {
            if(Time.timeScale != 0)
            {
                if (deathTimer > 2f)
                {

                    deathTimer -= Time.deltaTime / Time.timeScale;
                }
                if (curRagdoll && curRagdoll.velocity.magnitude <= 3f)
                {

                    deathTimer -= Time.deltaTime / Time.timeScale;
                }
                else if (!curRagdoll && deathTimer <= 2f)
                {

                    deathTimer -= Time.deltaTime / Time.timeScale;
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
        
        public void EnterVehicle(BasicVehicle newVehicle)
        {
            //Debug.Log("ENTER CAR " + Time.time);
            if (curState != PlayerState.Dead)
            {

                //successful combo
                if (comboTimer > 0 && (prevComboPosition - newVehicle.transform.position).magnitude >= comboDistance)
                {
                    comboMultiplier++;
                    comboTimer = comboTimeSet;
                }
                //start new combo
                else if ((prevComboPosition - newVehicle.transform.position).magnitude >= comboDistance && !prevBroken)
                {
                    if (comboBS)
                        comboBS.comboStart();
                    comboMultiplier = 1;
                    comboTimer = comboTimeSet;
                }
                //drop combo b/c too close
                else
                {
                    if (comboBS)
                        comboBS.comboEnd(comboMultiplier);
                    comboMultiplier = 0;
                    comboTimer = 0;
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
            //SoundScript.PlaySound(GetComponent<AudioSource>(), "Jump");

            curVehicle.GetComponent<AudioSource>().Stop();
            SoundScript.PlaySound(playerSource, "Jump");
            //Debug.Log("EXIT CAR " + buttonLaunch);
            if (curState != PlayerState.Dead)
            {

                //set position for combo
                prevComboPosition = curVehicle.transform.position;

                curState = PlayerState.Rider;

                if (curVehicle != null)
                {

                    curVehicle.player = false;

                    //let go of steering wheel
                    curVehicle.inputHorz(0);
                    curVehicle.inputAccel(0);

                    Vector3 newUp = Vector3.up;
                    if (curVehicle.easyCheckWheelsOnGround())
                    {
                        newUp = curVehicle.transform.up;
                    }


                    //spawn rider above car.
                    curRider = Instantiate(selectedCharacter_Prefab, curVehicle.transform.position + newUp * 3.5f, Quaternion.Euler(0, curVehicle.transform.eulerAngles.y, 0)).GetComponent<BasicRider>();

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
                prevVehicleIntangibility = 3f;
                curRider.setPreviousVehicle(curVehicle.gameObject);
                curVehicle = null;
            }
            else
            {
                Debug.Log("big no no error");
            }


        }
    }
}
