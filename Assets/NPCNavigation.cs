using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavigation : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent agent;

    public bool isWalking;
    
    Vector3 lastPosition;

    public Transform goal;
    public float walkThreshold;

    

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // agent.updatePosition = false;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = goal.position;
        anim.SetFloat("speed", agent.velocity.magnitude);
        Debug.Log(agent.velocity);
        /*Vector3 velocity = transform.position - lastPosition;
        float speed = velocity.magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if (speed > walkThreshold)
        {
            if (!isWalking)
            {
                anim.SetTrigger("Start Walking");
            }
            isWalking = true;
        }
        else
        {
            if (isWalking)
            {
                anim.SetTrigger("Stop Walking");
            }
            isWalking = false;
        */
    }


}


