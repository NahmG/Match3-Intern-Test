﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTime : LevelCondition
{
    private float m_time;

    private GameManager m_mngr;
    private BoardController m_board;

    public override void Setup(float value, Text txt, GameManager mngr, BoardController board)
    {
        base.Setup(value, txt, mngr, board);

        m_mngr = mngr;

        m_board = board;
        m_board.SetBottomInteractable(true);
        m_board.SetCheckLoseCondition(false);

        m_board._OnLevelComplete += OnLevelComplete;

        m_time = value;

        UpdateText();
    }

    private void Update()
    {
        if (m_conditionCompleted) return;

        if (m_mngr.State != GameManager.eStateGame.GAME_STARTED) return;

        m_time -= Time.deltaTime;

        UpdateText();

        if (m_time <= -1f)
        {
            SetGameCondition(eCondition.LOSE);
            OnConditionComplete();
        }
    }

    void OnLevelComplete(eCondition gameCondition)
    {
        SetGameCondition(gameCondition);
        OnConditionComplete();
    }

    protected override void UpdateText()
    {
        if (m_time < 0f) return;

        m_txt.text = string.Format("TIME:\n{0:00}", m_time);
    }
}
