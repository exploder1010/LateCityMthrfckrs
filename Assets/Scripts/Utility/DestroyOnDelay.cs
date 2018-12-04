using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDelay : MonoBehaviour {
    public float Delay = 1f;

    private void Start()
    {
        Destroy(gameObject, Delay);
    }
}
