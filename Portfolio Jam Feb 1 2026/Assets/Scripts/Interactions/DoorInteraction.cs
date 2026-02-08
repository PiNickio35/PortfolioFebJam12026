using DG.Tweening;
using UnityEngine;

namespace Interactions
{
    public class DoorInteraction : BaseInteractable
    {
        [Header("Door Settings")]
        [SerializeField] private Vector3 openRotation = new(0f, -100f, 0f);
        [SerializeField] private Vector3 closeRotation = new(0f, 0f, 0f);
        [SerializeField] private float rotationSpeed = 3f;
        
        private bool _isOpen;

        public override void Interact(InteractionController interactor)
        {
            transform.DORotate(_isOpen ? closeRotation : openRotation, rotationSpeed);

            _isOpen = !_isOpen;
        }
    }
}
