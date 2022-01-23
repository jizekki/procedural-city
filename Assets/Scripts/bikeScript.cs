using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bikeScript : MonoBehaviour
{
    NavMeshAgent bike;
    public bool injured;

    // Start is called before the first frame update
    void Start()
    {
        bike = GetComponent<NavMeshAgent>();
        injured = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!injured) {
            if (bike.isStopped || bike.remainingDistance < 1.0f)
            {
                bike.destination = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
            } 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(bike != null) {
            if (collision.gameObject.tag == "Car") {
                injured = true;
                bike.isStopped = true;
                Debug.Log("Collision of a bike with a car detected");
            }
            else if (collision.gameObject.tag == "Ambulance") {
                injured = false;
                collision.gameObject.GetComponent<ambulanceScript>().hasMission = false;
                collision.gameObject.GetComponent<ambulanceScript>().followedBike = null;
                Debug.Log("Collision of a bike with an ambulance");
            }
        }
    }
}
