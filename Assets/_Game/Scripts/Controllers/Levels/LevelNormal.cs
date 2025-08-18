using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelNormal : LevelCondition
{
    private BoardController m_board;

    public override void Setup(Text txt, BoardController board)
    {
        base.Setup(txt, board);

        m_board = board;

        m_board._OnLevelComplete += OnLevelComplete;
        m_board.SetBottomInteractable(false);
        m_board.SetCheckLoseCondition(true);
    }

    private void OnLevelComplete(eCondition condition)
    {
        SetGameCondition(condition);
        OnConditionComplete();
    }

    protected override void OnDestroy()
    {
        if (m_board != null) m_board._OnLevelComplete -= OnLevelComplete;

        base.OnDestroy();
    }
}
