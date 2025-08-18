using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

using ITEM = NormalItem.eNormalType;

public class Bottom
{
    private int _bottomSize;
    private Cell[] _cells;
    private Transform _root;
    private int _boardSizeY;
    private GameSettings gameSettings;

    public Bottom(Transform transform, GameSettings gameSettings)
    {
        this.gameSettings = gameSettings;

        _root = transform;
        _bottomSize = gameSettings.BottomSize;
        _boardSizeY = gameSettings.BoardSizeY;

        _cells = new Cell[_bottomSize];

        Create();
    }

    public void Create()
    {
        Vector3 origin = new Vector3(-_bottomSize * 0.5f + 0.5f, -_boardSizeY * 0.5f - 1f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);

        for (int i = 0; i < _bottomSize; i++)
        {
            GameObject go = GameObject.Instantiate(prefabBG);
            go.transform.position = origin + new Vector3(i, 0, 0f);
            go.transform.SetParent(_root);

            Cell cell = go.GetComponent<Cell>();
            cell.SetType(Cell.eType.ON_BOTTOM);

            _cells[i] = cell;
        }
    }

    public void AddItem(Cell cell, Action callback = null)
    {
        if (IsFull()) return;

        NormalItem item = cell.Item as NormalItem;
        Cell targetCell = GetInsertCell(item);
        if (targetCell != null)
        {
            cell.Free();
            targetCell.Assign(item);
            item.AnimationMoveToPosition();

            callback?.Invoke();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            Cell cell = _cells[i];
            cell.Clear();

            GameObject.Destroy(cell.gameObject);
            _cells[i] = null;
        }
    }

    public bool IsFull()
    {
        return !_cells[_bottomSize - 1].IsEmpty;
    }

    public bool IsEmpty()
    {
        return _cells[0].IsEmpty;
    }

    public Cell GetFirstCell()
    {
        return _cells[0];
    }

    public void SetAllInteractable(bool state)
    {
        foreach (Cell cell in _cells)
        {
            cell.SetInteractable(state);
        }
    }

    public List<Cell> FindMatches()
    {
        List<Cell> temp = new List<Cell>();

        Cell targetCell = _cells[0];
        temp.Add(_cells[0]);
        int count = 1;

        for (int i = 1; i < _cells.Length; i++)
        {
            Cell cell = _cells[i];

            if (cell.IsEmpty)
            {
                return new List<Cell>();
            }
            else if (cell.IsSameType(targetCell))
            {
                temp.Add(cell);
                targetCell = cell;
                count++;
                if (count >= gameSettings.DuplicateMin)
                {
                    return temp;
                }
            }
            else
            {
                targetCell = cell;
                temp.Clear();
                temp.Add(cell);
                count = 1;
            }
        }

        return temp;
    }

    public void ShiftLeftItem()
    {
        int shifts = 0;
        for (int i = 0; i < _cells.Length; i++)
        {
            Cell cell = _cells[i];
            if (cell.IsEmpty)
            {
                shifts++;
                continue;
            }

            if (shifts == 0) continue;

            Cell holder = _cells[i - shifts];

            Item item = cell.Item;
            cell.Free();

            holder.Assign(item);
            item.AnimationMoveToPosition();
        }
    }

    Cell GetInsertCell(Item item)
    {
        /*
        Find similar type item existed in the collection 
        */
        int insertIndex = -1;
        for (int i = 0; i < _cells.Length; i++)
        {
            Cell cell = _cells[i];
            if (!cell.IsEmpty && cell.Item.IsSameType(item))
            {
                insertIndex = i + 1;
            }
        }

        if (insertIndex == -1)
        {
            return GetEmptyCell();
        }

        if (!_cells[insertIndex].IsEmpty)
        {
            /*
            Shift item from insertIndex to left by 1 cell
            */
            for (int i = _cells.Length - 1; i >= insertIndex; i--)
            {
                Cell cell = _cells[i];
                if (!cell.IsEmpty)
                {
                    Cell nextCell = _cells[i + 1];
                    Item currentItem = cell.Item;
                    cell.Free();
                    nextCell.Assign(currentItem);
                    currentItem.AnimationMoveToPosition();
                }
            }
        }

        return _cells[insertIndex];
    }

    Cell GetEmptyCell()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells[i].IsEmpty)
            {
                return _cells[i];
            }
        }

        return null;
    }
}