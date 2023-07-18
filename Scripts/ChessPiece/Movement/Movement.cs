using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement
{
    public PieceController enPassantPiece = null;
    public Block EnPassantBlock = null;

    public static Movement GetMover(char c)
    {
        switch (c)
        {
            case 'r':
            case 'R':
                return new RookMovement();
            case 'n':
            case 'N':
                return new KnightMovement();
            case 'b':
            case 'B':
                return new BishopMovement();
            case 'q':
            case 'Q':
                return new QueenMovement();
            case 'k':
            case 'K':
                return new KingMovement();
            case 'p':
            case 'P':
                return new PawnMovement();
            default:
                return default;
        }
    }

    public abstract HashSet<(int, int)> GetMoves(PieceController piece);
    public abstract HashSet<(int, int)> GetAttacking(PieceController piece);
    public virtual void MoveCallBack(PieceController piece, Position target)
    {

    }

    public virtual void EnPassant(PieceController piece, Block block)
    {

    }
}
