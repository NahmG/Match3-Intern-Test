using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Action<eStateGame> StateChangedAction;

    public enum eLevelMode
    {
        TIMER,
        NORMAL,
        AUTO_WIN,
        AUTO_LOSE
    }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_OVER,
        WIN
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction?.Invoke(m_state);
        }
    }

    private GameSettings m_gameSettings;

    private BoardController m_boardController;

    private AutoPlayController m_autoPlay;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        m_boardController?.Update();
    }

    internal void SetState(eStateGame state)
    {
        State = state;

        if (State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode)
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);

        if (mode == eLevelMode.NORMAL)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelNormal>();
            m_levelCondition.Setup(m_uiMenu.GetLevelConditionView(), m_boardController);
        }
        else if (mode == eLevelMode.TIMER)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelTime>();
            m_levelCondition.Setup(m_gameSettings.LevelTime, m_uiMenu.GetLevelConditionView(), this, m_boardController);
        }

        m_levelCondition._OnConditionComplete += OnGameFinish;

        State = eStateGame.GAME_STARTED;
    }

    public void StartAutoPlay(AutoPlayController.eAutoMode mode)
    {
        LoadLevel(eLevelMode.NORMAL);

        m_autoPlay = new GameObject("AutoPlay").AddComponent<AutoPlayController>();
        m_autoPlay.SetUp(m_boardController, mode);
        m_autoPlay.StartAutoplay();
    }

    public void OnGameFinish(LevelCondition.eCondition gameCondition)
    {
        switch (gameCondition)
        {
            case LevelCondition.eCondition.LOSE:
                StartCoroutine(WaitBoardController(eStateGame.GAME_OVER));
                break;
            case LevelCondition.eCondition.WIN:
                StartCoroutine(WaitBoardController(eStateGame.WIN));
                break;
        }
    }

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }
    }

    private IEnumerator WaitBoardController(eStateGame state)
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        SetState(state);

        if (m_levelCondition != null)
        {
            m_levelCondition._OnConditionComplete = null;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
}
