using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingMovement : Movement
{
    List<(int, int)> moves = new List<(int, int)>() { (0, 1), (1, 0), (0, -1), (-1, 0), (1, 1), (1, -1), (-1, -1), (-1, 1) };
    public override HashSet<(int, int)> GetMoves(PieceController piece)
    {
        var res = new HashSet<(int, int)>();

        foreach (var m in moves)
        {
            bool pro = piece.isWhite ? Board.whiteBeingAttackedPositions.Contains(piece.position.OffSet(m)) : Board.blackBeingAttackedPositions.Contains(piece.position.OffSet(m));
            if (piece.position.IsLegal(m) && !GameManager.board.PositionIsAlly(piece.position.OffSet(m), piece) && !pro) res.Add(piece.position.OffSet(m));
        }

        if (piece.notation == 'k' && !Board.blackBeingChecked)
        {
            if (Board.CastlingAvailable(false, true)) res.Add((6, 7));
            if (Board.CastlingAvailable(false, false)) res.Add((2, 7));
        }
        else if (piece.notation == 'K' && !Board.whiteBeingChecked)
        {
            if (Board.CastlingAvailable(true, true)) res.Add((6, 0));
            if (Board.CastlingAvailable(true, false)) res.Add((2, 0));
        }

        return res;
    }

    public override HashSet<(int, int)> GetAttacking(PieceController piece)
    {
        var res = new HashSet<(int, int)>();

        foreach (var m in moves)
        {
            if (piece.position.IsLegal(m)) res.Add(piece.position.OffSet(m));
        }

        return res;
    }
}
