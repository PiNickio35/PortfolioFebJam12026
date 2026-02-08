using System.Collections;
using DG.Tweening;
using Interactions;
using UnityEngine;
using UnityEngine.AI;

public class PetInteraction : BaseInteractable {
    [SerializeField] private PetAI petAi;
    [SerializeField] private ChecklistManager checklistManager;
    
    protected internal bool canPickUp = true;
    
    public override void Interact(InteractionController interactor)
    {
        if (interactor.heldObject == null && canPickUp)
        {
            if (!checklistManager.isChecked[0]) { StartCoroutine(checklistManager.ShowCheckoff(0)); }
            if (petAi.hiding && !checklistManager.isChecked[2]) { StartCoroutine(checklistManager.ShowCheckoff(2)); }
            
            transform.DOKill();
            interactor.PickUpObject(transform.gameObject);
        }
        else if (canPickUp)
        {
            canPickUp = false;
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
        canPickUp = true;
        petAi.currentState = PetAI.State.Idle;
    }
}
