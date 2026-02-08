using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [Header("Pick Up Settings")] 
    [SerializeField] private Transform holdArea;

    [Header("Physics Parameters")] 
    [SerializeField] private float pickUpRange = 5.0f;
    [SerializeField] private float pickUpForce = 150.0f;
    
    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    
    protected internal GameObject _heldObject;
    private Rigidbody _heldObjectRigidBody;
    private bool _isClick;

    private void Update()
    {
        if (playerInputHandler.InteractTriggered != _isClick)
        {
            _isClick = playerInputHandler.InteractTriggered;
            if (_isClick)
            {
                Debug.Log("Click");
                if (_heldObject == null)
                {
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit,
                            pickUpRange))
                    {
                        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance,
                            Color.yellow);
                        PickUpObject(hit.transform.gameObject);
                    }
                }
                else
                {
                    DropObject();
                }
            }
        }

        if (_heldObject != null)
        {
            MoveObject();
        }
    }

    private void PickUpObject(GameObject pickedUpObject)
    {
        if (pickedUpObject.GetComponent<Rigidbody>())
        {
            _heldObjectRigidBody = pickedUpObject.GetComponent<Rigidbody>();
            _heldObjectRigidBody.useGravity = false;
            _heldObjectRigidBody.linearDamping = 10; // Should be drag so just check
            _heldObjectRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            _heldObjectRigidBody.transform.parent = holdArea;
            _heldObject = pickedUpObject;
        }
    }

    private void MoveObject()
    {
        if (Vector3.Distance(_heldObject.transform.position, holdArea.position) > 0.1f)
        {
            Vector3 moveDirection = (holdArea.position - _heldObject.transform.position).normalized; // Should maybe be normalised
            _heldObjectRigidBody.AddForce(moveDirection * pickUpForce);
        }
    }
    
    protected internal void DropObject()
    {
        _heldObjectRigidBody.useGravity = true;
        _heldObjectRigidBody.linearDamping = 1; // Should be drag so just check
        _heldObjectRigidBody.constraints = RigidbodyConstraints.None;
        _heldObject.transform.parent = null;
        _heldObject = null;
    }
}
