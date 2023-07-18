using System.Collections.Generic;
using UnityEngine;

public class DiedPiece : MonoBehaviour
{
    public GameObject r;
    public GameObject n;
    public GameObject b;
    public GameObject q;
    public GameObject k;
    public GameObject p;
    public GameObject R;
    public GameObject N;
    public GameObject B;
    public GameObject Q;
    public GameObject K;
    public GameObject P;
    public Dictionary<char, GameObject> piecePrefabs = new Dictionary<char, GameObject>();
    public static Transform holder;

    void Awake()
    {
        holder = transform;
        piecePrefabs['r'] = r;
        piecePrefabs['n'] = n;
        piecePrefabs['b'] = b;
        piecePrefabs['q'] = q;
        piecePrefabs['k'] = k;
        piecePrefabs['p'] = p;

        piecePrefabs['R'] = R;
        piecePrefabs['N'] = N;
        piecePrefabs['B'] = B;
        piecePrefabs['Q'] = Q;
        piecePrefabs['K'] = K;
        piecePrefabs['P'] = P;
    }
}