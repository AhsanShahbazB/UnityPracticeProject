using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MatchCard : MonoBehaviour
{
    [HideInInspector]
    public Sprite SelectedMatchObjectImage;


    public Sprite UnSelectedMatchObjectImage;
    private Color _selectedColor = Color.green;
    private Color _unSelectedColor = Color.cyan;

    public bool IsSelected;

    public Image BgImage;

    public Image matchObjImage;

    public void UpdateCardSprites()
    {
        if(IsSelected)
        {
            BgImage.color = _selectedColor;
        //    matchObjImage.sprite = SelectedMatchObjectImage;
        }
        else
        {
            BgImage.color = _unSelectedColor;
            matchObjImage.sprite = UnSelectedMatchObjectImage;
        }
    }
}
