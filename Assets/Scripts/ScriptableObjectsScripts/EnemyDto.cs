using Controllers;
using UnityEngine;

namespace ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "Enemy Dto", menuName = "Enemy Dto", order = 0)]
    public class EnemyDto : ScriptableObject
    {
        public Sprite Sprite;
        public AttackType Weakness;
        public int KillPoints;
    }
}