using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public static Dictionary<string, AudioClip> sounds;
    
    public List<AudioLink> Clips;


    // Use this for initialization
    void Awake()
    {
        sounds = new Dictionary<string, AudioClip>();
        foreach (AudioLink al in Clips)
        {
            sounds.Add(al.name, al.Clip);
        }
    }

    // Update is called once per frame
    void Update()
    {



    }

    public static void PlaySound(AudioSource source, string clip)
    {
        source.PlayOneShot(sounds[clip]);
    }
}

[System.Serializable]
public struct AudioLink{
    public string name;
    public AudioClip Clip;
 }
