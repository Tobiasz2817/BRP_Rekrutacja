using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ui
{
    public static class MouseHoverController
    {
        static PointerEventData _pointerEventData;
        static List<RaycastResult> _results;
        
        static Vector2 _lastMousePosition;
        static bool _unSelectOnPointerExit;

        static Selectable _selected;
        static Selectable _previousSelected;
        
        public static void Initialize(bool unSelectOnPointerExit = true)
        {
            _results = new List<RaycastResult>();
            _pointerEventData = new PointerEventData(EventSystem.current);
         
            _unSelectOnPointerExit =  unSelectOnPointerExit;

            GameObject firstToSelect = EventSystem.current.firstSelectedGameObject;
            
            _selected = firstToSelect.GetComponent<Selectable>();
            EventSystem.current.SetSelectedGameObject(firstToSelect);
            _previousSelected = _selected;

            var inputSystem = EventSystem.current.GetComponent<InputSystemUIInputModule>();
            if (inputSystem)
            {
                inputSystem.move.action.performed += context =>
                {
                    if (context.canceled) return;
                    
                    if (!EventSystem.current.currentSelectedGameObject && _previousSelected.gameObject)
                    {
                        EventSystem.current.SetSelectedGameObject(_previousSelected.gameObject);
                    }
                };
            }
        }

        public static void SelectPrevious()
        {
            EventSystem.current.SetSelectedGameObject(_previousSelected.gameObject);
        }
        
        public static void TickMouseHover()
        {
            if (!EventSystem.current.currentSelectedGameObject && _previousSelected)
            {
                EventSystem.current.SetSelectedGameObject(_previousSelected.gameObject);
            }
            
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (mousePosition == _lastMousePosition)
                return;
            
            Debug.Log("TICK MOUSE HOVER");
            _pointerEventData.position = mousePosition;
            
            EventSystem.current.RaycastAll(_pointerEventData, _results);

            if (_results.Count > 0)
            {
                Selectable toSelect = _results[0].gameObject.GetComponentInParent<Selectable>();
                
                if (toSelect != null && toSelect.navigation.mode != Navigation.Mode.None)
                {
                    _selected = toSelect;
                
                    Debug.Log(_selected?.gameObject.name);
                
                    if (_selected && _previousSelected != _selected)
                    {
                        EventSystem.current.SetSelectedGameObject(_selected.gameObject);
                        _previousSelected = _selected;
                    }
                }
                
                if (_unSelectOnPointerExit)
                {
                    EventSystem.current.SetSelectedGameObject(null); 
                }
            }
            
            _lastMousePosition = mousePosition;
        }
    }
}

