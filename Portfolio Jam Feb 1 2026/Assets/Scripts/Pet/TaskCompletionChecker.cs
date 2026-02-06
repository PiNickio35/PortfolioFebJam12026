using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TaskCompletionChecker : MonoBehaviour {
    [Header("Reaction Parameters")]
    [SerializeField] private float throwForce;
    private int frustration = 0;
    
    [Header("Task Items References")]
    [SerializeField] private GameObject[] correctFoods;
    [SerializeField] private GameObject litterbox;
    [SerializeField] private GameObject bed;
    
    [Header("References")]
    [SerializeField] private PetAI petAi;
    [SerializeField] private Animator anim;
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private Complications  complications;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
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
        petAi.currentState = PetAI.State.Ragdoll;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        // Drop and look at food
        interactionController.DropObject();
        foodCol.enabled = false;
        foodRb.useGravity = false;
        foodRb.constraints = RigidbodyConstraints.FreezeAll;
        
        transform.DOLookAt(foodObject.transform.position, 0.5f);
        yield return new WaitForSeconds(0.5f);
        
        // Eat food
        anim.SetBool("isEating", true);
        yield return new WaitForSeconds(1f);
        foodObject.SetActive(false);
        
        // Realign
        petAi.AlignAgentWithModel();
        anim.SetBool("isEating", false);
        yield return new WaitForSeconds(1f);
        rb.constraints =  RigidbodyConstraints.None;
        petAi.currentState = PetAI.State.Idle;
        StartCoroutine(checklistManager.ShowCheckoff(1));
    }
}
