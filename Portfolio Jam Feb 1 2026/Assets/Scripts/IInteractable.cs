using UnityEngine;

public interface IInteractable
{
    public bool CanInteract();
    public void Interact(InteractionController interactor);
    void OnFocusEnter();
    void OnFocusExit();
}
