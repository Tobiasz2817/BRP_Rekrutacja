using System.Collections.Generic;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine;

namespace Ui
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

            ConnectCallbacks();
        }

        public static void Disable()
        {
            DisconnectCallbacks();
        }

        static void ConnectCallbacks()
        {
            _inputModule.move.action.performed += HandleMoveAction;
            _inputModule.leftClick.action.performed += HandleLeftClickAction;
        }
        
        static void DisconnectCallbacks()
        {
            _inputModule.move.action.performed -= HandleMoveAction;
            _inputModule.leftClick.action.performed -= HandleLeftClickAction;
        }

        static void HandleMoveAction(InputAction.CallbackContext _)
        {
            DisableMouse();
                
            Debug.Log("HANDLE MOVE " + _selection + " CUR: " + _eventSystem.currentSelectedGameObject);
            
            if (!_eventSystem.currentSelectedGameObject)
            {
                GameObject toSelect = _selection ?? GetPrevious(false) ?? _eventSystem.firstSelectedGameObject;
                
                _eventSystem.SetSelectedGameObject(toSelect);
                
                Debug.Log("HANDLE MOVE TS " + toSelect + " CUR: " + _eventSystem.currentSelectedGameObject);
            }
            else
                _selection = _eventSystem.currentSelectedGameObject;
            
            Debug.Log("HANDLE MOVE " + _selection + " CUR: " + _eventSystem.currentSelectedGameObject);
        }
        
        static void HandleLeftClickAction(InputAction.CallbackContext _)
        {
            Debug.Log("HANDLE LEFT CLICK " + _selection + " CUR: " + _eventSystem.currentSelectedGameObject);
            
            if (_eventSystem.currentSelectedGameObject)
                _selection = _eventSystem.currentSelectedGameObject;
                
            if (!_isSelectableActive)
                _eventSystem.SetSelectedGameObject(null);
            
            Debug.Log("HANDLE LEFT CLICK " + _selection + " CUR: " + _eventSystem.currentSelectedGameObject);
        }

        public static void Tick()
        {
            if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
            {   
                EnableMouse();
                
                _eventSystem.SetSelectedGameObject(null, null);
            }
        }

        public static void Select(GameObject selection, bool savePrevious = true)
        {
            Debug.Log("SELECT SELECTION " + selection + " PREV: " + _selection + " " + _selection?.transform.GetSiblingIndex() + " SV: " + savePrevious);
            
            if (savePrevious) SaveSelection(_selection);
            
            _selection = selection;
            
            if (_isSelectableActive)
                _eventSystem.SetSelectedGameObject(_selection);       
        }
        
        public static void SaveSelection(GameObject selection)
        {
            Debug.Log("SAVING SELECTION " + selection + " " + selection?.transform.GetSiblingIndex());
            
            _selections.Add(selection);
        }

        public static void SelectPrevious()
        {
            Assert.IsTrue(_selections.Count > 0, "Selections array cannot be empty");
            
            _selection = GetPrevious(true);
            
            Debug.Log("SELECT PREV " + _selection);
            
            if (_isSelectableActive)
                _eventSystem.SetSelectedGameObject(_selection);
        }
        
        static GameObject GetPrevious(bool onlyActive = true)
        {
            GameObject last = null;
            for (int i = _selections.Count - 1; i >= 0; i--)
            {
                last = _selections[i];
                _selections.RemoveAt(i);
                
                if (!onlyActive || (last != null && last.activeInHierarchy))
                    break;
            }

            return last;
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