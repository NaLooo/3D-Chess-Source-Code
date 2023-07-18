using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookMovement : GeneralMovement
{
    public override List<(int, int)> GetDirections()
    {
        return new List<(int, int)>() { (0, 1), (1, 0), (0, -1), (-1, 0) };
    }
}
