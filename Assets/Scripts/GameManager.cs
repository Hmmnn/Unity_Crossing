using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 게임 오버 / 게임 재시작
public class GameManager : MonoBehaviour
{
    public static GameManager instance {  get; private set; }

    private GameObject _player;
    private UIManager _UIManager;

    #region Unity Method
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _player = GameObject.Find("Player");
        Debug.Assert(_player, "Player Missing");

        _UIManager = GameObject.Find("GameUI").GetComponent<UIManager>();
        Debug.Assert(_UIManager, "UIManager Missing");
    }
    #endregion

    #region Utils
    public void GameOver()
    {
        _UIManager.ShowGameOverUI();
    }

    public void StartNewGame()
    {
        MapManager.instance.InitializePlayMap();
        _player.GetComponent<PlayerController>().ResetState();
    }
    #endregion
}
