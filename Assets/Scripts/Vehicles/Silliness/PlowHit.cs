using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlowHit : MonoBehaviour {
    private void OnCollision(Collision collision)
    {
        if(collision.rigidbody != null)
            collision.rigidbody.AddForce(transform.forward * 100);
    }
}
