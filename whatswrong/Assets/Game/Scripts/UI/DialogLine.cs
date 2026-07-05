using System;
using UnityEngine;

namespace Game.Scripts
{
    [Serializable]
    public class DialogLine
    {
        [SerializeField] public string text;
        [SerializeField] public SpeakerColor speakerColor;
    }
}