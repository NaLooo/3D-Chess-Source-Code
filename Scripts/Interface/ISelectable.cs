using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    public PieceController GetPiece();
    public Block GetBlock();
}
