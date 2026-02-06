using System.Collections;
using DG.Tweening;
using Interactions;
using UnityEngine;
using UnityEngine.AI;

public class PetInteraction : BaseInteractable {
    [SerializeField] private PetAI petAi;
    
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
        return Physics.Raycast(transform.position, Vector3.down, 0.8f);
    }

    IEnumerator QueueAgentReenable()
    {
        bool isGrounded = false;
        while (!isGrounded)
        {
            isGrounded = IsGrounded();
            yield return new WaitForSeconds(1f);
        }
        
        petAi.AlignAgentWithModel();
        yield return new WaitForSeconds(3f);
        petAi.currentState = PetAI.State.Idle;
    }
}
