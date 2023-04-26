using UnityEngine;
using UnityEngine.AI;

// [RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class RootMotionAgentController: MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    [SerializeField] Transform target;
    [SerializeField] float maxSpeed = 1.5f;
    [SerializeField] float angleToTarget;

    enum animState { TurningLeft, TurningRight, Walking};
    animState currentState;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Don’t update position automatically

        agent.updateRotation = false;
        agent.updatePosition = false;
        
    }

    void Update()
    {
        Vector3 directionToTarget = target.position - transform.position;
        angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);

        if(angleToTarget < 90)
        {
            
            if (currentState != animState.Walking)
            {
                Debug.Log("Walking");
                anim.SetTrigger("Walk");
                agent.updateRotation = true;
                currentState = animState.Walking;
            }
            
        }
        else if(angleToTarget > 90 && currentState!=animState.TurningRight)
        {
            Debug.Log("Turning Right");
            anim.SetTrigger("TurnRight");
            agent.updateRotation = false;
            currentState = animState.TurningRight;
        }
        else if (angleToTarget < 90 && currentState != animState.TurningLeft)
        {
            Debug.Log("Turning Left");
            anim.SetTrigger("TurnLeft");
            agent.updateRotation = false;
            currentState = animState.TurningLeft;
        }

        doMovement();



        // Transform look at will be replaced by logic that determines the signed angle from his forward direction to the target direction
        // If his angle is greater than a certain amount, have him turn. If angle is less than that same amount, have him walk. 
        // If his angle is negative, then run anim.settrigger('turn left')
        // When his angle becomes 0, allow velocity.y to be > 0.
        // transform.LookAt(target.position);
        // GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;



    }

    void doMovement()
    {
        agent.destination = target.position;
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        velocity.y = Mathf.Min(velocity.y, maxSpeed);

        // bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;
        // navmesh agent stopping distance used instead

        // Update animation parameters
        anim.SetFloat("speed", velocity.y);
    }

    void OnAnimatorMove()
    {
        // Update position to agent position
        if(currentState == animState.Walking)
        {
            transform.position = agent.nextPosition;
        }
        
    }
}