using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoardController : MonoBehaviour
{
    public GameObject boardGO;
    bool active = true;
    static Dictionary<(int, int), Image> board = new Dictionary<(int, int), Image>();
    static Dictionary<(int, int), RectTransform> children = new Dictionary<(int, int), RectTransform>();
    static Transform trans;
    public static bool isWhite = true;

    void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                children[(j, i)] = boardGO.transform.GetChild(i * 8 + j).GetChild(0).GetComponent<RectTransform>();
                board[(j, i)] = boardGO.transform.GetChild(i * 8 + j).GetChild(0).GetComponent<Image>();
                board[(j, i)].color = Constants.transparent;
            }
        }
        trans = boardGO.transform;
    }

    public void OnClick()
    {
        active = !active;
        boardGO.SetActive(active);
    }

    public static void FlipBoard()
    {
        isWhite = !isWhite;
        if (isWhite)
        {
            trans.rotation = Quaternion.identity;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    children[(j, i)].localRotation = Quaternion.identity;
                }
            }
        }
        else
        {
            trans.rotation = Constants.flip;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    children[(j, i)].localRotation = Constants.flip;
                }
            }
        }
    }

    public static void UpdateBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = Board.GetPiece((i, j));
                if (piece == null)
                {
                    board[(i, j)].color = Constants.transparent;
                }
                else
                {
                    board[(i, j)].sprite = piece.image;
                    board[(i, j)].color = Constants.normalColor;
                }
            }
        }
    }
}
