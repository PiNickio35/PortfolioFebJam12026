using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Add win checker

public class ChecklistManager : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private TextMeshProUGUI[] tasks;
    
    [Header("Animation Parameters")]
    [SerializeField] private int slideSpeed;
    [SerializeField] private int outY;
    private bool listVisible = false;
    private Vector3 slideDir;
    
    void Update() {
        AnimateChecklist();
    }

    public void ToggleChecklist(InputAction.CallbackContext ctx) 
    {
        if (listVisible) {
            listVisible = false;
            slideDir = Vector3.down;
        }
        else {
            listVisible = true;
            slideDir = Vector3.up;
        }
    }

    public void CheckOff(int taskId) 
    {
        tasks[taskId].fontStyle = FontStyles.Strikethrough;
    }

    void AnimateChecklist()
    {
        if ((listVisible && transform.localPosition.y < 0) || (!listVisible && transform.localPosition.y > outY))
        {
            transform.Translate(slideDir * (Screen.height * slideSpeed * Time.deltaTime));
        }
    }
}
