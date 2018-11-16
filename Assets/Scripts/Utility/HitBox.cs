using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    private List<Collider> colliders = new List<Collider>();
    public enum HitBoxType { Road = 0, Vehicle, BrokenVehicle, Wall };
    public HitBoxType Type;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collide");
        if(other.transform.root != transform.root)
        {
            switch (Type)
            {
                case HitBoxType.Road:
                    if (other.gameObject.layer == LayerMask.NameToLayer("Road"))
                    {
                        //Debug.Log("Collide 2");
                        colliders.Add(other);
                    }
                    break;
                case HitBoxType.Vehicle:
                    if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle") && other.transform.root.transform.GetComponent<BasicVehicle>() && !other.transform.root.transform.GetComponent<BasicVehicle>().broken)
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
                case HitBoxType.Wall:
                    if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        colliders.Add(other);
                    }
                    break;
                default:
                    //Debug.Log("Collide 3");
                    break;
            }
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
