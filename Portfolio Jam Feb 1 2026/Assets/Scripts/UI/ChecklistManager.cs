using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChecklistManager : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private TextMeshProUGUI[] tasks;
    private bool[] isChecked;
    
    [Header("Animation Parameters")]
    [SerializeField] private int slideSpeed;
    [SerializeField] private int outY;
    private bool listVisible = false;
    private Vector3 slideDir;

    protected bool canSleep = false;
    protected bool canPoop = true;

    void Start()
    {
        isChecked = new bool[tasks.Length];
    }
    
    void Update()
    {
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

    void CheckOff(int taskId) 
    {
        tasks[taskId].fontStyle = FontStyles.Strikethrough;
        isChecked[taskId] = true;
        
        if (canPoop && taskId == 3) {
            // TODO: Poop animation
            canPoop = false;
        }
        
        if (isChecked[tasks.Length - 1]) {
            // TODO: What happens when you win
            Debug.Log("Player won!");
        }
        else {
            int numChecked = isChecked.Count(check => check);
            if (numChecked == tasks.Length - 1) {
                canSleep = true;
            }
        }
    }

    void AnimateChecklist()
    {
        if ((listVisible && transform.localPosition.y < 0) || (!listVisible && transform.localPosition.y > outY))
        {
            transform.Translate(slideDir * (Screen.height * slideSpeed * Time.deltaTime));
        }
    }

    public IEnumerator ShowCheckoff(int taskId) {
        if (isChecked[taskId]) yield break;
        listVisible = true;
        slideDir = Vector3.up;
        yield return new WaitForSeconds(1f);
        CheckOff(taskId);
        yield return new WaitForSeconds(1f);
        listVisible = false;
        slideDir = Vector3.down;
    }
}
