using UnityEngine;

namespace Game.Scripts
{
    public class InspectionDatabase : MonoBehaviour
    {
        [SerializeField] private InspectionData[] entries;

        public InspectionData GetById(string objectId)
        {
            foreach (var entry in entries)
            {
                if (entry != null && entry.id == objectId)
                    return entry;
            }

            return null;
        }
    }
}