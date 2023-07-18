using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BishopMovement : GeneralMovement
{
    public override List<(int, int)> GetDirections()
    {
        return new List<(int, int)>() { (1, 1), (1, -1), (-1, -1), (-1, 1) };
    }
}
