using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridAutoScroll : MonoBehaviour
    {
        [SerializeField] ScrollRect _gridScroll;
        GridLayoutGroup _gridLayoutGroup;

        void Awake()
        {
            _gridLayoutGroup = GetComponent<GridLayoutGroup>();
            
            var inputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
            Assert.IsNotNull(inputModule, "GUI Navigation support only new input system, means UIInputModule, change in event system on new module");

            inputModule.move.action.performed += ReadMoveAction;
        }

        void ReadMoveAction(InputAction.CallbackContext obj)
        {
            CalculateScrollPosition();
        }
        
        void CalculateScrollPosition()
        {
            GameObject target = EventSystem.current.currentSelectedGameObject;
            
            
        }
    }
}