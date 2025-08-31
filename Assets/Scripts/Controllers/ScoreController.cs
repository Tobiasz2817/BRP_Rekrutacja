using System;
using UnityEngine;

namespace Controllers
{
    public class ScoreController : MonoBehaviour
    {
        #region Singleton

        private static ScoreController _instance;
        public static ScoreController Instance
        {
            get
            {
                if (_instance == null) _instance = FindFirstObjectByType<ScoreController>();
                return _instance;
            }
            set => _instance = value;
        }

        #endregion
        
        [Header("Settings")] 
        [Range(0, 100)]
        [SerializeField] float _pointsWeaknessMultiplier;
        
        uint _score;
        
        public static event Action<uint> OnScoreChanged; 
        
        void Awake() => ConnectCallbacks();
        void OnDestroy() => UnConnectCallbacks();

        void ConnectCallbacks()
        {
            GameEvents.EnemyKilled += UpdateScore;
        }

        void UnConnectCallbacks()
        {
            GameEvents.EnemyKilled -= UpdateScore;
        }

        void UpdateScore(IEnemy enemy, KillInformation killInfo)
        {
            AddScore((uint)enemy.GetEnemyRewardPoints(), killInfo.KilledByWeakness);
        }

        public void AddScore(uint score, bool useWeaknessMultiplier = false)
        {
            _score += CalculateScore(
                score,
                useWeaknessMultiplier
            );
            
            OnScoreChanged?.Invoke(_score);
        }

        uint CalculateScore(uint points, bool useMultiplier = false)
        {
            float adderPoints = points * (1 + _pointsWeaknessMultiplier / 100);
            return useMultiplier ? (uint)Mathf.CeilToInt(adderPoints) : points;
        }
    }
}