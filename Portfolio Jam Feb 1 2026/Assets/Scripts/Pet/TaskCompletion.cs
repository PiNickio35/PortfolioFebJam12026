using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TaskCompletion : MonoBehaviour {
    [Header("Reaction Parameters")]
    [SerializeField] private float throwForce;
    private int frustration = 0;
    
    [Header("Task Items References")]
    [SerializeField] private GameObject[] correctFoods;
    [SerializeField] private GameObject litterbox;
    [SerializeField] private GameObject candyPoop;
    [SerializeField] private GameObject bed;
    
    [Header("References")]
    [SerializeField] private PetAI petAi;
    [SerializeField] private PetInteraction petInteraction;
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private Complications  complications;
    [SerializeField] private Animator anim;
    [SerializeField] private Collider petCollider;
    
    void CheckIfComplicationApplies()
    {
        switch (frustration) {
            case 1:
                break;
            case 2:
                break;
            case 4:
                if (!complications.lostEye.activeInHierarchy) { complications.LoseEye(); }
                break;
            case 6:
                Debug.Log("Death");
                break;
        }
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == litterbox && checklistManager.canPoop && interactionController.heldObject == gameObject)
        {
            Debug.Log("Pooping");
            StartCoroutine(PoopAnimation());
        }

        if (col.gameObject == bed && checklistManager.canSleep && interactionController.heldObject == gameObject)
        {
            Debug.Log("Sleeping");
            StartCoroutine(SleepAnimation());
        }
        
        if (interactionController.heldObject != col.gameObject) return;
        if (correctFoods.Contains(col.gameObject))
        {
            Debug.Log("Correct food given");
            StartCoroutine(EatAnimation(col));
        }
        else
        {
            Debug.Log("Wrong food given");
            interactionController.DropObject();
            col.gameObject.GetComponent<Rigidbody>().AddForce(-(transform.position - col.gameObject.transform.position).normalized * throwForce,  ForceMode.Impulse);
            frustration++;
        }
        
        CheckIfComplicationApplies();
    }

    IEnumerator EatAnimation(Collider foodCol)
    {
        GameObject foodObject = foodCol.gameObject;
        Rigidbody foodRb = foodObject.GetComponent<Rigidbody>();
        petAi.currentState = PetAI.State.Standby;
        petInteraction.canPickUp = false;
        
        // Drop and look at food
        interactionController.DropObject();
        foodCol.enabled = false;
        foodRb.useGravity = false;
        foodRb.constraints = RigidbodyConstraints.FreezeAll;
        
        transform.DOLookAt(foodObject.transform.position, 0.5f);
        yield return new WaitForSeconds(0.5f);
        
        // Eat food
        anim.SetBool("isEating", true);
        yield return new WaitForSeconds(0.25f);
        foodObject.SetActive(false);
        
        // Realign
        yield return new WaitForSeconds(2f);
        petAi.AlignAgentWithModel();
        yield return new WaitForSeconds(2f);
        anim.SetBool("isEating", false);
        petAi.currentState = PetAI.State.Idle;
        petInteraction.canPickUp = true;
        checklistManager.canPoop = true;
        StartCoroutine(checklistManager.ShowCheckoff(1));
    }

    IEnumerator PoopAnimation()
    {
        interactionController.DropObject();
        petAi.currentState = PetAI.State.Standby;
        petInteraction.canPickUp = false;
        
        // Hover over bowl and poop
        transform.DORotateQuaternion(litterbox.transform.rotation, 1f);
        transform.DOMove(litterbox.transform.position + new Vector3(0, 0.15f, 0), 1f);
        yield return new WaitForSeconds(2f);
        candyPoop.SetActive(true);
        yield return new WaitForSeconds(1f);
        
        // Move off of bowl
        transform.DOMove(transform.position + transform.forward * 2 - new Vector3(0, 0.15f, 0), 1f);
        yield return new WaitForSeconds(2f);
        
        // Realign
        petAi.AlignAgentWithModel();
        yield return new WaitForSeconds(2f);
        petAi.currentState = PetAI.State.Idle;
        petInteraction.canPickUp = true;
        StartCoroutine(checklistManager.ShowCheckoff(3));
    }

    IEnumerator SleepAnimation()
    {
        interactionController.DropObject();
        petAi.currentState = PetAI.State.Standby;
        petCollider.enabled = false;
        petInteraction.canPickUp = false;
        
        transform.DORotateQuaternion(bed.transform.rotation, 1f);
        transform.DOMove(bed.transform.position, 1f);
        anim.SetBool("isYawning", true);
        yield return new WaitForSeconds(4f);
        anim.SetBool("isYawning", false);
        
        // End game
        StartCoroutine(checklistManager.ShowCheckoff(4));
    }
}
