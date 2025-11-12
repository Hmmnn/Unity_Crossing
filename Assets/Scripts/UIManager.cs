using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;

public class UIManager : MonoBehaviour
{
    private UIDocument gameUI;

    [SerializeField]
    private Sprite enabledLifeSprite;
    [SerializeField]
    private Sprite unabledLifeSprite;
    [SerializeField]
    private Sprite attakableSprite;
    [SerializeField]
    private Sprite unattackableSprite;

    [Header("Blur")]
    [SerializeField] Volume _volume;
    
    private StyleBackground unattackableBG;
    private StyleBackground unabledLifeBG;
    private StyleBackground attackableBG;
    private StyleBackground enabledLifeBG;

    private VisualElement[] _lifes = new VisualElement[3];
    private VisualElement _attackable;
    private Label _curScoreLabel;
    private VisualElement _gameOverScreen;
    private Label _newScoreLabel;
    private Label _bestScoreLabel;

    private const string _Life1Name = "Life1";
    private const string _Life2Name = "Life2";
    private const string _Life3Name = "Life3";
    private const string _AttackableName = "Attackable";
    private const string _ScoreFontName = "ScoreFont";

    private const string _GameOverScreenName = "UIGameOver";
    private const string _NewRecordName = "NewScore";
    private const string _BestRecordName = "BestScore";

    // score
    private int _curScore = 0;
    private int _bestScore = 0;
    private string _dataFilePath = "Assets/GameData/GameResult.txt";
    
    private bool _isShowingGameOverUI = false;

    #region Unity Method
    private void Start()
    {
        SetVisualElements();
        ReadDataFile();
    }

    private void Update()
    {
        if(_isShowingGameOverUI && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.instance.StartNewGame();
            StartNewGame();
        }
    }

    private void OnDestroy()
    {
        WriteDataFile();
    }
    #endregion

    #region Utils
    public void UpdateLife(int curLife)
    {
        for(int i = 0; i < _lifes.Length; ++i)
        {
            if (i < curLife)
                _lifes[i].style.backgroundImage = enabledLifeBG;
            else
                _lifes[i].style.backgroundImage = unabledLifeBG;
        }
    }

    public void UpdateAttackable(bool curState)
    {
        if (curState)
            _attackable.style.backgroundImage = attackableBG;
        else
            _attackable.style.backgroundImage = unattackableBG;
    }

    public void UpdateScore()
    {
        ++_curScore;
        string conv = _curScore.ToString();
        _curScoreLabel.text = (conv.Length < 2) ? "0" + conv : conv;
    }

    public void ShowGameOverUI()
    {
        if (_isShowingGameOverUI) return;
        _isShowingGameOverUI = true;

        SoundManager.instance.StopBGM();
        BlurBackground(true);
        ShowVisualElement(_gameOverScreen, true);

        string conv = _curScore.ToString();
        _newScoreLabel.text = (conv.Length < 2) ? "0" + conv : conv;

        if(_curScore > _bestScore)
            _bestScore = _curScore;

        string conv2 = _bestScore.ToString();
        _bestScoreLabel.text = (conv2.Length < 2) ? "0" + conv2 : conv2;

        WriteDataFile();
    }

    public void StartNewGame()
    {
        _isShowingGameOverUI = false;

        SoundManager.instance.PlayBGM();
        BlurBackground(false);
        ShowVisualElement(_gameOverScreen, false);

        _curScore = 0;
        _curScoreLabel.text = "00";
    }

    private void ShowVisualElement(VisualElement visualElement, bool state)
    {
        if (visualElement == null) return;

        visualElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void BlurBackground(bool state)
    {
        if (_volume == null) return;

        DepthOfField blurDOF;
        if(_volume.profile.TryGet<DepthOfField>(out blurDOF))
                blurDOF.active = state;
    }

    private void WriteDataFile()
    {
        FileStream fileStream = new FileStream(_dataFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter writer = new StreamWriter(fileStream);
        writer.Write(_bestScore);
        writer.Close();
    }

    private void ReadDataFile()
    {
        if (!File.Exists(_dataFilePath)) return;
        
        StreamReader reader = new StreamReader(_dataFilePath);
        string read = reader.ReadLine();
        _bestScore = int.Parse(read);
        reader.Close();
    }

    private void SetVisualElements()
    {
        gameUI = GetComponent<UIDocument>();
        VisualElement root = gameUI.rootVisualElement;

        _lifes[0] = root.Q<VisualElement>(_Life1Name);
        _lifes[1] = root.Q<VisualElement>(_Life2Name);
        _lifes[2] = root.Q<VisualElement>(_Life3Name);
        _attackable = root.Q<VisualElement>(_AttackableName);
        _curScoreLabel = root.Q<Label>(_ScoreFontName);

        _gameOverScreen = root.Q(_GameOverScreenName);
        _newScoreLabel = root.Q<Label>(_NewRecordName);
        _bestScoreLabel = root.Q<Label>(_BestRecordName);

        enabledLifeBG = new StyleBackground(enabledLifeSprite);
        attackableBG = new StyleBackground(attakableSprite);

        unattackableBG = new StyleBackground(unattackableSprite);
        unabledLifeBG = new StyleBackground(unabledLifeSprite);
    }
    #endregion
}
