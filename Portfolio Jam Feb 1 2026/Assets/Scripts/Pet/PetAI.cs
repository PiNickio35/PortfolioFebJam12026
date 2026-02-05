using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Sequence = DG.Tweening.Sequence;

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
    bool spinning;
    
    [Header("References")]
    public NavMeshAgent agent;
    [SerializeField] private GameObject model;

    void Start() 
    {
        currentState = State.Idle;
    }

    void Update()
    {
        CheckPickUp();
        ProcessState();

        if (!idling) return;
        model.transform.position = transform.position;
        model.transform.rotation = transform.rotation;
    }

    void CheckPickUp() 
    {
        if (idling && model.transform.parent != gameObject.transform)
        {
            currentState = State.PickedUp;    
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
                if (spinning) return;
                agent.enabled = false;
                StartCoroutine(PlaySpinAnimation());
                break;
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
            // Random event checking
            bool isRandomEvent = Random.Range(0, 20) == 1;
            if (!idling) { break; }
            else if (isRandomEvent) { currentState = State.Event; break; }
            
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

    IEnumerator PlaySpinAnimation()
    {
        // TODO: Maybe use a sequence instead..? This isn't really working
        Debug.Log("Spin animation");
        Vector3 floatHeight = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        spinning = true;
        
        model.transform.SetParent(null);
        
        // model.transform.DOMove(floatHeight, 0.5f);
        // yield return new WaitForSeconds(0.5f);
        // model.transform.DORotate(new Vector3(0, 85, 0), 0.5f).SetLoops(3).SetEase(Ease.Linear);
        // yield return new WaitForSeconds(1.5f);
        
        model.transform.SetParent(transform);
        // model.transform.DOMove(transform.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        // model.transform.DORotate(transform.rotation.eulerAngles, 0.25f);
        currentState = State.Idle;
    }
}
