using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts
{
    public enum SpeakerColor
    {
        Default = 0,
        Pink = 1,
        Blue = 2
    }
    
    public static class SpeakerColorMappings
    {
        private static readonly SpeakerColorMapping[] Mappings = new SpeakerColorMapping[]
        {
            new SpeakerColorMapping {color = SpeakerColor.Pink, colorValue = new Color(1f, 0.47f, 0.78f)},
            new SpeakerColorMapping {color = SpeakerColor.Blue, colorValue = new Color(0.45f, 0.77f, 1f)} ,
            new SpeakerColorMapping {color = SpeakerColor.Default, colorValue = Color.white}
        };

        public static Color GetColor(SpeakerColor color)
        {
            foreach (var mapping in Mappings)
            {
                if (mapping.color == color)
                    return mapping.colorValue;
            }
            return Color.white;

        }
    }

    [Serializable]
    public class SpeakerColorMapping
    {
        [SerializeField] public SpeakerColor color;
        [SerializeField] public Color colorValue;
    }
}