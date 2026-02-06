using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class PetAI : MonoBehaviour 
{
    public enum State
    {
        Idle,
        Event,
        Ragdoll
    }
    public State currentState;
    
    [Header("Idling Parameters")]
    [SerializeField] private int randomMoveRadius;
    [SerializeField] private float randomMoveDelay;
    bool idling;

    [Header("Animation Parameters")]
    [SerializeField] private Animator anim;
    private bool inEvent = false;
    
    [Header("References")]
    public NavMeshAgent agent;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject player;
    private Rigidbody modelRb;

    void Awake()
    {
        modelRb = model.GetComponent<Rigidbody>();
    }
    
    void Start() 
    {
        currentState = State.Idle;
    }

    void Update()
    {
        CheckPickUp();
        ProcessState();

        anim.SetBool("isMoving", agent.velocity.magnitude > 0);
        
        if (!idling) return;
        model.transform.position = transform.position;
        model.transform.rotation = transform.rotation;
    }

    void CheckPickUp() 
    {
        if (idling && model.transform.parent != gameObject.transform)
        {
            currentState = State.Ragdoll;
        }
        
        if (idling && currentState != State.Idle)
        {
            idling = false;
        }
    }

    void ProcessState()
    {
        switch (currentState)
        {
            case State.Idle:
                if (!agent.enabled) { agent.enabled = true; modelRb.constraints = RigidbodyConstraints.None; }
                if (idling) return;
                StartCoroutine(SetRandomTarget());
                break;
            case State.Event:
                if (inEvent) return;
                int ranEvent = Random.Range(0, 3);
                ranEvent = 0;       // TODO: Remove this when more events added
                agent.enabled = false;
                switch (ranEvent)
                {
                    case 0:
                        StartCoroutine(AnimateFloat());
                        break;
                }
                break;
            case State.Ragdoll:
                agent.enabled = false;
                modelRb.constraints = RigidbodyConstraints.None;
                break;
        }
    }

    public void AlignAgentWithModel()
    {
        modelRb.constraints = RigidbodyConstraints.FreezeAll;
        agent.Warp(model.transform.position);
        model.transform.SetParent(transform);
        model.transform.DORotateQuaternion(transform.transform.rotation, 1f);
        model.transform.DOMove(transform.transform.position, 1f);
    }
    
    IEnumerator AnimateFloat()
    {
        inEvent = true;
        modelRb.constraints =  RigidbodyConstraints.FreezeRotation;
        
        model.transform.DOMove(model.transform.position + new Vector3(0, 1, 0), 5);
        yield return new WaitForSeconds(15);
        modelRb.constraints =  RigidbodyConstraints.None;
        model.transform.DOKill();
        yield return new WaitForSeconds(3);
        
        AlignAgentWithModel();
        yield return new WaitForSeconds(3f);
        currentState = State.Idle;
        inEvent = false;
    }
    
    IEnumerator SetRandomTarget()
    {
        idling = true;
        while (idling) 
        {
            yield return new WaitForSeconds(randomMoveDelay);
            bool isRandomEvent = Random.Range(0, 20) == 1;
            if (!idling) { break; }
            if (isRandomEvent) { currentState = State.Event; break; }

            bool stayStill = Random.Range(0, 3) == 1;
            if (stayStill) continue;
            
            Vector3 randomDir = Random.insideUnitSphere * randomMoveRadius;
            randomDir += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDir, out hit, randomMoveRadius, NavMesh.AllAreas);
            Vector3 finalTarget = hit.position;
            
            transform.DOLookAt(finalTarget, 0.25f);
            agent.SetDestination(finalTarget);
        }
    }
}
