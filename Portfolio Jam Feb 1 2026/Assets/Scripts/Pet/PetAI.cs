using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class PetAI : MonoBehaviour 
{
    public enum State
    {
        Standby,
        Idle,
        Event,
        Ragdoll,
        Play
    }
    public State currentState;
    
    [Header("Idling Parameters")]
    [SerializeField] private int randomMoveRadius;
    [SerializeField] private float randomMoveDelay;
    bool idling;

    [Header("Animation Parameters")]
    [SerializeField] private Animator anim;
    private bool saidHi = false;
    protected internal bool inEvent = false;
    protected internal bool hiding = false;
    
    [Header("References")]
    [SerializeField] private PetInteraction petInteraction;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject player;
    public NavMeshAgent agent;
    [SerializeField] private ParticleSystem whoosh;

    [SerializeField] private GameObject[] hidingSpots;
    private Rigidbody modelRb;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    void Awake()
    {
        modelRb = model.GetComponent<Rigidbody>();
    }
    
    void Start() 
    {
        currentState = State.Standby;
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
        if ((!saidHi || idling || hiding) && model.transform.parent != gameObject.transform)
        {
            currentState = State.Ragdoll;
            saidHi = true;
            hiding = false;
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
            case State.Standby:
                agent.enabled = false;
                modelRb.constraints = RigidbodyConstraints.FreezeAll;
                break;
            case State.Idle:
                if (!agent.enabled)
                {
                    agent.enabled = true;
                    modelRb.constraints = RigidbodyConstraints.None;
                }
                if (idling) return;
                StartCoroutine(SetRandomTarget());
                break;
            case State.Event:
                if (inEvent) return;
                int ranEvent = Random.Range(0, 2);
                petInteraction.canPickUp = false;
                agent.enabled = false;
                switch (ranEvent)
                {
                    case 0:
                        StartCoroutine(AnimateFloat());
                        break;
                    case 1:
                        StartCoroutine(StareAtPlayer());
                        break;
                }
                break;
            case State.Ragdoll:
                agent.enabled = false;
                modelRb.constraints = RigidbodyConstraints.None;
                break;
            case State.Play:
                if (hiding) return;
                hiding = true;
                
                int ranSpot = Random.Range(0, hidingSpots.Length);
                agent.enabled = false;
                modelRb.constraints = RigidbodyConstraints.FreezeAll;
                whoosh.Play();
                model.transform.position = hidingSpots[ranSpot].transform.position;
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
        yield return new WaitForSeconds(8f);
        modelRb.constraints =  RigidbodyConstraints.None;
        model.transform.DOKill();
        yield return new WaitForSeconds(1f);
        
        AlignAgentWithModel();
        yield return new WaitForSeconds(1f);
        currentState = State.Idle;
        petInteraction.canPickUp = true;
        inEvent = false;
    }

    IEnumerator StareAtPlayer()
    {
        inEvent = true;
        modelRb.constraints =  RigidbodyConstraints.FreezePosition;
        
        model.transform.DOLookAt(player.transform.position, 1f);
        audioSource.PlayOneShot(audioClips[Random.Range(1, 3)]);
        anim.SetBool("isYawning", true);
        yield return new WaitForSeconds(2f);
        anim.SetBool("isYawning", false);
        
        AlignAgentWithModel();
        yield return new WaitForSeconds(1f);
        currentState = State.Idle;
        petInteraction.canPickUp = true;
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
            if (stayStill)
            {
                if (Random.Range(0, 2) == 1) audioSource.PlayOneShot(audioClips[0]);
                continue;
            }
            
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
