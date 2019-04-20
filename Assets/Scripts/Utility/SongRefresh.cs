using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongRefresh : MonoBehaviour {

    private AudioSource MusicSource;
    public Text SongTitle;

	// Use this for initialization
	void Start () {
        MusicSource = GameObject.Find("Music Source").GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		if(MusicSource.isPlaying)
        {
            SongTitle.text = MusicSource.clip.name;
        }
	}
}
