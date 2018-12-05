using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

    void Awake ()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(this.gameObject.tag);
        if (objects.Length > 1)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

}
