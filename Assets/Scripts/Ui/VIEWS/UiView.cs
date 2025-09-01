using UnityEngine.UI;
using Controllers;
using UnityEngine;
using Services;
using System;

namespace Ui
{
    public class UiView : MonoBehaviour
    {
        [Header("UI VIEW elements")] [SerializeField]
        private bool UnpauseOnClose = false;

        [SerializeField] private bool CloseOnNewView = true;
        [SerializeField] Button BackButon;

        private UiView _parentView;

        GameObject _lastSelection;

        public virtual void Awake()
        {
            BackButon.onClick.AddListener(() => DisableView_OnClick(this));
        }

        public void ActiveView_OnClick(UiView viewToActive)
        {
            viewToActive.SetParentView(this);
            viewToActive.ActiveView();
            
            this.ActiveView(!CloseOnNewView);
        }

        public virtual void SelectViewElement()
        {
            SelectionService.Select(BackButon.gameObject);
        }

        private void DisableView_OnClick(UiView viewToDisable)
        {
            viewToDisable.DisableView();
        }

        public void DestroyView_OnClick(UiView viewToDisable)
        {
            viewToDisable.DestroyView();
        }

        public void SetParentView(UiView parentView)
        {
            _parentView = parentView;
        }

        public void ActiveView(bool active)
        {
            this.gameObject.SetActive(active);
        }

        public void ActiveView(Action onBackButtonAction = null)
        {
            if (onBackButtonAction != null) BackButon.onClick.AddListener(() => onBackButtonAction());

            if (!gameObject.activeSelf)
            {
                this.ActiveView(true);
                SelectViewElement();
            }
        }

        public void DisableView()
        {
            if (_parentView != null)
            {
                _parentView.ActiveView();
            }
        
            if (UnpauseOnClose) GameController.Instance.IsPaused = false;

            this.ActiveView(false);
            SelectionService.SelectPrevious();
        }

        public void DestroyView()
        {
            if (_parentView != null)
            {
                _parentView.ActiveView();
            }

            Destroy(this.gameObject);
            SelectionService.SelectPrevious();
        }

        public void DisableBackButton()
        {
            BackButon.gameObject.SetActive(false);
        }

        public Button GetBackButton()
        {
            return BackButon;
        }
    }
}