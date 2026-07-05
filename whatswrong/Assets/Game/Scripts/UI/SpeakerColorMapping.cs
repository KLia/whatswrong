using System;
using UnityEngine;

namespace Game.Scripts
{
    public enum SpeakerColor
    {
        Default = 0,
        Player = 1,
        Daddy = 2
    }
    
    public static class SpeakerColorMapping
    {
        private static readonly Mapping[] Mappings = new Mapping[]
        {
            new Mapping {color = SpeakerColor.Player, colorValue = new Color(1f, 0.47f, 0.78f)},
            new Mapping {color = SpeakerColor.Daddy, colorValue = new Color(0.45f, 0.77f, 1f)} ,
            new Mapping {color = SpeakerColor.Default, colorValue = Color.white}
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
    public class Mapping
    {
        [SerializeField] public SpeakerColor color;
        [SerializeField] public Color colorValue;
    }
}