using UnityEngine;

namespace Interactions
{
    public class BaseInteractable : MonoBehaviour, IInteractable
    {
        private Outline _outline;
        
        private void Awake()
        {
            _outline = gameObject.AddComponent<Outline>();
            _outline.OutlineMode = Outline.Mode.OutlineVisible;
            _outline.OutlineColor = Color.yellow;
            _outline.OutlineWidth = 10f;
            _outline.enabled = false;
        }

        public virtual bool CanInteract()
        {
            return true;
        }

        public virtual void Interact(InteractionController interactor)
        {
            throw new System.NotImplementedException();
        }

        public void OnFocusEnter()
        {
            _outline.enabled = true;
        }

        public void OnFocusExit()
        {
            _outline.enabled = false;
        }
    }
}
