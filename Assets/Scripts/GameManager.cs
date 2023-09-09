using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<MatchData> matchDatas = new List<MatchData>();

    private int _singlePairSelected;

    private MatchCard _firstCard;
    private MatchCard _secondCard;
    public enum Layouts
    {
        _2x3,_3x2
    };

    private int _totalCards;

    public Layouts SelectedUserLayout;

    public ScrollRect gameplayCardsScrollView;

    public GameObject CardPrefab;

    public GameObject LayoutPanel;
    public GameObject GamePanel;

    private List<MatchCard> matchCards = new List<MatchCard>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this);
        }
    }

    public void SelectLayout(int selectLayout)
    {
        if(selectLayout == 0)
        {
            SelectedUserLayout = Layouts._2x3;
            gameplayCardsScrollView.content.GetComponent<GridLayoutGroup>().constraintCount = 3;
            _totalCards = 2 * 3;
        }
        else
        {
            SelectedUserLayout = Layouts._3x2;
            gameplayCardsScrollView.content.GetComponent<GridLayoutGroup>().constraintCount = 2;
            _totalCards = 3 * 2;
        }
        GamePanel.SetActive(true);
        LayoutPanel.SetActive(false);
        StartCoroutine(StartGame());
    }

    public IEnumerator FillData()
    {
        for(int i = 0; i < _totalCards; i+=2)
        {
            var selectedDataForCard = Random.Range(0, matchDatas.Count);
            matchCards[i].matchFruitsForCard.Fruits = matchDatas[selectedDataForCard].Fruits;
            matchCards[i].matchFruitsForCard.FruitImage = matchDatas[selectedDataForCard].FruitImage;
            yield return null;
        }

        for(int i = 1; i < _totalCards; i+=2)
        {
            matchCards[i].matchFruitsForCard.Fruits = matchCards[i - 1].matchFruitsForCard.Fruits;
            matchCards[i].matchFruitsForCard.FruitImage = matchCards[i - 1].matchFruitsForCard.FruitImage;
        }
    }

    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.5f);
        for(int i = 0; i < _totalCards; i++)
        {
            MatchCard _cardInstantiated = Instantiate(CardPrefab, gameplayCardsScrollView.content).GetComponent<MatchCard>();
            _cardInstantiated.GetComponent<Button>().onClick.AddListener(() => CardSelected(_cardInstantiated));
            yield return new WaitForSeconds(0.5f);
            matchCards.Add(_cardInstantiated);
        }
        StartCoroutine(FillData());
    }

    public void CardSelected(MatchCard selectedCard)
    {
        StartCoroutine(CardRotating(selectedCard));
    }
    IEnumerator CardRotating(MatchCard cardToRotate)
    {
        if(cardToRotate.IsSelected == false)
        {
            while (cardToRotate.IsSelected == false)
            {
                cardToRotate.gameObject.transform.Rotate(new Vector3(0, 2, 0) * Time.deltaTime * 180f * 1);
                if (cardToRotate.transform.rotation.w <= 0f)
                {
                    cardToRotate.IsSelected = true;
                    cardToRotate.UpdateCardSprites();
                    _singlePairSelected++;
                    if(_singlePairSelected > 2)
                    {
                        _singlePairSelected = 2;
                    }

                    if(_firstCard == null)
                    {
                        _firstCard = cardToRotate;
                    }
                    else if(_secondCard == null)
                    {
                        _secondCard = cardToRotate;
                    }
                }
                yield return new WaitForSeconds(0.0001f);
            }
        }
        else
        {
            while (cardToRotate.IsSelected)
            {
                cardToRotate.gameObject.transform.Rotate(new Vector3(0, 2, 0) * Time.deltaTime * 180f * -1);
                if (cardToRotate.transform.rotation.w >= 0.999f)
                {
                    cardToRotate.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
                    cardToRotate.IsSelected = false;
                    cardToRotate.UpdateCardSprites();
                    _singlePairSelected--;
                    if(_singlePairSelected < 0)
                    {
                        _singlePairSelected = 0;
                    }
                    if(cardToRotate == _firstCard)
                    {
                        _firstCard = null;
                    }
                    else
                    {
                        _secondCard = null;
                    }
                }
                yield return new WaitForSeconds(0.0001f);
            }
        }
    }

    
    private void Update()
    {
        if(_singlePairSelected == 2)
        {   
            if((_firstCard & _secondCard))
            {
                _singlePairSelected = 0;
                StartCoroutine(CheckIfCardsMatch());
            }
        }
    }
    IEnumerator CheckIfCardsMatch()
    {
        if (_firstCard.matchFruitsForCard.Fruits == _secondCard.matchFruitsForCard.Fruits)
        {
            yield return new WaitForSeconds(0.1f);
            matchCards.Remove(_firstCard);
            yield return new WaitForSeconds(0.1f);
            matchCards.Remove(_secondCard);
            yield return new WaitForSeconds(0.1f);
            _totalCards -= 2;
            yield return new WaitForSeconds(0.1f);
            Destroy(_firstCard.gameObject);
            yield return new WaitForSeconds(0.1f);
            Destroy(_secondCard.gameObject);
        }
        else
        {
            StartCoroutine(CardRotating(_firstCard));
            StartCoroutine(CardRotating(_secondCard));
        }
            
    }
}
[Serializable]
public class MatchData
{
    public MatchFruits Fruits;
    public Sprite FruitImage;
}

public enum MatchFruits
{
    Apple,
    Mango,
    Banana,
    Orange,
    Peach,
}