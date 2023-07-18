using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPOOutline;

public class Block : MonoBehaviour, ISelectable
{
    public Outlinable outline;
    public Position position;

    public void Init(int x, int y)
    {
        outline = GetComponent<Outlinable>();
        outline.enabled = false;
        position = new Position(x, y);
    }

    public void Activate()
    {
        outline.enabled = true;
    }

    public void Deactivate()
    {
        outline.enabled = false;
    }

    public PieceController GetPiece()
    {
        return Board.GetPiece(position.tuple);
    }

    public Block GetBlock()
    {
        return this;
    }
}
