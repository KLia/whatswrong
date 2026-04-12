using System;
using UnityEngine;

namespace Game.Scripts
{
    [Serializable]
    public class DialogElement
    {
        [SerializeField] public string text;
        [SerializeField] public Color color;
    }
}