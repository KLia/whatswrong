using UnityEngine;

namespace Game.Scripts
{
    public class InspectableHotspot : MonoBehaviour
    {
        [SerializeField] private string objectId;

        public string ObjectId => objectId;
    }
}