using System.Collections;
using UnityEngine;

public class AutoPlayController : MonoBehaviour
{
    public enum eAutoMode
    {
        LOSE,
        WIN
    }

    private eAutoMode playMode;
    private BoardController _board;
    private float delayMove = .5f;

    public void SetUp(BoardController _boardController, eAutoMode mode)
    {
        this._board = _boardController;
        this.playMode = mode;
    }

    public void StartAutoplay()
    {
        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        while (!_board.Board.IsEmpty())
        {
            yield return new WaitForSeconds(delayMove);

            if (_board.GameOver) yield break;

            Cell cell = null;
            switch (playMode)
            {
                case eAutoMode.LOSE:
                    cell = _board.FindRandomMove();
                    break;
                case eAutoMode.WIN:
                    cell = _board.FindBestMove();
                    break;
            }

            if (cell != null)
            {
                _board.OnClickCell(cell);
            }
        }
    }
}