using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class PickUpInteraction : BaseInteractable
    {
        [SerializeField] private AudioSource audioSource;
        bool canPlayAudio = false;

        void Start()
        {
            StartCoroutine(QueueSoundEnable());
        }
        
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

        void OnCollisionEnter(Collision collision) {
            if (canPlayAudio) { audioSource.Play(); }
        }

        IEnumerator QueueSoundEnable()
        {
            yield return new WaitForSeconds(3);
            canPlayAudio = true;
        }
    }
}
