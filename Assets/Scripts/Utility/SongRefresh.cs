using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Luminosity.IO
{
    public class SongRefresh : MonoBehaviour
    {

        private GameObject MusicSource;
        public Text SongTitle;

        // Use this for initialization
        void Start()
        {
            MusicSource = GameObject.Find("Music Source");
        }

        // Update is called once per frame
        void Update()
        {
            if (MusicSource.GetComponent<AudioSource>().isPlaying)
            {
                SongTitle.text = (MusicSource.GetComponent<MusicShuffler>().TrackNum + ".  " + MusicSource.GetComponent<AudioSource>().clip.name);
            }
        }
    }
}
