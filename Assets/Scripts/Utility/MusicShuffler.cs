using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicShuffler : MonoBehaviour {

	AudioSource source;
	List<AudioClip> Tracks;
	List<AudioClip> q;
	int qindex = 0;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		Object[] t = Resources.LoadAll("Music", typeof(AudioClip));
		Tracks = new List<AudioClip>();
		foreach(Object o in t)
			Tracks.Add((AudioClip) o);
		Shuffle();
	}
	
	// Update is called once per frame
	void Update () {
		if(!source.isPlaying){
			qindex = (qindex>=q.Count-1) ? 0 : qindex+1;
			source.PlayOneShot(q[qindex]);
		}
	}

	void Shuffle(){
		q = new List<AudioClip>();
		List<AudioClip> Remaining = new List<AudioClip>(Tracks);
		while(Remaining.Count > 0){
			int rnd = Random.Range(0, Remaining.Count);
			q.Add(Remaining[rnd]);
			Remaining.RemoveAt(rnd);
		}
		
	}
}
