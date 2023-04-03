using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavigation : MonoBehaviour
{
    public Animator anim;
    public bool isWalking;
    private Rigidbody rb;
    Vector3 lastPosition;

    // Start is called before the first frame update
    public Transform goal;
    public float walkThreshold;

    public NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        agent.destination = goal.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = transform.position - lastPosition;
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
        }
        
    }
}
