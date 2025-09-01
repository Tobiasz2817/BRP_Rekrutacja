using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace Services
{
    public static class MouseService
    {
        static Vector2 _lastPosition;
        
        public static Action OnPositionChange;
        
        public static void Tick()
        {
            if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
            {   
                if (Cursor.lockState == CursorLockMode.Locked)
                    EnableMouse();
                
                OnPositionChange?.Invoke();
                _lastPosition =  Mouse.current.position.ReadValue();
            }
        }
        
        public static void EnableMouse()
        {
            Cursor.lockState = CursorLockMode.None;
            Mouse.current.WarpCursorPosition(_lastPosition);
        }
        
        public static void DisableMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}