using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour {

    public static AudioClip jumpSound;
    public static AudioClip abilitySound;
    public static AudioClip caridleSound;
    public static AudioClip atmosphericSound;
    public static AudioClip uiSound;
    public static AudioClip winSound;
    public static AudioClip deathSound;

    public static AudioSource atmosphericSource;
    public static AudioSource playerSource;
    public static AudioSource musicSource;

    public static void PlayJump()
    {
        playerSource.PlayOneShot(jumpSound);
    }
    public static void PlayAbility()
    {
        playerSource.PlayOneShot(abilitySound);
    }
    public static void PlayUI()
    {
        playerSource.PlayOneShot(uiSound);
    }
    public static void PlayWin()
    {
        playerSource.PlayOneShot(winSound);
    }
    public static void PlayDeath()
    {
        playerSource.PlayOneShot(deathSound);
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
