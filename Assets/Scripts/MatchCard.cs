using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class MatchCard : MonoBehaviour
{
    [HideInInspector]
    public Sprite SelectedMatchObjectImage;

    public MatchData matchFruitsForCard;
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
            matchObjImage.sprite = matchFruitsForCard.FruitImage;
        }
        else
        {
            BgImage.color = _unSelectedColor;
            matchObjImage.sprite = GameManager.Instance.questionMarkSprite;
        }
    }
}