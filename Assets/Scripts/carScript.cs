using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class carScript : MonoBehaviour
{
    NavMeshAgent car;
    public bool isCriminal;
    
    // Start is called before the first frame update
    void Start()
    {

        //Instantiate(car, new Vector3(0.0f, 0, 0.0f), Quaternion.identity);
        car = GetComponent<NavMeshAgent>();
        isCriminal = false;

    }

    // Update is called once per frame
    void Update()
    {
        // Update destination if the target moves one unit
        if (car.remainingDistance < 1.0f)
        {
            car.destination = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(car != null) {
            if (collision.gameObject.tag == "Bike") {
                isCriminal = true;
            }
            else if (collision.gameObject.tag == "Police" && isCriminal) {
                Destroy(gameObject);
                Debug.Log("Cought by police, proceed to destroy");
            }
        }
    }
}
