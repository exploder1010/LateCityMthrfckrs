using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Luminosity.IO
{
    public class MusicShuffler : MonoBehaviour
    {

        public AudioSource source;
        public int TrackNum = 0;
        Object[] Tracks;

        private void Awake()
        {
            Tracks = Resources.LoadAll("Music", typeof(AudioClip));
            source.clip = Tracks[TrackNum] as AudioClip;
        }

        // Use this for initialization
        void Start()
        {
            //source.Play();
        }

        // Update is called once per frame
        void Update()
        {
            if (InputManager.GetButtonDown("RadioBack"))
            {
                print(TrackNum);
                TrackNum -= 1;
                if (TrackNum == 0)
                {
                    TrackNum = 16;
                }
                source.clip = Tracks[TrackNum] as AudioClip;
                print(TrackNum);
                source.Play();
            }

            else if (InputManager.GetButtonDown("RadioForward"))
            {
                TrackNum += 1;
                if(TrackNum == 17)
                {
                    TrackNum = 0;
                }
                source.clip = Tracks[TrackNum] as AudioClip;
                source.Play();
            }

            if (!source.isPlaying)
            {
                print("Not Playing");
                PlayRandom();
            }
        }

        public void PlayRandom()
        {
            source.clip = Tracks[TrackNum = Random.Range(0, Tracks.Length)] as AudioClip;
            source.Play();
        }

        public void SelectSong(int SongChoice)
        {
            source.clip = Tracks[TrackNum = SongChoice] as AudioClip;
            source.Play();
        }
    }
}
