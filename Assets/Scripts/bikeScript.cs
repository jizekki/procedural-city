using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bikeScript : MonoBehaviour
{
    NavMeshAgent bike;
    public bool stopped;

    // Start is called before the first frame update
    void Start()
    {
        bike = GetComponent<NavMeshAgent>();
        stopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!stopped) {
            if (bike.remainingDistance < 1.0f)
            {
                bike.destination = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(bike != null) {
            if (collision.gameObject.tag == "Car") {
                stopped = true;
                bike.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                Debug.Log("Collision of a bike with a car detected");
            }
            else if (collision.gameObject.tag == "Ambulance") {
                stopped = false;
                bike.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
                collision.gameObject.GetComponent<ambulanceScript>().hasMission = false;
                collision.gameObject.GetComponent<ambulanceScript>().followedBike = null;
                Debug.Log("Collision of a bike with an ambulance deteceted");
                Debug.Log("Ambuance has ended the mission");
            }
        }
    }
}
