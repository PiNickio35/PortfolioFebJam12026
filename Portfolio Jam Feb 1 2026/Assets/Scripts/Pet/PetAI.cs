using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PetAI : MonoBehaviour 
{
    enum State
    {
        Idle,
        Event,
        Pet,
        PickedUp
    }
    State currentState;
    
    [Header("Idling Parameters")]
    [SerializeField] private int randomMoveRadius;
    [SerializeField] private float randomMoveDelay;
    bool idling; 
    
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;

    void Start() 
    {
        currentState = State.Idle;
    }

    void Update()
    {
        ProcessState();    
    }

    void ProcessState()
    {
        switch (currentState)
        {
            case State.Idle:
                if (idling) return;
                StartCoroutine(SetRandomTarget());
                break;
            case State.Event:
            case State.Pet:
            case State.PickedUp:
                break;
        }
    }
    
    IEnumerator SetRandomTarget()
    {
        idling = true;
        while (currentState == State.Idle) 
        {
            yield return new WaitForSeconds(randomMoveDelay);
            bool stayStill = Random.Range(0, 3) == 1;
            if (stayStill) continue;
            Vector3 randomDir = Random.insideUnitSphere * randomMoveRadius;
            randomDir += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDir, out hit, randomMoveRadius, NavMesh.AllAreas);
            Vector3 finalTarget = hit.position;
            agent.SetDestination(finalTarget);
        }

        if (currentState != State.Idle) {
            idling = false;
        }
    }
}
