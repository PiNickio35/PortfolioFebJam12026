using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChecklistManager : MonoBehaviour 
{
    [Header("Animation Parameters")]
    [SerializeField] private int slideSpeed;
    [SerializeField] private int outY;
    private bool listVisible = false;
    private Vector3 slideDir;
    
    [Header("CheckOff Conditions")]
    protected internal bool canPoop = false;
    protected internal bool canSleep = false;
    
    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private UIMenuController uiMenuController;
    [SerializeField] private GameObject prompt;
    [SerializeField] private TextMeshProUGUI[] tasks;
    protected internal bool[] isChecked;

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
        if (listVisible) 
        {
            listVisible = false;
            slideDir = Vector3.down;
        }
        else {
            if (prompt.activeInHierarchy) { prompt.SetActive(false); }
            listVisible = true;
            slideDir = Vector3.up;
        }
    }

    void CheckOff(int taskId) 
    {
        tasks[taskId].fontStyle = FontStyles.Strikethrough;
        isChecked[taskId] = true;
        
        if (canPoop && taskId == 3)
        {
            canPoop = false;
        }
        
        else
        {
            int numChecked = isChecked.Count(check => check);
            if (numChecked == tasks.Length - 2)
            {
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

    public IEnumerator ShowCheckoff(int taskId)
    {
        if (isChecked[taskId]) yield break;
        if (prompt.activeInHierarchy) { prompt.SetActive(false); }
        listVisible = true;
        slideDir = Vector3.up;
        yield return new WaitForSeconds(1f);
        CheckOff(taskId);
        yield return new WaitForSeconds(1f);
        listVisible = false;
        slideDir = Vector3.down;
        yield return new WaitForSeconds(2f);
        if (isChecked[tasks.Length - 1])
        {
            uiMenuController.ChangeWinStateUI(true);
        }
    }
}
