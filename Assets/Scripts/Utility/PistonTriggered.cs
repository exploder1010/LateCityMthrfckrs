using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonTriggered : MonoBehaviour
{
    public bool moving = false;
    public bool activated = false;
    public float CrushTime, CrushSpeed;
    public GameObject Piston;

    public void FixedUpdate()
    {
        if (moving == true)
        {
            Piston.GetComponent<Transform>().transform.Translate(Vector3.up * CrushSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer == LayerMask.NameToLayer("Vehicle") || other.gameObject.layer == LayerMask.NameToLayer("Rider")) && activated == false)
        {
            moving = true;
            activated = true;
            StartCoroutine(Crushing());
        }
    }

    IEnumerator Crushing()
    {
        yield return new WaitForSeconds(CrushTime);
        moving = false;
    }
}