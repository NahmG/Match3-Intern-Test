using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum eType
    {
        ON_BOARD = 0,
        ON_BOTTOM = 1
    }

    public int BoardX { get; private set; }
    public int BoardY { get; private set; }
    public Item Item { get; private set; }
    public eType Type { get; private set; }

    public bool Interactable { get; private set; }

    public bool IsEmpty => Item == null;

    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public void SetInteractable(bool state)
    {
        Interactable = state;
        col.enabled = state;
    }

    public void Setup(int cellX, int cellY)
    {
        this.BoardX = cellX;
        this.BoardY = cellY;
    }

    public void SetType(eType type)
    {
        Type = type;
    }

    public bool IsNeighbour(Cell other)
    {
        return BoardX == other.BoardX && Mathf.Abs(BoardY - other.BoardY) == 1 ||
            BoardY == other.BoardY && Mathf.Abs(BoardX - other.BoardX) == 1;
    }

    public void Free()
    {
        Item = null;
    }

    public void Assign(Item item)
    {
        Item = item;
        Item.SetCell(this);
    }

    public void ApplyItemPosition(bool withAppearAnimation)
    {
        Item.SetViewPosition(this.transform.position);

        if (withAppearAnimation)
        {
            Item.ShowAppearAnimation();
        }
    }

    internal void Clear()
    {
        if (Item != null)
        {
            Item.Clear();
            Item = null;
        }
    }

    internal bool IsSameType(Cell other)
    {
        return Item != null && other.Item != null && Item.IsSameType(other.Item);
    }

    internal void ExplodeItem()
    {
        if (Item == null) return;

        Item.ExplodeView();
        Item = null;
    }

    internal void AnimateItemForHint()
    {
        Item.AnimateForHint();
    }

    internal void StopHintAnimation()
    {
        Item.StopAnimateForHint();
    }

    internal void ApplyItemMoveToPosition()
    {
        Item.AnimationMoveToPosition();
    }
}
