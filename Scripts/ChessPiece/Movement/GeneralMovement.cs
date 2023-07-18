using System.Collections.Generic;

public class GeneralMovement : Movement
{
    public override HashSet<(int, int)> GetMoves(PieceController piece)
    {
        var res = new HashSet<(int, int)>();
        foreach (var m in GetDirections())
        {
            for (int i = 1; i < 8; i++)
            {
                var t = (m.Item1 * i, m.Item2 * i);
                if (piece.position.IsLegal(t) && !GameManager.board.PositionIsAlly(piece.position.OffSet(t), piece))
                {
                    res.Add(piece.position.OffSet(t));
                    if (GameManager.board.PositionIsEnemy(piece.position.OffSet(t), piece)) break;
                }
                else break;
            }
        }
        return res;
    }

    public override HashSet<(int, int)> GetAttacking(PieceController piece)
    {
        return GetMoves(piece);
    }

    public virtual List<(int, int)> GetDirections()
    {
        return null;
    }
}