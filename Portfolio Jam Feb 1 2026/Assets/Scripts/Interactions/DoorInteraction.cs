using DG.Tweening;
using UnityEngine;

namespace Interactions
{
    public class DoorInteraction : BaseInteractable
    {
        [Header("Door Settings")]
        [SerializeField] private Vector3 targetRotation = new(0f, -100f, 0f);
        [SerializeField] private float rotationSpeed = 3f;
        
        private bool _isOpen;

        public override void Interact(InteractionController interactor)
        {
            if (_isOpen)
            {
                transform.DORotate(-targetRotation, rotationSpeed, RotateMode.WorldAxisAdd);
            }
            else
            {
             transform.DORotate(targetRotation, rotationSpeed, RotateMode.WorldAxisAdd);   
            }
            
            _isOpen = !_isOpen;
        }
    }
}
