using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHolder : MonoBehaviour
{
    public Sprite r;
    public Sprite n;
    public Sprite b;
    public Sprite q;
    public Sprite k;
    public Sprite p;
    public Sprite R;
    public Sprite N;
    public Sprite B;
    public Sprite Q;
    public Sprite K;
    public Sprite P;
    public static Dictionary<char, Sprite> dict = new Dictionary<char, Sprite>();

    void Awake()
    {
        dict['r'] = r;
        dict['n'] = n;
        dict['b'] = b;
        dict['q'] = q;
        dict['k'] = k;
        dict['p'] = p;

        dict['R'] = R;
        dict['N'] = N;
        dict['B'] = B;
        dict['Q'] = Q;
        dict['K'] = K;
        dict['P'] = P;
    }
}
