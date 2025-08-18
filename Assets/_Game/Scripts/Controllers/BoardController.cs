using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public Action<LevelCondition.eCondition> _OnLevelComplete;

    public bool IsBusy { get; private set; }
    public bool CheckLose { get; private set; }

    private Board m_board;
    private Bottom m_bottom;
    private GameManager m_gameManager;
    private Camera m_cam;
    private GameSettings m_gameSettings;
    private bool m_gameOver;

    public Board Board => m_board;
    public bool GameOver => m_gameOver;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);
        m_bottom = new Bottom(this.transform, gameSettings);

        FillBoard();
    }

    private void FillBoard()
    {
        m_board.Fill();
    }

    public void SetCheckLoseCondition(bool value)
    {
        CheckLose = value;
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
        }
    }

    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                if (!cell.IsEmpty && cell.Interactable)
                {
                    OnClickCell(cell);
                }
            }
        }
    }

    public void OnClickCell(Cell cell)
    {
        if (cell.Type == Cell.eType.ON_BOARD)
        {
            IsBusy = true;
            m_bottom.AddItem(cell, () =>
            {
                StartCoroutine(FindMatchesAndCollapse());
            });
        }
        else if (cell.Type == Cell.eType.ON_BOTTOM)
        {
            m_board.AddItem(cell);
            StartCoroutine(DelayCollapseItem());
        }
    }

    IEnumerator FindMatchesAndCollapse()
    {
        List<Cell> matches = m_bottom.FindMatches();
        if (matches.Count >= m_gameSettings.DuplicateMin)
        {
            yield return new WaitForSeconds(.2f);

            foreach (Cell cell in matches)
            {
                cell.ExplodeItem();
            }

            yield return new WaitForSeconds(.2f);

            m_bottom.ShiftLeftItem();
        }

        IsBusy = false;
        CheckLevelComplete();
    }

    IEnumerator DelayCollapseItem()
    {
        yield return new WaitForSeconds(.2f);
        m_bottom.ShiftLeftItem();
    }

    void CheckLevelComplete()
    {
        if (m_board.IsEmpty())
        {
            _OnLevelComplete?.Invoke(LevelCondition.eCondition.WIN);
        }

        if (m_bottom.IsFull() && CheckLose)
        {
            _OnLevelComplete?.Invoke(LevelCondition.eCondition.LOSE);
        }
    }

    public Cell FindBestMove()
    {
        if (m_bottom.IsEmpty())
        {
            return FindRandomMove();
        }
        else
        {
            Cell targetCell = m_bottom.GetFirstCell();
            return m_board.GetCellWithType(targetCell);
        }
    }

    public Cell FindRandomMove()
    {
        return m_board.GetRandomCell();
    }

    public void SetBottomInteractable(bool state)
    {
        m_bottom.SetAllInteractable(state);
    }

    internal void Clear()
    {
        m_board.Clear();
    }
}
