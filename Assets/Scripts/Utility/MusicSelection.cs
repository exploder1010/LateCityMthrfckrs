using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Luminosity.IO
{
    public class MusicSelection : MonoBehaviour
    {
        GameObject Audio;

        void Start()
        {
            Audio = GameObject.Find("Music Source");
        }

        public void BeginSelection(int SongChoice)
        {
            Audio.GetComponent<MusicShuffler>().SelectSong(SongChoice);
        }
    }
}
