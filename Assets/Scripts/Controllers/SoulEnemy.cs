using ScriptableObjectsScripts;
using UnityEngine.UI;
using UnityEngine;
using Services;

namespace Controllers
{
    public class SoulEnemy : MonoBehaviour, IEnemy
    {
        [SerializeField] private GameObject InteractionPanelObject;
        [SerializeField] private GameObject ActionsPanelObject;
        [SerializeField] private SpriteRenderer EnemySpriteRenderer;

        private SpawnPoint _enemyPosition;

        EnemyDto _dto;

        public void SetupEnemy(EnemyDto dto, SpawnPoint spawnPoint)
        {
            _dto = dto;
            _enemyPosition = spawnPoint;
            
            EnemySpriteRenderer.sprite = dto.Sprite;
            gameObject.SetActive(true);
            
            ActiveSelection();
            GameEvents.ChangeGameplayNavigation += ChangeNavigation;
        }

        void OnDestroy() => GameEvents.ChangeGameplayNavigation -= ChangeNavigation;

        void ChangeNavigation(Navigation.Mode mode)
        {
            foreach (var selectable in gameObject.GetComponentsInChildren<Selectable>(true))
            {
                Navigation nav = selectable.navigation;
                nav.mode = mode;
                selectable.navigation = nav;
            }
        }

        public SpawnPoint GetEnemyPosition()
        {
            return _enemyPosition;
        }

        public GameObject GetEnemyObject()
        {
            return this.gameObject;
        }

        public int GetEnemyRewardPoints()
        {
            return _dto.KillPoints;
        }

        private void ActiveCombatWithEnemy()
        {
            ActiveInteractionPanel(false);
            ActiveActionPanel(true);
            ActiveSelection();
        }

        private void ActiveInteractionPanel(bool active)
        {
            InteractionPanelObject.SetActive(active);
        }

        private void ActiveActionPanel(bool active)
        {
            ActionsPanelObject.SetActive(active);
        }

        private void ActiveSelection()
        {
            GameObject panelTarget = InteractionPanelObject.activeInHierarchy ? InteractionPanelObject : ActionsPanelObject;
        
            GameObject child = panelTarget.transform.GetChild(0).gameObject;
            SelectionService.Select(child, false);
        }

    
        private void UseBow()
        {
            // USE BOW
            GameEvents.EnemyKilled?.Invoke(this, new KillInformation
            {
                KilledByWeakness = _dto.Weakness == AttackType.Bow,
            });
        }

        private void UseSword()
        {
            // USE SWORD
            GameEvents.EnemyKilled?.Invoke(this, new KillInformation
            {
                KilledByWeakness = _dto.Weakness == AttackType.Sword,
            });
        }
        
        #region OnClicks

        public void Combat_OnClick()
        {
            ActiveCombatWithEnemy();
        }

        public void Bow_OnClick()
        {
            UseBow();
        }

        public void Sword_OnClick()
        {
            UseSword();
        }

        #endregion
    }


    public interface IEnemy
    {
        SpawnPoint GetEnemyPosition();
        GameObject GetEnemyObject();
        int GetEnemyRewardPoints();
    }

    public struct KillInformation
    {
        public bool KilledByWeakness;
        // we can expand it, killed by, can be dmg ... etc...
        // public AttackType KilledBy;
    }

    public enum AttackType
    {
        Bow,
        Sword,
    }
}