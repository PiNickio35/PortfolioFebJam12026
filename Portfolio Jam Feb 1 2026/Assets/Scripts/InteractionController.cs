using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("Pick Up Settings")] 
    [SerializeField] private Transform holdArea;

    [Header("Physics Parameters")] 
    [SerializeField] private float interactionRange = 5.0f;
    [SerializeField] private float pickUpForce = 150.0f;
    
    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    
    private Rigidbody _heldObjectRigidBody;
    private bool _isClick;
    private IInteractable _lastHitObject;
    
    public GameObject heldObject;

    private void Update()
    {
        if (heldObject != null)
        {
            MoveObject();
            
            if (playerInputHandler.InteractTriggered != _isClick)
            {
                _isClick = playerInputHandler.InteractTriggered;
                if (_isClick)
                {
                    _lastHitObject.Interact(this);
                }
            }
        }
        else if (DoInteractionTest(out IInteractable interactable))
        {
            if (interactable.CanInteract())
            {
                InteractableInSight(interactable);
                if (playerInputHandler.InteractTriggered != _isClick)
                {
                    _isClick = playerInputHandler.InteractTriggered;
                    if (_isClick)
                    {
                        interactable.Interact(this);
                    }
                }
            }
            else
            {
                InteractableOutOfSight(interactable);
            }
        }
    }

    private void InteractableInSight(IInteractable interactable)
    {
        interactable.OnFocusEnter();
    }

    private void InteractableOutOfSight(IInteractable interactable)
    {
        interactable.OnFocusExit();
    }

    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit,
                interactionRange))
        {
            interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && interactable == _lastHitObject)
            {
                return true;
            }
            if (interactable != null && _lastHitObject != null)
            {
                _lastHitObject.OnFocusExit();
                _lastHitObject = interactable;
                return true;
            }
            if (interactable != null)
            {
                _lastHitObject = interactable;
                return true;
            }
        }

        _lastHitObject?.OnFocusExit();
        _lastHitObject = null;
        return false;
    }
    
    public void PickUpObject(GameObject pickedUpObject)
    {
        if (!pickedUpObject.GetComponent<Rigidbody>()) return;
        _heldObjectRigidBody = pickedUpObject.GetComponent<Rigidbody>();
        _heldObjectRigidBody.useGravity = false;
        _heldObjectRigidBody.linearDamping = 10; 
        _heldObjectRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        _heldObjectRigidBody.transform.parent = holdArea;
        heldObject = pickedUpObject;
    }
    
    public void DropObject()
    {
        _heldObjectRigidBody.useGravity = true;
        _heldObjectRigidBody.linearDamping = 1; 
        _heldObjectRigidBody.constraints = RigidbodyConstraints.None;
        heldObject.transform.parent = null;
        heldObject = null;
    }

    private void MoveObject()
    {
        if (!(Vector3.Distance(heldObject.transform.position, holdArea.position) > 0.1f)) return;
        Vector3 moveDirection = (holdArea.position - heldObject.transform.position).normalized; 
        _heldObjectRigidBody.AddForce(moveDirection * pickUpForce);
    }
}
