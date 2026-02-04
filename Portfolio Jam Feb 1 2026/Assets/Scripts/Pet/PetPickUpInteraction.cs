using System.Collections;
using DG.Tweening;
using Interactions;
using UnityEngine;
using UnityEngine.AI;

public class PetPickUpInteraction : BaseInteractable {
    [SerializeField] private PetAI petAi;
    [SerializeField] private Transform parent;
    [SerializeField] private NavMeshAgent agent;
    
    public override void Interact(InteractionController interactor)
    {
        if (interactor.heldObject == null)
        {
            StopCoroutine(QueueAgentReenable());
            interactor.PickUpObject(transform.gameObject);
        }
        else
        {
            interactor.DropObject();
            StartCoroutine(QueueAgentReenable());
        }
    }

    IEnumerator QueueAgentReenable() {
        yield return new WaitForSeconds(1f);
        agent.Warp(transform.position);
        transform.SetParent(parent);
        transform.DORotate(parent.transform.rotation.eulerAngles, 1f);
        transform.DOMove(parent.transform.position, 1f);
        petAi.currentState = PetAI.State.Idle;
    }
}
