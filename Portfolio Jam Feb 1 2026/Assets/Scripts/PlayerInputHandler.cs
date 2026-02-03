using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")] 
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")] 
    [SerializeField] private string playerActionMapName = "Player";

    [SerializeField] string uiActionMapName = "UI";
    
    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string checkList = "CheckList";
    
    [Header("Script References")]
    [SerializeField] private ChecklistManager checkListManager;
    
    private InputAction _movementAction;
    private InputAction _rotationAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _interactAction;
    private InputAction _checkListAction;
    
    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool CheckListTriggered { get; private set; }

    private void Awake()
    {
        InputActionMap playerMapReference = playerControls.FindActionMap(playerActionMapName);
        InputActionMap uiMapReference = playerControls.FindActionMap(uiActionMapName);
        
        _movementAction = playerMapReference.FindAction(movement);
        _rotationAction = playerMapReference.FindAction(rotation);
        _jumpAction = playerMapReference.FindAction(jump);
        _sprintAction = playerMapReference.FindAction(sprint);
        _interactAction = playerMapReference.FindAction(interact);
        _checkListAction = uiMapReference.FindAction(checkList);
        
        SubscribeActionValuesToInputEvents();
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(playerActionMapName).Enable();
    }

    private void OnDisable()
    {
        playerControls.FindActionMap(playerActionMapName).Disable();
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
        
        _checkListAction.performed += ctx => checkListManager.ToggleChecklist(ctx);
    }
}
