using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    private List<Collider> colliders = new List<Collider>();
    public string layer;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(layer))
        {
            colliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (colliders.Contains(other))
        {
            colliders.Remove(other);
        }
    }

    public int collidersCount()
    {
        return colliders.Count;
    }

    public List<Collider> returnColliders()
    {
        return colliders;
    }
}
