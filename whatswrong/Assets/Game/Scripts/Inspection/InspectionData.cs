using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(menuName = "Game/Inspection Data")]
    public class InspectionData : ScriptableObject
    {
        public string id;
        public string title;
        [TextArea] public string description;
        public Sprite image;
    }
}

