using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCondition : MonoBehaviour
{
    public enum eCondition
    {
        LOSE = 0,
        WIN = 1
    }

    public Action<eCondition> _OnConditionComplete;

    public eCondition gameCondition;

    protected Text m_txt;

    protected bool m_conditionCompleted = false;

    public virtual void Setup(float value, Text txt)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, GameManager mngr, BoardController board)
    {
        m_txt = txt;
    }

    public virtual void Setup(Text txt, BoardController board)
    {
        m_txt = txt;
    }

    protected virtual void UpdateText() { }

    protected void OnConditionComplete()
    {
        m_conditionCompleted = true;

        _OnConditionComplete(gameCondition);
    }

    protected void SetGameCondition(eCondition condition)
    {
        gameCondition = condition;
    }

    protected virtual void OnDestroy()
    {

    }
}
