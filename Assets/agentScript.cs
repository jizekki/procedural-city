using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class agentScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

      //Instantiate(car, new Vector3(0.0f, 0, 0.0f), Quaternion.identity);
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        agent.destination = new Vector3(2f, 0, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
