using System;
using UI;
using UnityEngine;

public enum GameLocalization
{
    SWAMPS,
    DUNGEON,
    CASTLE,
    CITY,
    TOWER
}

public class GameController : MonoBehaviour
{
    #region Singleton

    private static GameController _instance;

    public static GameController Instance
    {
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<GameController>();
            return _instance;
        }
        set => _instance = value;
    }

    private void Awake() => Instance = this;

    #endregion

    [SerializeField] private GameLocalization currentGameLocalization;

    public GameLocalization CurrentGameLocalization
    {
        get => currentGameLocalization;

        set => currentGameLocalization = value;
    }

    private bool _isPaused;

    public bool IsPaused
    {

        get => _isPaused;
        set
        {
            _isPaused = value;
            Time.timeScale = _isPaused ? 0f : 1f;
        }
    }

    public bool IsCurrentLocalization(GameLocalization localization)
    {
        return CurrentGameLocalization == localization;
    }
}