using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : Movement
{
    bool enPassant = false;
    public override HashSet<(int, int)> GetMoves(PieceController piece)
    {
        var res = new HashSet<(int, int)>();
        int direction = piece.isWhite ? 1 : -1;
        if (piece.position.IsLegal(0, direction) && GameManager.board.PositionIsEmpty(piece.position.OffSet((0, direction))))
        {
            res.Add((piece.position.x, piece.position.y + direction));
            if (piece.isFirstMove && piece.position.IsLegal(0, direction + direction) && GameManager.board.PositionIsEmpty(piece.position.OffSet((0, direction + direction))))
            {
                res.Add((piece.position.x, piece.position.y + direction + direction));
            }
        }
        if (piece.position.IsLegal(-1, direction) && GameManager.board.PositionIsEnemy(piece.position.OffSet((-1, direction)), piece))
        {
            res.Add((piece.position.x - 1, piece.position.y + direction));
        }
        if (piece.position.IsLegal(1, direction) && GameManager.board.PositionIsEnemy(piece.position.OffSet((1, direction)), piece))
        {
            res.Add((piece.position.x + 1, piece.position.y + direction));
        }
        if (enPassant)
        {
            res.Add((enPassantPiece.position.x, enPassantPiece.position.y + direction));
        }
        return res;
    }

    public override HashSet<(int, int)> GetAttacking(PieceController piece)
    {
        var res = new HashSet<(int, int)>();
        int direction = piece.isWhite ? 1 : -1;

        if (piece.position.IsLegal(-1, direction)) res.Add((piece.position.x - 1, piece.position.y + direction));
        if (piece.position.IsLegal(1, direction)) res.Add((piece.position.x + 1, piece.position.y + direction));

        return res;
    }

    public override void MoveCallBack(PieceController piece, Position target)
    {
        if (Mathf.Abs(piece.position.y - target.y) == 2)
        {
            var enemy = piece.isWhite ? 'p' : 'P';
            var direction = piece.isWhite ? -1 : 1;
            if (target.x > 0)
            {
                var p = Board.GetPiece((target.x - 1, target.y));
                if (p != null && p.notation == enemy) p.mover.EnPassant(piece, Board.GetBlock((target.x, target.y + direction)));
            }
            if (target.x < 7)
            {
                var p = Board.GetPiece((target.x + 1, target.y));
                if (p != null && p.notation == enemy) p.mover.EnPassant(piece, Board.GetBlock((target.x, target.y + direction)));
            }
        }
    }

    public override void EnPassant(PieceController piece, Block block)
    {
        enPassant = true;
        enPassantPiece = piece;
        EnPassantBlock = block;
        Board.afterMoveCallBack += AfterMoveCallBack;
    }

    public void AfterMoveCallBack(object o, EventArgs e)
    {
        enPassant = false;
        enPassantPiece = null;
        EnPassantBlock = null;
        Board.afterMoveCallBack -= AfterMoveCallBack;
    }
}
