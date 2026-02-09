using UnityEngine;

namespace Pet
{
    public class Complications : MonoBehaviour {
        [Header("References")]
        [SerializeField] protected internal GameObject lostEye;
    
        public void LoseEye()
        {
            lostEye.SetActive(true);
        }
    }
}
