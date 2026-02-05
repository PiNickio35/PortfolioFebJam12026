using System;
using System.Linq;
using UnityEngine;

public class TaskCompletionChecker : MonoBehaviour {
    [Header("Reaction Parameters")]
    [SerializeField] private float throwForce;
    
    [Header("References")]
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private GameObject[] correctFoods;
    [SerializeField] private GameObject litterbox;
    [SerializeField] private GameObject bed;
    
    void OnTriggerEnter(Collider col)
    {
        if (interactionController.heldObject != col.gameObject) return;
        
        if (correctFoods.Contains(col.gameObject))
        {
            Debug.Log("Correct food given");
            interactionController.DropObject();
            StartCoroutine(checklistManager.ShowCheckoff(1));
        }
        else
        {
            Debug.Log("Wrong food given");
            interactionController.DropObject();
            col.gameObject.GetComponent<Rigidbody>().AddForce(-(transform.position - col.gameObject.transform.position).normalized * throwForce,  ForceMode.Impulse);
        }
    }
}
