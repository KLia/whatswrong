using UnityEngine;

namespace Game.Scripts
{
    public class InspectionTrigger : MonoBehaviour
    {
        [SerializeField] private InspectionData data;

        public InspectionData Data => data;
    }
}