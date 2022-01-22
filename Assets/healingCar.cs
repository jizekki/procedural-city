using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healingCar : MonoBehaviour
{
    public VoronoiDemo map;

    private UnityEngine.AI.NavMeshAgent nma;

    // Start is called before the first frame update
    void Start()
    {
        nma = GetComponent<UnityEngine.AI.NavMeshAgent>();
        wanderRandom();
    }

    private void wanderRandom() {
        Vector3 pos = new Vector3(Random.Range(0, 100.0f), 0, Random.Range(0, 100.0f));
        nma.destination = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (map.targetBikes.Count > 0) {
            nma.destination = map.targetBikes[0].transform.position;
        }
        else if (nma.remainingDistance < 0.6) {
            wanderRandom();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(map.targetBikes.Count > 0) {
            if (map.targetBikes[0] == collision.gameObject) {
                map.targetBikes.Remove(collision.gameObject);
                collision.gameObject.GetComponent<collisionBike>().nma.isStopped = false;
                Debug.Log("PIPOOPOPOPP");
            }
        }
    }
}