using System.Collections.Generic;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public static class SelectionService
    {
        static EventSystem _eventSystem;
        static GraphicRaycaster _raycaster;
        static InputSystemUIInputModule _inputModule;
        
        static GameObject _selection;

        static List<GameObject> _selections;

        static bool _isSelectableActive;
        
        public static void Initialize(GraphicRaycaster raycaster)
        {
            _inputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
            
            Assert.IsNotNull(_inputModule, "GUI Navigation support only new input system, means UIInputModule, change in event system on new module");
            
            _eventSystem = EventSystem.current;
            _selections = new();

            _raycaster = raycaster;
            
            _inputModule.move.action.performed += _ =>
            {
                DisableMouse();
                
                if (!_eventSystem.currentSelectedGameObject)
                    _eventSystem.SetSelectedGameObject(_selection ?? _eventSystem.firstSelectedGameObject);
                else
                    _selection = _eventSystem.currentSelectedGameObject;
            };
            
            _inputModule.leftClick.action.performed += _ =>
            {
                if (_eventSystem.currentSelectedGameObject)
                    _selection = _eventSystem.currentSelectedGameObject;
                
                if (!_isSelectableActive)
                    _eventSystem.SetSelectedGameObject(null);
            };
        }

        public static void Tick()
        {
            if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
            {   
                EnableMouse();
                
                _eventSystem.SetSelectedGameObject(null);
            }
        }

        public static void Select(GameObject selection, bool savePrevious = true)
        {
            if (savePrevious) SaveSelection(_selection);
            
            _selection = selection;
            
            if (_isSelectableActive)
                _eventSystem.SetSelectedGameObject(_selection);       
        }
        
        public static void SaveSelection(GameObject selection) => 
            _selections.Add(selection);

        public static void SelectPrevious()
        {
            Assert.IsTrue(_selections.Count > 0, "Selections array cannot be empty");
            
            GameObject last = null;
            for (int i = _selections.Count - 1; i >= 0; i--)
            {
                last = _selections[i];
                _selections.RemoveAt(i);
                
                if (last.activeInHierarchy)
                    break;
            }
            
            _selection = last;
            
            if (_isSelectableActive)
                _eventSystem.SetSelectedGameObject(last);
        }

        static void EnableMouse()
        {
            _raycaster.enabled = true;
            Cursor.visible = true;
            _isSelectableActive = false;
        }
        
        static void DisableMouse()
        {
            _raycaster.enabled = false;
            Cursor.visible = false;
            _isSelectableActive = true;
        }
    }
}