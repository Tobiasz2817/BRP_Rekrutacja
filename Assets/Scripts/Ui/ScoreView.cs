using UnityEngine.UI;
using UnityEngine;
using Controllers;

namespace Ui
{
    public class ScoreView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] Text _scoreCountText;
        
        void Awake()
        {
            ScoreController.OnScoreChanged += UpdatePoints;
        }

        void OnDestroy()
        {
            ScoreController.OnScoreChanged -= UpdatePoints;
        }

        void UpdatePoints(uint points)
        {
            _scoreCountText.text = points.ToString();
        }
    }
}
