using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public int PlayerScore;

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
    public Text ShowPlayerScoreText;
    public AudioClip CardSelectAudio;
    public AudioClip CardUnselectAudio;
    public AudioClip CardMatchSuccessAudio;
    public AudioClip CardMatchFailAudio;
    public ScrollRect gameplayCardsScrollView;

    public GameObject CardPrefab;

    public GameObject MainPanel;
    public GameObject LayoutPanel;
    public GameObject GamePanel;

    private List<MatchCard> matchCards = new List<MatchCard>();

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip audioClip)
    {
        _audioSource.clip = audioClip;
        _audioSource.Play();
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
        var i = 0;
        List<int> previousRandomSelectedIndex = new List<int>();
        List<int> previousRandomMatchedSelectedIndex = new List<int>();
        while(i != 6)
        {
            
            if(i < (_totalCards / 2))
            {
                var selectedDataForCard = Random.Range(0, matchDatas.Count);
                if (matchDatas[selectedDataForCard].AmISelected == false)
                {
                    var selectRandomIndex = Random.Range(0, _totalCards);
                    if (!previousRandomSelectedIndex.Contains(selectRandomIndex))
                    {
                        previousRandomSelectedIndex.Add(selectRandomIndex);
                        matchDatas[selectedDataForCard].AmISelected = true;
                        yield return new WaitForSeconds(0.1f);
                        matchCards[selectRandomIndex].matchFruitsForCard.Fruits = matchDatas[selectedDataForCard].Fruits;
                        matchCards[selectRandomIndex].matchFruitsForCard.FruitImage = matchDatas[selectedDataForCard].FruitImage;
                        matchCards[selectRandomIndex].matchFruitsForCard.AmISelected = matchDatas[selectedDataForCard].AmISelected;
                        yield return new WaitForSeconds(0.1f);
                        i++;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                    var returnDataMatchedCards = matchDatas.FindAll((x) => x.AmISelected);
                    var selectRandomMatchedCard = Random.Range(0, returnDataMatchedCards.Count);
                    var selectRandomIndex = Random.Range(0, _totalCards);
                    if (!previousRandomSelectedIndex.Contains(selectRandomIndex) & !previousRandomMatchedSelectedIndex.Contains(selectRandomMatchedCard))
                    {
                        previousRandomSelectedIndex.Add(selectRandomIndex);
                        previousRandomMatchedSelectedIndex.Add(selectRandomMatchedCard);
                        yield return new WaitForSeconds(0.1f);
                        matchCards[selectRandomIndex].matchFruitsForCard.Fruits = returnDataMatchedCards[selectRandomMatchedCard].Fruits;
                        matchCards[selectRandomIndex].matchFruitsForCard.FruitImage = returnDataMatchedCards[selectRandomMatchedCard].FruitImage;
                        matchCards[selectRandomIndex].matchFruitsForCard.AmISelected = returnDataMatchedCards[selectRandomMatchedCard].AmISelected;
                        yield return new WaitForSeconds(0.1f);
                        i++;
                    }
                    yield return new WaitForEndOfFrame();
            }
                        
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
            PlayAudio(CardSelectAudio);
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
            PlayAudio(CardUnselectAudio);
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
            PlayAudio(CardMatchSuccessAudio);
            yield return new WaitForSeconds(0.1f);
            PlayerScore++;
            yield return new WaitForSeconds(0.1f);
            ShowPlayerScoreText.text = PlayerScore.ToString();
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
            yield return new WaitForSeconds(0.1f);
            if(_totalCards <= 0)
            {
                ResetGame();
            }
        }
        else
        {
         
            StartCoroutine(CardRotating(_firstCard));
            StartCoroutine(CardRotating(_secondCard));
            yield return new WaitForSeconds(0.1f);

            PlayAudio(CardMatchFailAudio);
        }
    }

    public void ResetGame()
    {
        MainPanel.SetActive(true);
        GamePanel.SetActive(false);
        PlayerScore = 0;
        ShowPlayerScoreText.text = PlayerScore.ToString();
    }
}
[Serializable]
public class MatchData
{
    public MatchFruits Fruits;
    public Sprite FruitImage;
    public bool AmISelected;
}

public enum MatchFruits
{
    Apple,
    Mango,
    Banana,
    Orange,
    Peach,
}