using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object References :")]
    [SerializeField] Transform _base;
    [SerializeField] PlayerBehaviour _player;

    float _lastZpos = 0;
    bool _isGameOver = false;
    bool _isRevive = false;

    MenuManager _menuController;
    ScoreManager _scoreManager;

    public static event Action OnGameEnd;
    public static event Action OnRevive;
    public static event Action<int> OnScoreUpdated;

    private void Awake()
    {
        _scoreManager = GetComponent<ScoreManager>();
    }

    private void OnEnable()
    {
        Piece.OnGameOver += GameEnd;
        Piece.OnLastPieceExit += UpdateLastPos;
        Piece.OnGettingScore += SetScore;

        PlayerBehaviour.OnPlayerDeath += GameEnd;
        PlayerBehaviour.OnFirstJump += StartGameplay;
        AdsManager.OnRewardedAdWatchedComplete += Revive;
    }

    private void SetScore(int val)
    {
        _scoreManager.AddScore(val);
        OnScoreUpdated?.Invoke(_scoreManager.Score);
    }

    private void Start()
    {
        _menuController = MenuManager.GetInstance();
    }

    public void GameEnd()
    {
        if (_isGameOver) return;
        _isGameOver = true;

        OnGameEnd?.Invoke();

        _player.GameOver();


        
            _menuController.SwitchMenu(MenuType.GameOver);
       

        SoundController.GetInstance().PlayAudio(AudioType.GAMEOVER);

    }

    private void Revive()
    {
        _isGameOver = false;
        OnRevive?.Invoke();

        _menuController.SwitchMenu(MenuType.Gameplay);

        Vector3 revivePosition = Vector3.forward * _lastZpos;

        _base.position = revivePosition;
        _player.Revive(revivePosition + Vector3.up);
    }

    private void UpdateLastPos(Vector3 lastPos)
    {
        _lastZpos = lastPos.z;
    }

    public void StartGameplay()
    {
        if (_menuController.GetCurrentMenu != MenuType.Gameplay)
        {
            _menuController.SwitchMenu(MenuType.Gameplay);
        }
    }

    private void OnDisable()
    {
        Piece.OnGameOver -= GameEnd;
        Piece.OnLastPieceExit -= UpdateLastPos;
        Piece.OnGettingScore -= SetScore;

        PlayerBehaviour.OnPlayerDeath -= GameEnd;
        PlayerBehaviour.OnFirstJump -= StartGameplay;
        AdsManager.OnRewardedAdWatchedComplete += Revive;
    }
}
