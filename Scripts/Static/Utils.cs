using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class Utils
{
    public static List<string> NotationConverter(string notation)
    {
        var subs = notation.Split(' ')[0].Split('/');

        List<string> res = new List<string>();
        foreach (var s in subs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var e in s)
            {
                int i = e - '0';
                if (i >= 0 && i < 10)
                {
                    for (int j = 0; j < i; j++)
                    {
                        sb.Append('-');
                    }
                }
                else
                {
                    sb.Append(e);
                }
            }
            res.Add(sb.ToString());
        }
        res.Reverse();

        return res;
    }
}
