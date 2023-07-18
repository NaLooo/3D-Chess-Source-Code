using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightMovement : Movement
{
    List<(int, int)> moves = new List<(int, int)>() { (1, 2), (2, 1), (2, -1), (1, -2), (-1, -2), (-2, -1), (-2, 1), (-1, 2) };
    public override HashSet<(int, int)> GetMoves(PieceController piece)
    {
        var res = new HashSet<(int, int)>();

        foreach (var m in moves)
        {
            if (piece.position.IsLegal(m) && !GameManager.board.PositionIsAlly(piece.position.OffSet(m), piece)) res.Add(piece.position.OffSet(m));
        }

        return res;
    }

    public override HashSet<(int, int)> GetAttacking(PieceController piece)
    {
        return GetMoves(piece);
    }
}
