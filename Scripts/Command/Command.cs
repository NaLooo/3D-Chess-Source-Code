
using System;
public abstract class Command
{
    public Block enPassantBlock;
    public abstract void Execute(bool skip = true);
    public abstract void Undo();
}

public class Move : Command
{
    PieceController piece;
    PieceController capturedPiece;
    Block target;
    Position oldPos;
    public Move(PieceController piece, Block target)
    {
        this.piece = piece;
        this.target = target;
        capturedPiece = target.GetPiece();
        oldPos = piece.position.Duplicate();
    }

    public override void Execute(bool skip = true)
    {
        if (skip && capturedPiece != null) capturedPiece.gameObject.SetActive(false);
        piece.MoveTo(target.position, skip);
        Board.UpdatePiecePos(piece, oldPos.tuple, target.position.tuple);
    }

    public override void Undo()
    {
        if (capturedPiece != null)
        {
            capturedPiece.gameObject.SetActive(true);
            Board.UpdatePiecePos(capturedPiece, target.position.tuple, target.position.tuple);
        }
        piece.MoveTo(oldPos);
        Board.UpdatePiecePos(piece, target.position.tuple, oldPos.tuple);
    }
}

public class EnPassant : Command
{
    PieceController piece;
    PieceController enPassantPiece;
    Position oldPos;

    public EnPassant(PieceController piece, PieceController enPassantPiece, Block enPassantBlock)
    {
        this.piece = piece;
        this.enPassantPiece = enPassantPiece;
        this.enPassantBlock = enPassantBlock;
        oldPos = piece.position.Duplicate();
    }

    public override void Execute(bool skip = true)
    {
        if (skip)
        {
            enPassantPiece.gameObject.SetActive(false);
            piece.MoveTo(enPassantBlock.position, skip);
        }
        else
        {
            piece.EnPassant(enPassantBlock.position, enPassantPiece);
        }
        Board.UpdatePiecePos(piece, oldPos.tuple, enPassantBlock.position.tuple);
    }

    public override void Undo()
    {
        enPassantPiece.gameObject.SetActive(true);
        Board.UpdatePiecePos(piece, piece.position.tuple, oldPos.tuple);
        piece.MoveTo(oldPos);
    }
}

public class Castling : Command
{
    PieceController king;
    PieceController rook;
    Block kingTarget;
    Block rookTarget;
    Position kingOldPos;
    Position rookOldPos;

    public Castling(PieceController king, PieceController rook, Block kingTarget, Block rookTarget)
    {
        this.king = king;
        this.rook = rook;
        this.kingTarget = kingTarget;
        this.rookTarget = rookTarget;
        kingOldPos = king.position.Duplicate();
        rookOldPos = rook.position.Duplicate();
    }

    public override void Execute(bool skip = true)
    {
        king.MoveTo(kingTarget.position, skip);
        rook.MoveTo(rookTarget.position, skip);
        Board.UpdatePiecePos(king, kingOldPos.tuple, kingTarget.position.tuple);
        Board.UpdatePiecePos(rook, rookOldPos.tuple, rookTarget.position.tuple);
    }

    public override void Undo()
    {
        king.MoveTo(kingOldPos);
        rook.MoveTo(rookOldPos);
        Board.UpdatePiecePos(king, kingTarget.position.tuple, kingOldPos.tuple);
        Board.UpdatePiecePos(rook, rookTarget.position.tuple, rookOldPos.tuple);
    }
}

public class Promotion : Command
{
    PieceController pawn;
    PieceController capturedPiece;
    PieceController promotedPiece;
    Block target;
    Position oldPos;
    char p;

    public Promotion(PieceController pawn, Block target, char p)
    {
        this.p = p;
        this.pawn = pawn;
        this.target = target;
        capturedPiece = target.GetPiece();
        oldPos = pawn.position.Duplicate();
    }

    public override void Execute(bool skip = true)
    {
        if (skip)
        {
            if (capturedPiece != null) capturedPiece.gameObject.SetActive(false);
            pawn.MoveTo(target.position, skip);
            Board.UpdatePiecePos(pawn, oldPos.tuple, target.position.tuple);
            pawn.gameObject.SetActive(false);
            promotedPiece.gameObject.SetActive(true);
            Board.UpdatePiecePos(promotedPiece, target.position.tuple, target.position.tuple);
        }
        else
        {
            pawn.commandCallback += CreatePromotedPiece;
            pawn.MoveTo(target.position, skip);
            Board.UpdatePiecePos(pawn, oldPos.tuple, target.position.tuple);
        }
    }

    public override void Undo()
    {
        promotedPiece.gameObject.SetActive(false);
        if (capturedPiece != null)
        {
            capturedPiece.gameObject.SetActive(true);
            Board.UpdatePiecePos(capturedPiece, target.position.tuple, target.position.tuple);
        }
        pawn.gameObject.SetActive(true);
        pawn.MoveTo(oldPos);
        Board.UpdatePiecePos(pawn, target.position.tuple, oldPos.tuple);
    }

    public void CreatePromotedPiece(object o, EventArgs e)
    {
        pawn.commandCallback -= CreatePromotedPiece;
        pawn.gameObject.SetActive(false);
        promotedPiece = GameManager.CreatePiece(p, target.position.x, target.position.y);
        Board.UpdatePiecePos(promotedPiece, target.position.tuple, target.position.tuple);
    }
}