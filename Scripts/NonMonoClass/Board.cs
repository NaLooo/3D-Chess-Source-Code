using System;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    static Dictionary<(int, int), Block> blocks = new Dictionary<(int, int), Block>();
    static Dictionary<(int, int), PieceController> board = new Dictionary<(int, int), PieceController>();
    static HashSet<Block> showingBlocks = new HashSet<Block>();
    static Stack<Command> commandBuffer = new Stack<Command>();
    static Stack<Command> temporalBuffer = new Stack<Command>();
    public static EventHandler afterMoveCallBack = Handler;
    Dictionary<char, int> map = new Dictionary<char, int>() { ['a'] = 0, ['b'] = 1, ['c'] = 2, ['d'] = 3, ['e'] = 4, ['f'] = 5, ['g'] = 6, ['h'] = 7 };
    public static Dictionary<char, bool> castling = new Dictionary<char, bool>();
    string enPassant = "-";

    public static bool blackBeingChecked = false;
    public static bool whiteBeingChecked = false;
    public static HashSet<(int, int)> blackBeingAttackedPositions = new HashSet<(int, int)>();
    public static HashSet<(int, int)> whiteBeingAttackedPositions = new HashSet<(int, int)>();

    public Board()
    {
        Reset();
    }

    public void Backward()
    {
        if (commandBuffer.Count == 0) return;
        var command = commandBuffer.Pop();
        command.Undo();
        temporalBuffer.Push(command);
        UIBoardController.UpdateBoard();
    }

    public void Forward()
    {
        if (temporalBuffer.Count == 0) return;
        var command = temporalBuffer.Pop();
        command.Execute();
        commandBuffer.Push(command);
        UIBoardController.UpdateBoard();
    }

    public void ToPresent()
    {
        while (temporalBuffer.Count > 0)
        {
            var command = temporalBuffer.Pop();
            command.Execute();
            commandBuffer.Push(command);
        }
        UIBoardController.UpdateBoard();
    }

    public void MovePiece(PieceController piece, Block target)
    {
        if (piece.isFirstMove)
        {
            switch (piece.notation)
            {
                case 'p':
                case 'P':
                    if (piece.position.x == target.position.x && Mathf.Abs(piece.position.y - target.position.y) == 2)
                    {
                        enPassant = (char)(piece.position.x + 97) + ((piece.position.y + target.position.y) / 2 + 1).ToString();
                    }
                    break;
                case 'k':
                    enPassant = "-";
                    castling['k'] = false;
                    castling['q'] = false;
                    break;
                case 'K':
                    enPassant = "-";
                    castling['K'] = false;
                    castling['Q'] = false;
                    break;
                case 'r':
                    enPassant = "-";
                    if (piece.position.x == 0) castling['q'] = false;
                    if (piece.position.x == 7) castling['k'] = false;
                    break;
                case 'R':
                    enPassant = "-";
                    if (piece.position.x == 0) castling['Q'] = false;
                    if (piece.position.x == 7) castling['K'] = false;
                    break;
                default:
                    enPassant = "-";
                    break;
            }
        }
        else enPassant = "-";

        if (piece.mover.EnPassantBlock == target)
        {
            ExecuteCommand(new EnPassant(piece, piece.mover.enPassantPiece, piece.mover.EnPassantBlock));
        }
        else if ((piece.notation == 'k' || piece.notation == 'K') && Mathf.Abs(piece.position.x - target.position.x) == 2)
        {
            PieceController rook;
            Block rookTarget;
            if (target.position.x == 2)
            {
                rook = board[(0, piece.position.y)];
                rookTarget = blocks[(3, piece.position.y)];
            }
            else
            {
                rook = board[(7, piece.position.y)];
                rookTarget = blocks[(5, piece.position.y)];
            }
            ExecuteCommand(new Castling(piece, rook, target, rookTarget));
        }
        else
        {
            ExecuteCommand(new Move(piece, target));
        }
    }

    public void PromotionMove(PieceController pawn, Block target, char p)
    {
        ExecuteCommand(new Promotion(pawn, target, p));
    }

    void ExecuteCommand(Command c)
    {
        TurnOffHightLight();
        c.Execute(false);
        commandBuffer.Push(c);
        afterMoveCallBack?.Invoke(null, EventArgs.Empty);
        UIBoardController.UpdateBoard();
        UpdateAttackingStatus();
    }

    public void AIMove(string move)
    {
        var piece = board[(map[move[0]], move[1] - '1')];
        var target = blocks[(map[move[2]], move[3] - '1')];
        if (move.Length == 4)
        {
            MovePiece(piece, target);
        }
        else
        {
            var p = move[4];
            if (piece.isWhite) p = (char)(p - 32);
            PromotionMove(piece, target, p);
        }
    }

    public static void UpdatePiecePos(PieceController piece, (int, int) oldPos, (int, int) newPos)
    {
        if (board[oldPos] == piece) board[oldPos] = null;
        board[newPos] = piece;
    }

    public static bool CastlingAvailable(bool white, bool kingSide)
    {
        var positions = white ? whiteBeingAttackedPositions : blackBeingAttackedPositions;
        int y = white ? 0 : 7;
        List<(int, int)> list;
        if (kingSide)
        {
            list = new List<(int, int)>() { (5, y), (6, y) };
            if (positions.Contains((7, y))) return false;
        }
        else
        {
            list = new List<(int, int)>() { (1, y), (2, y), (3, y) };
            if (positions.Contains((0, y))) return false;
        }
        foreach (var m in list) if (positions.Contains(m) || board[m] != null) return false;
        return true;
    }

    public bool LegalTarget(Block block)
    {
        return showingBlocks.Contains(block);
    }

    public void HighLightBlocks(HashSet<(int, int)> list)
    {
        foreach (var e in list)
        {
            var block = blocks[e];
            block.Activate();
            showingBlocks.Add(block);
        }
    }

    void UpdateAttackingStatus()
    {
        blackBeingAttackedPositions.Clear();
        whiteBeingAttackedPositions.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = board[(i, j)];
                if (piece != null)
                {
                    if (piece.isWhite)
                    {
                        foreach (var e in piece.GetAttacking()) blackBeingAttackedPositions.Add(e);
                    }
                    else
                    {
                        foreach (var e in piece.GetAttacking()) whiteBeingAttackedPositions.Add(e);
                    }
                }
            }
        }
    }

    public void TurnOffHightLight()
    {
        foreach (var e in showingBlocks)
        {
            e.Deactivate();
        }
        showingBlocks.Clear();
    }

    public static void RegisterPiece(int x, int y, PieceController piece)
    {
        board[(x, y)] = piece;
    }

    public void Reset()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[(i, j)] = null;
            }
        }
        TurnOffHightLight();
        commandBuffer.Clear();
        temporalBuffer.Clear();
        castling['K'] = true;
        castling['Q'] = true;
        castling['k'] = true;
        castling['q'] = true;
        enPassant = "-";
    }

    public void CreateEmptyBoard(GameObject emptyBlock, GameObject BlockHolder)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {

                var block = GameObject.Instantiate(emptyBlock, Position.PositionToVector(j, i), Quaternion.identity, BlockHolder.transform).GetComponent<Block>();
                block.Init(j, i);
                blocks[(j, i)] = block;
            }
        }
    }

    public static PieceController GetPiece((int, int) pos)
    {
        return board[pos];
    }

    public static Block GetBlock((int, int) pos)
    {
        return blocks[pos];
    }

    public bool PositionIsEnemy((int, int) pos, PieceController piece)
    {
        if (board[pos] == null) return false;
        return piece.isWhite != board[pos].isWhite;
    }

    public bool PositionIsAlly((int, int) pos, PieceController piece)
    {
        if (board[pos] == null) return false;
        return piece.isWhite == board[pos].isWhite;
    }

    public bool PositionIsEmpty((int, int) pos)
    {
        return board[pos] == null;
    }

    public bool IsPresent()
    {
        return temporalBuffer.Count == 0;
    }

    static void Handler(object o, EventArgs e)
    {

    }

    public string GetNotation()
    {
        string res = "";

        for (int i = 7; i > -1; i--)
        {
            int cnt = 0;
            for (int j = 0; j < 8; j++)
            {
                var p = (j, i);
                if (board[p] != null)
                {
                    if (cnt != 0)
                    {
                        res += cnt.ToString();
                        cnt = 0;
                    }
                    res += board[p].notation;
                }
                else
                {
                    cnt++;
                }
            }
            if (cnt != 0) res += cnt.ToString();
            if (i != 0) res += "/";
        }
        res += " ";

        res += GameManager.whiteTurn ? "w" : "b";
        res += " ";

        string castle = "";
        if (castling['K']) castle += "K";
        if (castling['Q']) castle += "Q";
        if (castling['k']) castle += "k";
        if (castling['q']) castle += "q";
        if (castle == "") castle = "-";
        res += castle;
        res += " ";

        res += enPassant;
        UnityEngine.Debug.Log(res);

        return res;
    }
}