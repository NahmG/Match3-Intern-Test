using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

using URandom = UnityEngine.Random;
using ITEM = NormalItem.eNormalType;

public class Board
{
    public enum eMatchDirection
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    private int boardSizeX;

    private int boardSizeY;

    private Cell[,] m_cells;

    private Transform m_root;

    private int m_dupMin;

    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        m_dupMin = gameSettings.DuplicateMin;

        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;

        m_cells = new Cell[boardSizeX, boardSizeY];

        CreateBoard();
    }

    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);
                cell.SetInteractable(true);
                cell.SetType(Cell.eType.ON_BOARD);

                m_cells[x, y] = cell;
            }
        }
    }

    internal void Fill()
    {
        /*
        check if board size is divisible by 3
        */
        int boardSize = boardSizeX * boardSizeY;
        if (boardSize % m_dupMin != 0)
        {
            Debug.Log($"Board size invalid");
            return;
        }

        /*
        first distribute each type as 3
        */
        int itemCount = Enum.GetValues(typeof(ITEM)).Length;
        Dictionary<ITEM, int> itemDistribution = new Dictionary<ITEM, int>();
        foreach (ITEM item in Enum.GetValues(typeof(ITEM)))
        {
            itemDistribution[item] = m_dupMin;
        }

        /*
        choose a random item to distribute more 3 
        until no there no more cell
        */
        int remaining = boardSize - itemCount * m_dupMin;
        while (remaining > 0)
        {
            ITEM item = Utils.GetRandomNormalType();
            itemDistribution[item] += m_dupMin;
            remaining -= m_dupMin;
        }

        /*
        change dict to a list
        */
        List<ITEM> itemList = new List<ITEM>();
        foreach (var item in itemDistribution.Keys)
        {
            for (int i = 0; i < itemDistribution[item]; i++)
            {
                itemList.Add(item);
            }
        }

        /*
        shuffle list
        */
        int n = itemList.Count;
        while (n > 1)
        {
            n--;
            int k = URandom.Range(0, n + 1);
            ITEM value = itemList[k];
            itemList[k] = itemList[n];
            itemList[n] = value;
        }

        /*
        Add item to board
        */
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                NormalItem item = new NormalItem();

                item.SetType(itemList[boardSizeY * x + y]);
                item.SetView();
                item.SetViewRoot(m_root);

                cell.Assign(item);
                cell.ApplyItemPosition(false);

                itemCount++;
            }
        }
    }

    public void AddItem(Cell cell)
    {
        Item item = cell.Item;
        Cell targetCell = item.CellOnBoard;

        cell.Free();
        targetCell.Assign(item);
        item.AnimationMoveToPosition();
    }

    public Cell GetCellWithType(Cell cell)
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell _cell = m_cells[x, y];
                if (!_cell.IsEmpty && _cell.IsSameType(cell))
                    return _cell;
            }
        }

        return null;
    }

    public Cell GetRandomCell()
    {
        List<Cell> list = new List<Cell>();

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (!m_cells[x, y].IsEmpty)
                    list.Add(m_cells[x, y]);
            }
        }

        if (list.Count == 0)
            return null;

        Cell cell = list[URandom.Range(0, list.Count)];
        return cell;
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < m_cells.GetLength(0); i++)
        {
            for (int j = 0; j < m_cells.GetLength(1); j++)
            {
                if (!m_cells[i, j].IsEmpty)
                    return false;
            }
        }
        return true;
    }

    public void Clear()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                cell.Clear();

                GameObject.Destroy(cell.gameObject);
                m_cells[x, y] = null;
            }
        }
    }
}
