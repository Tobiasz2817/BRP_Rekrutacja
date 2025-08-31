using ScriptableObjectsScripts;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace Ui
{
    public class SoulInformation : MonoBehaviour
    {
        [SerializeField] private Image MainImage;
        [SerializeField] private Button SoulButton;
        [HideInInspector] public SoulItem soulItem;

        public void SetSoulItem(SoulItem _soulItem, Action OnSoulClick = null)
        {
            soulItem = _soulItem;
            MainImage.sprite = soulItem.Avatar;
            if (OnSoulClick != null) SoulButton.onClick.AddListener(() => OnSoulClick());
        }
    }
}