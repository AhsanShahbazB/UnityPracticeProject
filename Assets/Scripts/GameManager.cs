using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.5f);
        for(int i = 0; i < _totalCards; i++)
        {
            MatchCard _cardInstantiated = Instantiate(CardPrefab, gameplayCardsScrollView.content).GetComponent<MatchCard>();
            _cardInstantiated.GetComponent<Button>().onClick.AddListener(() => CardSelected(_cardInstantiated));
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
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
                }
                yield return new WaitForSeconds(0.0001f);
            }
        }
    }
}