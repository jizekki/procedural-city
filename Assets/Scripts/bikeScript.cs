using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bikeScript : MonoBehaviour
{
    NavMeshAgent bike;
    public bool stopped = false;

    // Start is called before the first frame update
    void Start()
    {
        bike = GetComponent<NavMeshAgent>();
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
                Debug.Log("Collision of a bike with an ambulance");
            }
        }
    }
}
