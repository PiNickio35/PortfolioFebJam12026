namespace Interactions
{
    public class PickUpInteraction : BaseInteractable
    {
        
        public override void Interact(InteractionController interactor)
        {
            if (interactor.heldObject == null)
            {
                interactor.PickUpObject(transform.gameObject);
            }
            else
            {
                interactor.DropObject();
            }
        }
    }
}
