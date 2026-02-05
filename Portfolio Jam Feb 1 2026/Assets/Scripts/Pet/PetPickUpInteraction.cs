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
            transform.DOKill();
            interactor.PickUpObject(transform.gameObject);
        }
        else
        {
            interactor.DropObject();
            StartCoroutine(QueueAgentReenable());
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1);
    }

    IEnumerator QueueAgentReenable()
    {
        bool isGrounded = false;
        while (!isGrounded)
        {
            isGrounded = IsGrounded();
            yield return new WaitForSeconds(1f);
        }
        agent.Warp(transform.position);
        transform.SetParent(parent);
        transform.DORotateQuaternion(parent.transform.rotation, 1f);
        transform.DOMove(parent.transform.position, 1f);
        yield return new WaitForSeconds(1f);
        petAi.currentState = PetAI.State.Idle;
    }
}
