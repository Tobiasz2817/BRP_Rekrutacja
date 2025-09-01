using Controllers;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class GUIController : MonoBehaviour
    {
        #region singleton

        public static GUIController Instance;

        private void Awake()
        {
            DisableOnStartObject.SetActive(false);
            Instance = this;
        
            SelectionService.Initialize();
        }

        #endregion
        
        [Header("References")]
        [SerializeField] private GameObject DisableOnStartObject;
        [SerializeField] private RectTransform ViewsParent;
        [SerializeField] private GameObject InGameGuiObject;
        [SerializeField] private PopUpView PopUp;
        [SerializeField] private PopUpScreenBlocker ScreenBlocker;
        
        private void Start()
        {
            if (ScreenBlocker) ScreenBlocker.InitBlocker();
        }

        private void ActiveInGameGUI(bool active)
        {
            InGameGuiObject.SetActive(active);
        }

        public void ShowPopUpMessage(PopUpInformation popUpInfo)
        {
            PopUpView newPopUp = Instantiate(PopUp, ViewsParent) as PopUpView;
            newPopUp.ActivePopUpView(popUpInfo);
        }

        public void ActiveScreenBlocker(bool active, PopUpView popUpView)
        {
            if (active) ScreenBlocker.AddPopUpView(popUpView);
            else ScreenBlocker.RemovePopUpView(popUpView);
        }


        #region IN GAME GUI Clicks

        public void InGameGUIButton_OnClick(UiView viewToActive)
        {
            viewToActive.ActiveView(() =>
            {
                ActiveInGameGUI(true);
                GameEvents.ChangeGameplayNavigation?.Invoke(Navigation.Mode.Automatic);
            });

            ActiveInGameGUI(false);
            
            GameController.Instance.IsPaused = true;
            GameEvents.ChangeGameplayNavigation?.Invoke(Navigation.Mode.None);
            
            SelectionService.Select(viewToActive.GetEnableSelection(), true);
        }

        public void ButtonQuit()
        {
            Application.Quit();
        }
    
        #endregion
        
        #region UNITY CALLBACKS

        void Update() => MouseService.Tick();
        
        #endregion

    }
}