using UnityEngine;

namespace Game.Scripts
{
    public class InspectableHotspot : MonoBehaviour
    {
        [SerializeField] private InspectionData data;

        public InspectionData Data => data;
    }
}