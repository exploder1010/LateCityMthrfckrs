using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour
{
    public bool moving = false;
    public Vector3 Destination, Original;
    public float StallingTime, CrushTime, CrushDelay, CrushSpeed;
    public float direction = 1;

    void Start()
    {
        Original = transform.position;
        Destination = transform.position;
        StartCoroutine(Delay());
    }

    public void FixedUpdate()
    {
        if (moving == true)
        {
            transform.Translate(Vector3.up * direction * CrushSpeed * Time.deltaTime);
            Destination = transform.position;
            //if (Vector3.Distance(transform.position, Destination) < 0.1f)
            //{
            //    transform.position = Destination;
            //    moving = false;
            //    if (Original != Destination)
            //    {
            //        Destination = Original;
            //    }
            //    else
            //    {
            //        Destination.y = Destination.y + PistonDistance;
            //    }
            //    direction = direction * -1;
            //    StartCoroutine(StayPut());
            //}
        }
    }

    IEnumerator StayPut()
    {
        yield return new WaitForSeconds(StallingTime);
        moving = true;
        StartCoroutine(Crush());
    }

    IEnumerator Crush()
    {
        yield return new WaitForSeconds(CrushTime);
        moving = false;
        if(direction == -1)
        {
            transform.position = Original;
        }
        direction *= -1;
        StartCoroutine(StayPut());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(CrushDelay);
        moving = true;
        StartCoroutine(Crush());
    }
}
