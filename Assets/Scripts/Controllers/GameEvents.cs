using UnityEngine.UI;

namespace Controllers
{
    public static class GameEvents
    {
        public delegate void OnEnemyKilled(IEnemy enemy, KillInformation killedInformation);
        public static OnEnemyKilled EnemyKilled;
        public delegate void OnChangeGameplayNavigation(Navigation.Mode mode);
        public static OnChangeGameplayNavigation ChangeGameplayNavigation;
    }
}

