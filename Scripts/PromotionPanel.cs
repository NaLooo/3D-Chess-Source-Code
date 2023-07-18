using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromotionPanel : MonoBehaviour
{
    public GameObject panelGO;
    public Image rook;
    public Image knight;
    public Image bishop;
    public Image queen;

    public void Open(bool isWhite, Vector3 position)
    {
        panelGO.GetComponent<RectTransform>().position = MainCamera.cam.WorldToScreenPoint(position);
        panelGO.SetActive(true);
        rook.sprite = SpriteHolder.dict[isWhite ? 'R' : 'r'];
        knight.sprite = SpriteHolder.dict[isWhite ? 'N' : 'n'];
        bishop.sprite = SpriteHolder.dict[isWhite ? 'B' : 'b'];
        queen.sprite = SpriteHolder.dict[isWhite ? 'Q' : 'q'];
    }

    public void Close()
    {
        panelGO.SetActive(false);
    }
}
