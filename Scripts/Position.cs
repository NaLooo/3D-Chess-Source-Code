using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public int x;
    public int y;
    public (int, int) tuple { get { return (x, y); } }
    public Vector3 VectorPosition
    {
        get
        {
            return Constants.boardOrigin + new Vector3(x * Constants.blockWidth, 0, y * Constants.blockWidth);
        }
    }

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void Copy(Position pos)
    {
        x = pos.x;
        y = pos.y;
    }

    public Position Duplicate()
    {
        return new Position(x, y);
    }

    public (int, int) OffSet((int, int) offset)
    {
        return (x + offset.Item1, y + offset.Item2);
    }

    public bool IsLegal(int x, int y)
    {
        int newX = this.x + x;
        int newY = this.y + y;
        return newX >= 0 && newX < 8 && newY >= 0 && newY < 8;
    }

    public bool IsLegal((int, int) pos)
    {
        return IsLegal(pos.Item1, pos.Item2);
    }

    public string ToNotation()
    {
        return (char)(x + 65) + (y + 1).ToString();
    }

    public Vector3 ToVector()
    {
        return Constants.boardOrigin + new Vector3(x * Constants.blockWidth, 0, y * Constants.blockWidth);
    }

    public static Vector3 PositionToVector(Position pos)
    {
        return Constants.boardOrigin + new Vector3(pos.x * Constants.blockWidth, 0, pos.y * Constants.blockWidth);
    }

    public static Vector3 PositionToVector(int x, int y)
    {
        return Constants.boardOrigin + new Vector3(x * Constants.blockWidth, 0, y * Constants.blockWidth);
    }

    public static string PositionToNotation(Position position)
    {
        return (char)(position.x + 97) + (position.y + 1).ToString();
    }
}
