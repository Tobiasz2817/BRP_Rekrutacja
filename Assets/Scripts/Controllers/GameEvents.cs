using UnityEngine.UI;

namespace Controllers
{
    public static class GameEvents
    {
        public delegate void OnEnemyKilled(IEnemy enemy, KillInformation killedInformation);
        public static OnEnemyKilled EnemyKilled;
        
        public delegate void OnEnemySpawned(IEnemy enemy, int positionIndex);
        public static OnEnemySpawned EnemySpawned;

        public delegate void OnScoreChange(uint points, bool useCalculations = false);
        public static OnScoreChange ChangeScore;
        
        public delegate void OnChangeEnemyNavigation(Navigation.Mode mode);
        public static OnChangeEnemyNavigation ChangeEnemyNavigation;
    }
}

