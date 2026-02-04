using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PetAI : MonoBehaviour 
{
    public enum State
    {
        Idle,
        Event,
        Pet,
        PickedUp
    }
    public State currentState;
    
    [Header("Idling Parameters")]
    [SerializeField] private int randomMoveRadius;
    [SerializeField] private float randomMoveDelay;
    bool idling; 
    
    [Header("References")]
    public NavMeshAgent agent;
    [SerializeField] private GameObject model;

    void Start() 
    {
        agent.updateRotation = false;
        currentState = State.Idle;
    }

    void Update()
    {
        CheckState();
        ProcessState();    
    }

    void CheckState() 
    {
        // Pick up checking
        if (idling && model.transform.parent != gameObject.transform)
        {
            currentState = State.PickedUp;    
        }
        else if (!idling && model.transform.parent == gameObject.transform)
        {
            currentState = State.Idle;
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
                if (!agent.enabled) { agent.enabled = true; }
                if (idling) return;
                StartCoroutine(SetRandomTarget());
                break;
            case State.Event:
            case State.Pet:
            case State.PickedUp:
                agent.enabled = false;
                break;
        }
    }
    
    IEnumerator SetRandomTarget()
    {
        idling = true;
        while (idling) 
        {
            yield return new WaitForSeconds(randomMoveDelay);
            if (!idling) { break; }
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
