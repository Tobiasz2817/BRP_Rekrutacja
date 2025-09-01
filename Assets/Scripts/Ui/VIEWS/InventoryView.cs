using ScriptableObjectsScripts;
using UnityEngine.UI;
using UnityEngine;
using Controllers;
using Services;
using Utility;

namespace Ui
{
    public class InventoryView : UiView
    {
        [Header("Inventory Elements")] [SerializeField]
        private SoulInformation SoulItemPlaceHolder;

        [SerializeField] private Text Description;
        [SerializeField] private Text Name;
        [SerializeField] private Image Avatar;
        [SerializeField] private Button UseButton;
        [SerializeField] private Button DestroyButton;

        private RectTransform _contentParent;
        private GameObject _currentSelectedGameObject;
        private SoulInformation _currentSoulInformation;
        
        // It's -1 because of first soul template
        int ContentChildCount => _contentParent.childCount - 1;
        
        public override void Awake()
        {
            base.Awake();
            _contentParent = (RectTransform)SoulItemPlaceHolder.transform.parent;
        
            InitializeInventoryItems();
        }
    
        private void InitializeInventoryItems()
        {
            for (int i = 0, j = SoulController.Instance.Souls.Count; i < j; i++)
            {
                SoulInformation newSoul = Instantiate(SoulItemPlaceHolder, _contentParent);
                newSoul.SetSoulItem(SoulController.Instance.Souls[i], () => SoulItem_OnClick(newSoul));
            }

            SoulItemPlaceHolder.gameObject.SetActive(false);
        }

        public override GameObject GetEnableSelection()
        {
            GameObject child = _contentParent.gameObject.GetFirstActiveChild();
            if (child != null)
                SoulItem_OnClick(child.GetComponent<SoulInformation>());
            
            return _contentParent.gameObject.GetFirstActiveChild() ?? GetBackButton().gameObject;
        }

        protected void OnEnable()
        {
            ClearSoulInformation();
            SelectionService.Select(GetEnableSelection());
        }

        private void ClearSoulInformation()
        {
            Description.text = "";
            Name.text = "";
            Avatar.sprite = null;
            SetupUseButton(false);
            SetupDestroyButton(false);
            _currentSelectedGameObject = null;
            _currentSoulInformation = null;
        }

        public void SoulItem_OnClick(SoulInformation soulInformation)
        {
            _currentSoulInformation = soulInformation;
            _currentSelectedGameObject = soulInformation.gameObject;
            SetupSoulInformation(soulInformation.soulItem);
        }

        private void SetupSoulInformation(SoulItem soulItem)
        {
            Description.text = soulItem.Description;
            Name.text = soulItem.Name;
            Avatar.sprite = soulItem.Avatar;
            SetupUseButton(soulItem.CanBeUsed);
            SetupDestroyButton(soulItem.CanBeDestroyed);
        }

        //A popup about the soul being unusable isn’t needed because we get new functionality.
        private void UseCurrentSoul()
        {
            ScoreController.Instance.AddScore(_currentSoulInformation.soulItem.Points);
            DestroyCurrentSoul();
        }

        private void DestroyCurrentSoul()
        {
            SelectionService.ClearPreviousSelection();
            
            GameObject soulNeighbour = GetCurrentSoulNeighbour();
            
            Destroy(_currentSelectedGameObject);
            ClearSoulInformation();
            
            if (soulNeighbour != null)
                SoulItem_OnClick(soulNeighbour.GetComponent<SoulInformation>());
        }

        private void SetupUseButton(bool active)
        {
            UseButton.onClick.RemoveAllListeners();
            if (active)
            {
                bool isInCorrectLocalization = GameController.Instance.IsCurrentLocalization(_currentSoulInformation.soulItem.UsableInLocalization);
                if (isInCorrectLocalization)
                {
                    PopUpInformation popUpInfo = new PopUpInformation
                    {
                        DisableOnConfirm = isInCorrectLocalization,
                        UseOneButton = false,
                        Header = "USE ITEM",
                        Message = "Are you sure you want to USE: " + _currentSoulInformation.soulItem.Name + " ?",
                        Confirm_OnClick = () => UseCurrentSoul()
                    };

                    UseButton.onClick.AddListener(() =>
                    {
                        SelectionService.SaveSelection(GetNextSelection());
                        GUIController.Instance.ShowPopUpMessage(popUpInfo);
                    });
                }
                
                UseButton.interactable = isInCorrectLocalization;
            }
            
            UseButton.gameObject.SetActive(active);
        }

        private void SetupDestroyButton(bool active)
        {
            DestroyButton.onClick.RemoveAllListeners();
            if (active)
            {
                PopUpInformation popUpInfo = new PopUpInformation
                {
                    DisableOnConfirm = true,
                    UseOneButton = false,
                    Header = "DESTROY ITEM",
                    Message = "Are you sure you want to DESTROY: " + Name.text + " ?",
                    Confirm_OnClick = () => DestroyCurrentSoul()
                };

                DestroyButton.onClick.AddListener(() =>
                {
                    SelectionService.SaveSelection(GetNextSelection());
                    GUIController.Instance.ShowPopUpMessage(popUpInfo);
                });
            }

            DestroyButton.gameObject.SetActive(active);
        }

        /// <summary>
        /// Finding next neighbour if didn't exist will return from the closest left side
        /// </summary>
        /// <returns> returns game objects neighbour</returns>
        GameObject GetCurrentSoulNeighbour()
        {
            Transform selectedTransform = _currentSelectedGameObject.transform;
            int childCount = ContentChildCount;
            
            int nextChildIndex = selectedTransform.GetSiblingIndex() + 1;

            if (nextChildIndex > childCount)
            {
                nextChildIndex -= 2;

                if (childCount < nextChildIndex)
                    return null;
            }
            
            return _contentParent.GetChild(nextChildIndex).gameObject;
        }

        GameObject GetNextSelection()
        {
            GameObject nextSoul = GetCurrentSoulNeighbour();

            if (nextSoul == null)
                return GetBackButton().gameObject;
            
            return nextSoul;
        }
    }
}