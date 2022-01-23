using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class policeScript : MonoBehaviour
{

    NavMeshAgent police;
    bool hasMission = false;
    GameObject followedCar;

    // Start is called before the first frame update
    void Start()
    {
        police = GetComponent<NavMeshAgent>();
        followedCar = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasMission) {
            // Police does not have a mission
            List<GameObject> listCars = new List<GameObject>(GameObject.FindGameObjectsWithTag("Car"));
            foreach (GameObject car in listCars)
            {
                if(car.GetComponent<carScript>().isCriminal) { 
                    hasMission = true;
                    Debug.Log("Police has now a mission !");
                    followedCar = car.gameObject;
                    police.destination = followedCar.transform.position;
                    break;
                }
            }
        } else {
            if(followedCar != null) {
                // Police already has a mission, just update the destination to the new position of the followed car
                police.destination = followedCar.transform.position;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(police != null) {
            if (collision.gameObject.tag == "Car" && collision.gameObject == followedCar) {
                hasMission = false;
                followedCar = null;
            }
        }
    }
}
