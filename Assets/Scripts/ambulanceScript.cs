using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ambulanceScript : MonoBehaviour
{


    NavMeshAgent ambulance;
    bool hasMission = false;


    // Start is called before the first frame update
    void Start()
    {
        ambulance = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasMission) {
            List<GameObject> listbikes = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bike"));
            foreach (GameObject bike in listbikes)
            {
                if(bike.GetComponent<bikeScript>().stopped) { 
                    hasMission = true;
                    Debug.Log("Ambulance has now a mission !");
                    ambulance.destination = bike.gameObject.transform.position;
                    break;
                }
            }
        }
        if(!hasMission) {
            // If no mission is found, a random destination is assigned
            ambulance.destination = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
        }
    }
}
