using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    private List<Collider> colliders = new List<Collider>();
    public enum HitBoxType { Road = 0, Vehicle, BrokenVehicle };
    public HitBoxType Type;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        switch (Type)
        {
            case HitBoxType.Road:
                if (other.gameObject.layer == LayerMask.NameToLayer("Road"))
                {
                    colliders.Add(other);
                }
                break;
            case HitBoxType.Vehicle:
                if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
                {
                    colliders.Add(other);
                }
                break;
            case HitBoxType.BrokenVehicle:
                if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle") && other.transform.root.transform.GetComponent<BasicVehicle>() && other.transform.root.transform.GetComponent<BasicVehicle>().broken)
                {
                    colliders.Add(other);
                }
                break;
            default:
                break;
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
