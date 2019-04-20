using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongRefresh : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(GameObject.Find("Music Source").GetComponent<AudioSource>().isPlaying)
        {
            //this.GetComponent<Text>().text = AudioClip.name
        }
	}
}
