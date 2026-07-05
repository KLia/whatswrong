using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts
{
    public static class PointerInput
    {
        public static bool TryGetPosition(out Vector2 position)
        {
            Pointer pointer = Pointer.current;
            if (pointer == null)
            {
                position = default;
                return false;
            }

            position = pointer.position.ReadValue();
            return true;
        }

        public static bool WasPressedThisFrame()
        {
            Pointer pointer = Pointer.current;
            return pointer != null && pointer.press.wasPressedThisFrame;
        }
    }
}
