using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Sprite questionMarkSprite;

    public Text WarningText_01;

    public Text WarningText_02;

    public int PlayerScore;

    public List<MatchData> MatchDatas = new List<MatchData>();

    private int _singlePairSelected;

   
    private MatchCard _firstCard;
    private MatchCard _secondCard;
    public enum Layouts
    {
        _2x3,_3x2
    };

   public int TotalCards;
    
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

    public List<MatchCard> MatchCards = new List<MatchCard>();

    private AudioSource _audioSource;

    private void Awake()
    {
        Debug.LogError(Application.persistentDataPath);
        if(Instance == null)
        {
            Instance = this;
        }
    }

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
            TotalCards = 2 * 3;
        }
        else
        {
            SelectedUserLayout = Layouts._3x2;
            gameplayCardsScrollView.content.GetComponent<GridLayoutGroup>().constraintCount = 2;
            TotalCards = 3 * 2;
        }
        GamePanel.SetActive(true);
        LayoutPanel.SetActive(false);
        StartCoroutine(StartGame(true));
    }

    public IEnumerator FillData()
    {
        var i = 0;
        List<int> previousRandomSelectedIndex = new List<int>();
        List<int> previousRandomMatchedSelectedIndex = new List<int>();
        while(i != 6)
        {
            
            if(i < (TotalCards / 2))
            {
                var selectedDataForCard = Random.Range(0, MatchDatas.Count);
                if (MatchDatas[selectedDataForCard].AmISelected == false)
                {
                    var selectRandomIndex = Random.Range(0, TotalCards);
                    if (!previousRandomSelectedIndex.Contains(selectRandomIndex))
                    {
                        previousRandomSelectedIndex.Add(selectRandomIndex);
                        MatchDatas[selectedDataForCard].AmISelected = true;
                        yield return new WaitForSeconds(0.1f);
                        MatchCards[selectRandomIndex].matchFruitsForCard.Fruits = MatchDatas[selectedDataForCard].Fruits;
                        MatchCards[selectRandomIndex].matchFruitsForCard.FruitImage = MatchDatas[selectedDataForCard].FruitImage;
                        MatchCards[selectRandomIndex].matchFruitsForCard.AmISelected = MatchDatas[selectedDataForCard].AmISelected;
                        yield return new WaitForSeconds(0.1f);
                        i++;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                    var returnDataMatchedCards = MatchDatas.FindAll((x) => x.AmISelected);
                    var selectRandomMatchedCard = Random.Range(0, returnDataMatchedCards.Count);
                    var selectRandomIndex = Random.Range(0, TotalCards);
                    if (!previousRandomSelectedIndex.Contains(selectRandomIndex) & !previousRandomMatchedSelectedIndex.Contains(selectRandomMatchedCard))
                    {
                        previousRandomSelectedIndex.Add(selectRandomIndex);
                        previousRandomMatchedSelectedIndex.Add(selectRandomMatchedCard);
                        yield return new WaitForSeconds(0.1f);
                    MatchCards[selectRandomIndex].matchFruitsForCard.Fruits = returnDataMatchedCards[selectRandomMatchedCard].Fruits;
                    MatchCards[selectRandomIndex].matchFruitsForCard.FruitImage = returnDataMatchedCards[selectRandomMatchedCard].FruitImage;
                    MatchCards[selectRandomIndex].matchFruitsForCard.AmISelected = returnDataMatchedCards[selectRandomMatchedCard].AmISelected;
                        yield return new WaitForSeconds(0.1f);
                        i++;
                    }
                    yield return new WaitForEndOfFrame();
            }
                        
        }
    }

    public IEnumerator StartGame(bool newGame,DataToSave dataLoad = null)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < TotalCards; i++)
        {
            MatchCard _cardInstantiated = Instantiate(CardPrefab, gameplayCardsScrollView.content).GetComponent<MatchCard>();
            _cardInstantiated.GetComponent<Button>().onClick.AddListener(() => CardSelected(_cardInstantiated));
            yield return new WaitForSeconds(0.5f);
            MatchCards.Add(_cardInstantiated);
        }
        yield return new WaitForEndOfFrame();
        if (newGame)
        {
            StartCoroutine(FillData());
        }
        else
        {
            StartCoroutine(FillLoadedData(dataLoad));
        }
    }

    IEnumerator FillLoadedData(DataToSave loadData)
    {
        for(int i = 0; i < loadData.cardFruitName.Count;i++)
        {
            MatchCards[i].matchFruitsForCard.FruitName = loadData.cardFruitName[i];
            if (loadData.cardFruitName[i] == "Apple")
            {
                MatchCards[i].matchFruitsForCard.Fruits = MatchFruits.Apple;
                MatchCards[i].matchFruitsForCard.FruitImage = MatchDatas[0].FruitImage;
            }
            else if (loadData.cardFruitName[i] == "Mango")
            {
                MatchCards[i].matchFruitsForCard.Fruits = MatchFruits.Mango;
                MatchCards[i].matchFruitsForCard.FruitImage = MatchDatas[1].FruitImage;
            }
            else if (loadData.cardFruitName[i] == "Banana")
            {
                MatchCards[i].matchFruitsForCard.Fruits = MatchFruits.Banana;
                MatchCards[i].matchFruitsForCard.FruitImage = MatchDatas[2].FruitImage;
            }
            else if (loadData.cardFruitName[i] == "Orange")
            {
                MatchCards[i].matchFruitsForCard.Fruits = MatchFruits.Orange;
                MatchCards[i].matchFruitsForCard.FruitImage = MatchDatas[3].FruitImage;
            }
            else if (loadData.cardFruitName[i] == "Peach")
            {
                MatchCards[i].matchFruitsForCard.Fruits = MatchFruits.Peach;
                MatchCards[i].matchFruitsForCard.FruitImage = MatchDatas[4].FruitImage;
            }
        }
        yield return null;
    }

    public void LoadGameToUser(DataToSave dataToLoad)
    {
        if (dataToLoad.layoutSelected == 0)
        {
            SelectedUserLayout = Layouts._2x3;
            gameplayCardsScrollView.content.GetComponent<GridLayoutGroup>().constraintCount = 3;
        }
        else
        {
            SelectedUserLayout = Layouts._3x2;
            gameplayCardsScrollView.content.GetComponent<GridLayoutGroup>().constraintCount = 2;
        }
        GamePanel.SetActive(true);
        MainPanel.SetActive(false);
        ShowPlayerScoreText.text = PlayerScore.ToString();
        StartCoroutine(StartGame(false,dataToLoad));
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
            MatchCards.Remove(_firstCard);
            yield return new WaitForSeconds(0.1f);
            MatchCards.Remove(_secondCard);
            yield return new WaitForSeconds(0.1f);
            TotalCards -= 2;
            yield return new WaitForSeconds(0.1f);
            Destroy(_firstCard.gameObject);
            yield return new WaitForSeconds(0.1f);
            Destroy(_secondCard.gameObject);
            yield return new WaitForSeconds(0.1f);
            if(TotalCards <= 0)
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
    public string FruitName;
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