using ScriptableObjectsScripts;
using UnityEngine.UI;
using UnityEngine;
using Controllers;

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
    
        [Header("Scroll Navigation")]
        [SerializeField] private ScrollRect _inventorySoulsScrollRect;
        [SerializeField] private GridLayoutGroup _gridLayout;

        private RectTransform _contentParent;
        private GameObject _currentSelectedGameObject;
        private SoulInformation _currentSoulInformation;
        
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
                SoulInformation newSoul = Instantiate(SoulItemPlaceHolder.gameObject, _contentParent).GetComponent<SoulInformation>();
                newSoul.SetSoulItem(SoulController.Instance.Souls[i], () => SoulItem_OnClick(newSoul));
            }

            SoulItemPlaceHolder.gameObject.SetActive(false);
        }

        protected void OnEnable() => ClearSoulInformation();
    
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
            Destroy(_currentSelectedGameObject);
            ClearSoulInformation();
        }

        private void DestroyCurrentSoul()
        {
            Destroy(_currentSelectedGameObject);
            ClearSoulInformation();
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
                        SelectionService.SaveSelection(GetInventorySelection());

                        ScoreController.Instance.AddScore(_currentSoulInformation.soulItem.Points);
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
                    SelectionService.SaveSelection(GetInventorySelection());
                    GUIController.Instance.ShowPopUpMessage(popUpInfo);
                });
            }

            DestroyButton.gameObject.SetActive(active);
        }

        GameObject GetInventorySelection()
        {
            Transform selectedTransform = _currentSelectedGameObject.transform;
            int childCount = _contentParent.childCount - 1;
            
            int nextChildIndex = selectedTransform.GetSiblingIndex() + 1;

            if (nextChildIndex > childCount)
            {
                nextChildIndex -= 2;
                
                if (nextChildIndex < childCount)
                    return GetBackButton().gameObject;
            }
            
            return _contentParent.GetChild(nextChildIndex).gameObject;
        }
    }
}