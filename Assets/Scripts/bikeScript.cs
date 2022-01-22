using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bikeScript : MonoBehaviour
{
    Vector3 destination;
    NavMeshAgent bike;
    bool stopped = false;

    // Start is called before the first frame update
    void Start()
    {
        bike = GetComponent<NavMeshAgent>();
        destination = bike.destination;
    }

    // Update is called once per frame
    void Update()
    {
        if(!stopped) {
            Vector3 target= new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
            if (Vector3.Distance(destination, target) > 1.0f)
            {
                destination = target;
                bike.destination = destination;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        bike.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
        stopped = true;
        if (collision.gameObject.tag == "Car") {
            Debug.Log("Collision with a car detected");
            List<GameObject> listcars = new List<GameObject>(GameObject.FindGameObjectsWithTag("Car"));
            if (!listcars.Contains(collision.gameObject)) {
                listcars.Add(collision.gameObject);
            }
        }

        else if (collision.gameObject.tag == "Ambulance") {
            Debug.Log("Collision with ambulance not implemented");
        } else {
            Debug.Log("Collision object does not have a valid tag");
        }
    }
}
