using System.Collections.Generic;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using UnityEngine;

namespace Services
{
    public static class SelectionService
    {
        static GameObject _selection;
        static EventSystem _eventSystem;
        static InputSystemUIInputModule _inputModule;
        
        static List<GameObject> _selections;

        public static bool IsSelectableActive { private set; get; }

        #region Init

        public static void Initialize()
        {
            _inputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
            
            Assert.IsNotNull(_inputModule, "GUI Navigation support only new input system, means UIInputModule, change in event system on new module");
            
            _eventSystem = EventSystem.current;
            _selections = new();
            
            ConnectCallbacks();
        }

        public static void Disable()
        {
            DisconnectCallbacks();
        }

        static void ConnectCallbacks()
        {
            MouseService.OnPositionChange += MouseSelectionActive;
            _inputModule.move.action.performed += HandleMoveAction;
            _inputModule.leftClick.action.performed += HandleLeftClickAction;
        }
        
        static void DisconnectCallbacks()
        {
            MouseService.OnPositionChange -= MouseSelectionActive;
            _inputModule.move.action.performed -= HandleMoveAction;
            _inputModule.leftClick.action.performed -= HandleLeftClickAction;
        }

        #endregion
        
        public static void Select(GameObject selection, bool savePrevious = true)
        {
            if (savePrevious) SaveSelection(_selection);
            
            _selection = selection;
            
            if (IsSelectableActive)
                SetSelection(_selection);     
        }
        
        public static void SaveSelection(GameObject selection)
        {
            _selections.Add(selection);
        }

        public static void SelectPrevious()
        {
            Assert.IsTrue(_selections.Count > 0, "Selections array cannot be empty");
            
            GameObject previous = GetPrevious();
            Select(previous, false);
        }

        public static void ClearPreviousSelection()
        {
            GetPrevious(false);
        }

        static void HandleMoveAction(InputAction.CallbackContext _)
        {
            IsSelectableActive = true;
            MouseService.DisableMouse();
                
            if (!_eventSystem.currentSelectedGameObject)
            {
                GameObject toSelect = _selection ?? GetPrevious(false) ?? _eventSystem.firstSelectedGameObject;
                _eventSystem.SetSelectedGameObject(toSelect);
            }
            else
                _selection = _eventSystem.currentSelectedGameObject;
        }
        
        static void HandleLeftClickAction(InputAction.CallbackContext _)
        {
            if (_eventSystem.currentSelectedGameObject)
                _selection = _eventSystem.currentSelectedGameObject;
                
            if (!IsSelectableActive)
                _eventSystem.SetSelectedGameObject(null);
        }

        static void MouseSelectionActive()
        {
            IsSelectableActive = false;
            SetSelection(null);
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
        
        static void SetSelection(GameObject selection)
        {
            _eventSystem.SetSelectedGameObject(selection);       
        }
    }
}