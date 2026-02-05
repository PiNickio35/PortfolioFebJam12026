using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")] 
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")] 
    [SerializeField] private string actionMapName = "Player";
    
    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string interact = "Interact";
    
    private InputAction _movementAction;
    private InputAction _rotationAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _interactAction;
    
    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }

    private void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);
        
        _movementAction = mapReference.FindAction(movement);
        _rotationAction = mapReference.FindAction(rotation);
        _jumpAction = mapReference.FindAction(jump);
        _sprintAction = mapReference.FindAction(sprint);
        _interactAction = mapReference.FindAction(interact);
        
        SubscribeActionValuesToInputEvents();
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        _movementAction.performed += inputInfo => MovementInput = inputInfo.action.ReadValue<Vector2>();
        _movementAction.canceled += _ => MovementInput = Vector2.zero;
        
        _rotationAction.performed += inputInfo => RotationInput = inputInfo.action.ReadValue<Vector2>();
        _rotationAction.canceled += _ => RotationInput = Vector2.zero;
        
        _jumpAction.started += _ => JumpTriggered = true;
        _jumpAction.canceled += _ => JumpTriggered = false;
        
        _sprintAction.started += _ => SprintTriggered = true;
        _sprintAction.canceled += _ => SprintTriggered = false;
        
        _interactAction.started += _ => InteractTriggered = true;
        _interactAction.canceled += _ => InteractTriggered = false;
    }
}
