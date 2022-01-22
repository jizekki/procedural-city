using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class collisionBike : MonoBehaviour
{

    public NavMeshAgent nma;

    // Start is called before the first frame update
    void Start()
    {
        nma = GetComponent<NavMeshAgent>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Car") {
          //Debug.Log("Collision");
            List<GameObject> listcars = GetComponent<carScript>().map.targetCars;
            if (!listcars.Contains(collision.gameObject)) {
                listcars.Add(collision.gameObject);
            }

            if(nma != null) {
                nma.isStopped = true;
                List<GameObject> listbikes = GetComponent<carScript>().map.targetBikes;
                if (!listbikes.Contains(gameObject)){
                    listbikes.Add(gameObject);
                }
            //Debug.Log("azeaze");
            }
        }

        // Ajouter Ambulance
    }
}
