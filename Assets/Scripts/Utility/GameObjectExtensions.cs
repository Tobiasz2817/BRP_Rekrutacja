using UnityEngine;

namespace Utility
{
    public static class GameObjectExtensions
    {
        public static GameObject GetFirstActiveChild(this GameObject target)
        {
            foreach (Transform child in target.transform)
                if (child.gameObject.activeInHierarchy) 
                    return child.gameObject;

            return null;
        } 
    }
}